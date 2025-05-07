using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Feasto.Services.ShoppingCartAPI.RabbitMQSender;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    private IConnection _connection;
    private readonly string _HostName;
    private readonly string _Username;
    private readonly string _Password;

    public RabbitMQCartMessageSender()
    {
        _HostName = "localhost";
        _Username = "guest";
        _Password = "guest";
    }
    
    public void SendMessage(object message, string queueName)
    {
        if (ConnectionExists())
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queueName, false, false, false, null);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: queueName, null, body: body);
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