using Amazon.SQS;
using Amazon.SQS.Model;

AmazonSQSClient client = new AmazonSQSClient();
CancellationTokenSource tokenSource = new CancellationTokenSource();

var queueUrl = await client.GetQueueUrlAsync("net_demo");

string url = queueUrl.QueueUrl;

//Building our receive message request, and specifying we want to get all the atributes and message attributes
ReceiveMessageRequest messageRequest = new()
{
    QueueUrl = url,
    AttributeNames = new List<string>() { "All" },
    MessageAttributeNames = new List<string>() { "All" }
};

while (!tokenSource.IsCancellationRequested)
{
    //Requesting messages to receive them
    ReceiveMessageResponse receivedMessage = await client.ReceiveMessageAsync(messageRequest, tokenSource.Token);
    
    foreach (var message in receivedMessage.Messages)
    {
        //Accesing to the message properties
        Console.WriteLine($"Id: {message.MessageId}");
        Console.WriteLine($"Content: {message.Body}");
        Console.WriteLine($"Content: {message.Attributes}");
        Console.WriteLine($"Content: {message.MessageAttributes}");
    }
}
