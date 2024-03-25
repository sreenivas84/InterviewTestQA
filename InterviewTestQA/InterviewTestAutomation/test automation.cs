using System;
using Amazon.SQS;
using Lorax.Core.Config;
using Amazon.SQS.Model;
using Xunit;
using System.Collections.Generic;
using Moq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Lorax.Core.Messaging.Model.SQS;
using Lorax.Core.Graph.Model;
using Newtonsoft.Json;
using Lorax.Core.Graph;

namespace Lorax.Core.Messaging.Tests
{
    public class SQSTests
    {
        private const string CreateQueueName = "CreateQueueName";
        private const string QueueDeletedRecentlyExceptionName = "QueueDeletedRecentlyExceptionName";
        private const string QueueNameExistsExceptionName = "QueueNameExistsException";
        private const string DeleteQueueName = "DeleteQueueName";
        private string DeleteQueueURL = $"https://sqs.amazonaws.com/{Constants.AWSAccountNumber}/DeleteQueueName";
        private const string DeleteQueueDoesNotExistName = "DeleteQueueDoesNotExistName";
        private string DeleteQueueDoesNotExistURL = $"https://sqs.amazonaws.com/{Constants.AWSAccountNumber}/DeleteQueueDoesNotExistName";

        private const string SendMessageBatchRequestName = "Request";
        private const string BatchRequestTooLongExceptionName = "BatchRequestTooLongException";
        private const string TooManyEntriesInBatchRequestExceptionName = "TooManyEntriesInBatchRequestException";

        private readonly DateTime DefaultDateTime = new DateTime(2020, 5, 20, 11, 0, 0);

        private IServiceConfig Config;
        private readonly Mock<IDateTimeProvider> DateTimeProvider;
        private readonly Mock<ISecrets> Secrets;
        private readonly Mock<ICloudWatchLogger> Logger;
        private readonly Mock<IAmazonSQS> SQSClient;

        ILoraxSQS LoraxSQS { get; set; }

        public SQSTests()
        {
            Config = new ServiceConfigEnv("lx01dev"); //Or Inst?

            Secrets = new Mock<ISecrets>();

            Logger = new Mock<ICloudWatchLogger>();
            Logger
                .Setup(S => S.LogError(It.IsAny<Exception>())) // For any input parameter to be mocked, us It.IsAny<T>
                .Callback((Exception Ex) =>
                {
                    Debug.WriteLine(Ex.Message);
                })
                .Verifiable(); //For void methods, use Verifiable

            DateTimeProvider = new Mock<IDateTimeProvider>();
            DateTimeProvider.Setup(DT => DT.UtcNow).Returns(DefaultDateTime);

            SQSClient = new Mock<IAmazonSQS>();

            //CreateStream Mocks
            SQSClient
                .Setup(C => C.CreateQueueAsync(It.Is<CreateQueueRequest>(R => R.QueueName.Equals(CreateQueueName)), default))
                .Returns(
                    Task.FromResult(
                        new CreateQueueResponse
                        {
                            HttpStatusCode = System.Net.HttpStatusCode.OK
                        }
                    )
                );
            SQSClient
                .Setup(C => C.CreateQueueAsync(It.Is<CreateQueueRequest>(R => R.QueueName.Equals(QueueDeletedRecentlyExceptionName)), default))
                .Throws(new QueueDeletedRecentlyException("UnitTest"));
            SQSClient
                .Setup(C => C.CreateQueueAsync(It.Is<CreateQueueRequest>(R => R.QueueName.Equals(QueueNameExistsExceptionName)), default))
                .Throws(new QueueNameExistsException("UnitTest"));


            //DeleteStream Mocks
            SQSClient
                .Setup(C => C.DeleteQueueAsync(It.Is<DeleteQueueRequest>(R => R.QueueUrl.Equals(DeleteQueueURL)), default))
                .Returns(Task.FromResult(new DeleteQueueResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                }));
            SQSClient
                .Setup(C => C.DeleteQueueAsync(It.Is<DeleteQueueRequest>(R => R.QueueUrl.Equals(DeleteQueueDoesNotExistURL)), default))
                .Throws(new QueueDoesNotExistException("UnitTest"));


            SQSClient
                .Setup(C => C.DeleteQueueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DeleteQueueResponse { HttpStatusCode = System.Net.HttpStatusCode.OK });
        }

        [Fact]
        public async Task BatchMessages()
        {
            List<SendMessage> Messages = new List<SendMessage>();
            for (int i = 0; i < 10; i++)
            {
                Messages.Add(new SendMessage(i.ToString(), $"Test Message #{i}")
                {
                    MessageDeduplicationId = i.ToString(),
                    MessageGroupId = "SQSTests/Batch-Messages",
                });
            }

            LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration { QueueName = "LoraxOpsQueueTEST", IsFifo = true }, null);

            await foreach (var Result in LoraxSQS.SendMessages(Messages))
            {

            }

        }

        [Fact]
        public async Task ConstructorAndCreateTests()
        {
            //Queue Created OK
            LoraxSQS LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration { QueueName = CreateQueueName }, null);
            await LoraxSQS.CreateSQSQueue();

            //Queue with no name throws a null reference exception
            Assert.Throws<NullReferenceException>(() => LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration(), null));

            //Queue name exists exception returns true
            LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration { QueueName = QueueNameExistsExceptionName }, null);
            await LoraxSQS.CreateSQSQueue();
        }

        [Fact]
        public async Task DeleteLoraxOpsQueue()
        {
            LoraxSQS LoraxSQS;
            LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration { QueueName = DeleteQueueName, IsFifo = false }, null);
            await LoraxSQS.DeleteSQSQueue();

            LoraxSQS = new LoraxSQS(new ServiceConfig(ServiceConfigType.Ops), SQSClient.Object, new SqsConfiguration { QueueName = DeleteQueueDoesNotExistName, IsFifo = false }, null);
            await LoraxSQS.DeleteSQSQueue();
        }

        [Fact]
        public async Task SendMessageBatchAsyncUnitTest()
        {
            SQSClient
                .Setup(c => c.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), default))
                .Returns(Task.FromResult(new SendMessageBatchResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                }));

            LoraxSQS LoraxSQS;
            LoraxSQS = new LoraxSQS(this.Config, SQSClient.Object, new SqsConfiguration { QueueName = SendMessageBatchRequestName }, null);
            SendMessage MessageRequest = new SendMessage("ID", "Message");
            await LoraxSQS.SendMessage(MessageRequest);
        }

        [Fact]
        public void DoesSerializationIgnoreNulls()
        {
            var ExampleObject = new IncomingFile { S3Key = "Test S3 Key", Name = "Yes Name" };
            var SerializedIgnoreNull = JsonConvert.SerializeObject(ExampleObject, new JsonSerializerSettings { Formatting = Formatting.None, NullValueHandling = NullValueHandling.Ignore });
            SendMessage TestSendMessageIgnoresNulls = new SendMessage("TestMessage", ExampleObject);
            Assert.Equal(SerializedIgnoreNull, TestSendMessageIgnoresNulls.MessageBody);
        }
    }
}
