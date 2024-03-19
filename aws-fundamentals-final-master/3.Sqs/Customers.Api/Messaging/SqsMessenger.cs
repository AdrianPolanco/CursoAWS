using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Customers.Api.Messaging
{
    public class SqsMessenger: ISqsMessenger
    {
        private readonly IAmazonSQS _amazonSQS;
        private readonly IOptions<QueueSettings> _queueOptions;
        private string _queueUrl;

        public SqsMessenger(IAmazonSQS amazonSQS, IOptions<QueueSettings> queueOptions)
        {
            _amazonSQS = amazonSQS;
            _queueOptions = queueOptions;
        }

        public async Task<SendMessageResponse> SendMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
        {
            var queueUrlResponse = await GetQueueUrlAsync();

            var sendMessageRequest = new SendMessageRequest()
            {
                QueueUrl = queueUrlResponse,
                MessageBody = JsonSerializer.Serialize(message),
                MessageAttributes = new Dictionary<string, MessageAttributeValue> {
                    {
                        "MessageType",
                        new()
                        {
                            DataType = "String",
                            StringValue = typeof(TMessage).Name
                        }
                    } 
                }
            };

            return await _amazonSQS.SendMessageAsync(sendMessageRequest, cancellationToken);
        }

        private async Task<string> GetQueueUrlAsync()
        {
            if (_queueUrl is not null) return _queueUrl;

            var getQueueUrlResponse = await _amazonSQS.GetQueueUrlAsync(_queueOptions.Value.Name);

            _queueUrl = getQueueUrlResponse.QueueUrl;

            return _queueUrl;
        }
    }
}
