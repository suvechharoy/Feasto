using System.Text;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace Feasto.MessageBus;

public class MessageBus : IMessageBus
{
    private string connectionString; //primary connection string from azure portal ServiceBus resource

    public async Task PublishMessage(object message, string topic_queue_name)
    {

        await using var client = new ServiceBusClient(connectionString); //create client on service bus
        
        ServiceBusSender sender = client.CreateSender(topic_queue_name); //create a sender 

        var jsonMessage = JsonConvert.SerializeObject(message); //serialize the message
        
        ServiceBusMessage messageToPublish = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString(),
        };
        
        await sender.SendMessageAsync(messageToPublish); //send the message 
        await client.DisposeAsync(); 
    }
}