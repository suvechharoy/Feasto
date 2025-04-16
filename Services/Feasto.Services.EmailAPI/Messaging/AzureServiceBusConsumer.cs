using System.Text;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Feasto.Services.EmailAPI.Message;
using Feasto.Services.EmailAPI.Models;
using Feasto.Services.EmailAPI.Services;
using Newtonsoft.Json;

namespace Feasto.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailcartQueue;
    private readonly string registerUserQueue;
    private readonly IConfiguration _configuration;
    private readonly EmailService _emailService;
    private readonly string orderCreated_Topic;
    private readonly string orderCreated_Email_Subscription;

    private ServiceBusProcessor _emailOrderPlacedProcessor;
    private ServiceBusProcessor _emailCartProcessor;
    private ServiceBusProcessor _registerUserProcessor;
    
    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _emailService = emailService;
        _configuration = configuration;  
        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        emailcartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
        registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue");
        orderCreated_Topic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
        orderCreated_Email_Subscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Email_Subscription");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _emailCartProcessor = client.CreateProcessor(emailcartQueue);
        _registerUserProcessor = client.CreateProcessor(registerUserQueue);
        _emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailCartProcessor.StartProcessingAsync();
        
        _registerUserProcessor.ProcessMessageAsync += OnUserRegisterRequestReceived;
        _registerUserProcessor.ProcessErrorAsync += ErrorHandler;
        await _registerUserProcessor.StartProcessingAsync();
        
        _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
        _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;
        await _emailOrderPlacedProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _emailCartProcessor.StartProcessingAsync();
        await _emailCartProcessor.DisposeAsync();
        
        await _registerUserProcessor.StartProcessingAsync();
        await _registerUserProcessor.DisposeAsync();
        
        await _emailOrderPlacedProcessor.StartProcessingAsync();
        await _emailOrderPlacedProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs arg)
    {
        //This where you'll receive the message
       
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CartDTO objMsg = JsonConvert.DeserializeObject<CartDTO>(body);
        try
        {
            //try to log email
            await _emailService.EmailCartAndLog(objMsg);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task OnUserRegisterRequestReceived(ProcessMessageEventArgs arg)
    {
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        string email = JsonConvert.DeserializeObject<string>(body);
        try
        {
            //try to log email
            await _emailService.RegisterUserEmailAndLog(email);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs arg)
    {
        //This where you'll receive the message
       
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        RewardsMessage objMsg = JsonConvert.DeserializeObject<RewardsMessage>(body);
        try
        {
            //try to log email
            await _emailService.LogOrderPlaced(objMsg);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}