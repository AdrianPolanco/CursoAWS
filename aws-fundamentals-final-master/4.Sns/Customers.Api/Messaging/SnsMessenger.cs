using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly IOptions<TopicSettings> _topicSettings;
    private string? _topicUrl;

    // IOptions<TOptions> es la interfaz proporcionada por ASP.NET Core para implementar el patron de diseño Options, que es una manera de proporcionar
    //Opciones del appsettings.json de forma fuertemente tipada, TOptions es la clase que debe representar el key-value pair que queremos obtener del appsettings.json
    public SnsMessenger(IAmazonSimpleNotificationService sns, IOptions<TopicSettings> topicSettings)
    {
        _sns = sns;
        _topicSettings = topicSettings;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message)
    {
         var topicUrl = await GetTopicUrlAsync();

        var sendMessageRequest = new PublishRequest
        {
            TopicArn = topicUrl,
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await _sns.PublishAsync(sendMessageRequest);
    }

    private async ValueTask<string> GetTopicUrlAsync()
    {
        if (_topicUrl is not null)
        {
            return _topicUrl;
        }

        var queueUrlResponse = await _sns.FindTopicAsync(_topicSettings.Value.Name);
        _topicUrl = queueUrlResponse.TopicArn;
        return _topicUrl;
    }
}
