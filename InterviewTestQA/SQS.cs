using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace InterviewTestQA
{
    public class SQS
    {
        private string SQSUrl = "https://sqs.eu-north-1.amazonaws.com/730335407970/sreeni";
        string TestMessage = "Test Message";
        static string IAM_User_AccessKey = "AKIA2UC3B55RPNT2FE4O";
        static string IAM_User_SecretKey = "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";

        static AmazonSQSConfig sqsConfig = new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.EUNorth1
        };
        AmazonSQSClient sqsClient = new AmazonSQSClient(IAM_User_AccessKey, IAM_User_SecretKey, sqsConfig);

        [Fact]
        public async Task Send_Receive_Message()
        {
            var sentRequest = new SendMessageRequest
            {
                QueueUrl = SQSUrl,
                MessageBody = TestMessage
            };
            var sentMessage = await sqsClient.SendMessageAsync(sentRequest);

            if (sentMessage.MessageId != null)
            {
                Assert.NotNull(sentMessage.MessageId);
                Console.WriteLine("Message sent sucessfully with ID " + sentMessage.MessageId);
            }
            else
            {
                Console.WriteLine("No message sent");
            }

            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = SQSUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 10
            };
            var receiveMessage = await sqsClient.ReceiveMessageAsync(receiveMessageRequest);

            if (receiveMessage.Messages.Count == 1)
            {
                var receivedMessage = receiveMessage.Messages[0].Body;
                Assert.Equal(receivedMessage, TestMessage);
                Console.WriteLine("Received Message match the Sent Message " + sentMessage.MessageId + " " + receivedMessage);

                await sqsClient.DeleteMessageAsync(SQSUrl, receiveMessage.Messages[0].ReceiptHandle);
            }
            else
            {
                Console.WriteLine("No message received from the queue.");
            }

        }
    }
}
