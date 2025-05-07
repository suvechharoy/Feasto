namespace Feasto.Services.ShoppingCartAPI.RabbitMQSender;

public interface IRabbitMQCartMessageSender
{
    void SendMessage(Object message, string queueName); 
}