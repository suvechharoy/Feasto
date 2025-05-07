using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Feasto.Services.OrderAPI.RabbitMQSender;

public class RabbitMQOrderMessageSender : IRabbitMQOrderMessageSender
{
    private IConnection _connection;
    private readonly string _HostName;
    private readonly string _Username;
    private readonly string _Password;
    private const string OrderCreated_RewardsUpdateQueue = "RewardsUpdateQueue";
    private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";

    public RabbitMQOrderMessageSender()
    {
        _HostName = "localhost";
        _Username = "guest";
        _Password = "guest";
    }
    
    public void SendMessage(object message, string exchangeName)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false);
            channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
            channel.QueueDeclare(OrderCreated_RewardsUpdateQueue, false, false, false, null);
            
            channel.QueueBind(OrderCreated_EmailUpdateQueue, exchangeName, "EmailUpdate");
            channel.QueueBind(OrderCreated_RewardsUpdateQueue, exchangeName, "RewardsUpdate");

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: exchangeName, "EmailUpdate", null, body: body);
            channel.BasicPublish(exchange: exchangeName, "RewardsUpdate", null, body: body);
        }
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _HostName, 
                UserName = _Username,
                Password = _Password
            };
        
            _connection = factory.CreateConnection();
        }
        catch (Exception e)
        {
            
        }
    }

    private bool ConnectionExists()
    {
        if (_connection != null)
        {
            return true;
        }
        CreateConnection();
        return true;
    }
}