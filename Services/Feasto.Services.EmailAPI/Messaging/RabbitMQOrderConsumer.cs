using System.Text;
using Feasto.Services.EmailAPI.Message;
using Feasto.Services.EmailAPI.Models;
using Feasto.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Feasto.Services.EmailAPI.Messaging;

public class RabbitMQOrderConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private IConnection _connection;
    private IModel _channel;
    private const string OrderCreated_EmailUpdateQueue = "EmailUpdateQueue";
    private string ExchangeName = "";
    string queueName = "";

    public RabbitMQOrderConsumer(IConfiguration configuration, EmailService emailService)
    {
        _configuration = configuration;
        _emailService = emailService;
        ExchangeName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
        var factory = new ConnectionFactory()
        {
            HostName = "localhost", 
            UserName = "guest",
            Password = "guest"
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(OrderCreated_EmailUpdateQueue, false, false, false, null);
        _channel.QueueBind(OrderCreated_EmailUpdateQueue, ExchangeName, "EmailUpdate");
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            RewardsMessage rewards = JsonConvert.DeserializeObject<RewardsMessage>(content);
            HandleMessage(rewards).GetAwaiter().GetResult();

            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(OrderCreated_EmailUpdateQueue, false, consumer);
        return Task.CompletedTask;
    }

    private async Task HandleMessage(RewardsMessage rewards)
    {
        _emailService.LogOrderPlaced(rewards).GetAwaiter().GetResult();
    }
}