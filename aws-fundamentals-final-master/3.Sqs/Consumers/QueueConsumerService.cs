using Amazon.SQS;
using Amazon.SQS.Model;
using Consumers.Messages;
using Customers.Api.Messaging;
using MediatR;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Consumers
{
    public class QueueConsumerService : BackgroundService
    {
        private readonly IAmazonSQS _sqs;
        private readonly IOptions<QueueSettings> _settings;
        private readonly IMediator _mediator;
        private readonly ILogger<QueueConsumerService> _logger;

        public QueueConsumerService(IAmazonSQS sqs, IOptions<QueueSettings> settings, IMediator mediator)
        {
            _sqs = sqs;
            _settings = settings;
            _mediator = mediator;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetQueueUrlResponse queueUrlReponse =  await _sqs.GetQueueUrlAsync(_settings.Value.Name, stoppingToken);

            var receivedMessageRequest = new ReceiveMessageRequest()
            {
                QueueUrl = queueUrlReponse.QueueUrl,
                AttributeNames = new List<string> { "All" },
                MessageAttributeNames = new List<string> { "All" },
                MaxNumberOfMessages = 1
            };

            while(!stoppingToken.IsCancellationRequested)
            {
                var response = await _sqs.ReceiveMessageAsync(receivedMessageRequest, stoppingToken);

                foreach(var message in response.Messages)
                {
                    string? messageType = message.MessageAttributes["MessageType"].StringValue;

                    /*  switch(messageType)
                      {
                          case nameof(CustomerCreated):
                              break;
                          case nameof(CustomerUpdated):
                              break;
                          case nameof(CustomerDeleted):
                              break;
                      }*/

                    var type = Type.GetType($"Consumers.Messages.{messageType}");
                    if (messageType == null)
                    {
                        _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    }

                    var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;
                    
                    try
                    {
                        await _mediator.Send(typedMessage, stoppingToken);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex,"Message failed during processing");
                        continue;
                    }
                    await _sqs.DeleteMessageAsync(queueUrlReponse.QueueUrl, message.ReceiptHandle, stoppingToken);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
