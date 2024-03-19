using Amazon.SQS;
using Amazon.SQS.Model;
using SqsPublisher;
using System.Text.Json;


//Creating a instance of the AWS SQS client, since we have already logged in using the AWS CLI it was not neccesary to pass our credentials
AmazonSQSClient amazonSQSClient = new();

CustomerCreated customerCreated = new() {
    Id = Guid.NewGuid(),
    Email = "test@example",
    FullName = "Test User",
    GitHubUsername = "GitHub Test User",
    DateOfBirth = new(2001,11,6) };

//Getting the URL of the queue called "net_demo"
var queueUrl = await amazonSQSClient.GetQueueUrlAsync("net_demo");

//Building our message request to AWS SQS with our Url, our MessageBody that will contain JSON, and, additionally, passing it some attributes as "metadata"
SendMessageRequest messageRequest = new() { 
    QueueUrl = queueUrl.QueueUrl, 
    MessageBody = JsonSerializer.Serialize(customerCreated)/*Serializing our CustomerCreated instance into JSON*/,
    MessageAttributes = new Dictionary<string, MessageAttributeValue>() { 
        { "Message", new MessageAttributeValue { DataType = "String", StringValue = nameof(CustomerCreated) } 
        } 
    }
};

//Sending our request with the message request we just created
SendMessageResponse? response = await amazonSQSClient.SendMessageAsync(messageRequest);