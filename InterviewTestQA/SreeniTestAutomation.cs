using Amazon.SQS;
using Amazon.SQS.Model;
using Lorax.Core.Config;
using Lorax.Core.Graph;
using Lorax.Core.Graph.Model;
using Lorax.Core.Messaging.Model.SQS;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace InterviewTestQA
{
    public class SreeniTestAutomation
    {
        private const string AWSAccountNumber = "730335407970";
        private readonly IAmazonSQS _sqsClient;
        private readonly SqsConfiguration _config;
        private readonly ICloudWatchLogger _logger;

        public LoraxSQS(IAmazonSQS sqsClient, SqsConfiguration config, ICloudWatchLogger logger)
        {
            _sqsClient = sqsClient;
            _config = config;
            _logger = logger;
        }


        public async Task SendMessage(SendMessage messageRequest)
        {
            try
            {
                await _sqsClient.SendMessageAsync(new SendMessageRequest
                {
                    QueueUrl = GetQueueUrl(),
                    MessageBody = JsonConvert.SerializeObject(messageRequest, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                });
            }
            catch (Exception ex)
            {
                HandleSQSException(ex);
            }
        }


        public async Task<IEnumerable<SendMessageResult>> SendMessages(List<SendMessage> messages)
        {
            var results = new List<SendMessageResult>();

            await foreach (var result in _sqsClient.SendMessageBatchAsync(new SendMessageBatchRequest
            {
                QueueUrl = GetQueueUrl(),
                Entries = messages.Select(msg => new SendMessageBatchRequestEntry(msg.Id, msg.MessageBody)).ToList()
            }))
            {
                results.AddRange(result.Successful);
            }

            return results;
        }


        public async Task CreateSQSQueue()
        {
            try
            {
                await _sqsClient.CreateQueueAsync(new CreateQueueRequest
                {
                    QueueName = _config.QueueName,
                    Attributes = _config.IsFifo ? new Dictionary<string, string> { { "FifoQueue", "true" } } : null
                });
            }
            catch (QueueDeletedRecentlyException ex)
            {
                _logger.LogWarning($"Queue '{_config.QueueName}' was deleted recently. Error: {ex.Message}");
            }
            catch (QueueNameExistsException ex)
            {
                _logger.LogInformation($"Queue '{_config.QueueName}' already exists. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                HandleSQSException(ex);
            }
        }


        public async Task DeleteSQSQueue()
        {
            try
            {
                await _sqsClient.DeleteQueueAsync(GetQueueUrl());
            }
            catch (QueueDoesNotExistException ex)
            {
                _logger.LogWarning($"Queue '{_config.QueueName}' does not exist. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                HandleSQSException(ex);
            }
        }


        private string GetQueueUrl()
        {
            return $"https://sqs.amazonaws.com/{AWSAccountNumber}/{_config.QueueName}";
        }

        private async Task HandleSQSException(Exception ex)
        {
            _logger.LogError(ex);
        }
    }
}
