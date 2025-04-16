using System.Text;
using Azure.Messaging.ServiceBus;
using Feasto.Services.RewardAPI.Message;
using Feasto.Services.RewardAPI.Services;
using Newtonsoft.Json;

namespace Feasto.Services.RewardAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string orderCreatedTopic; // topic name
    private readonly string orderCreatedRewardSubscription; // subscription
    private readonly IConfiguration _configuration;
    private readonly RewardService _rewardService;
    
    private ServiceBusProcessor _rewardProcessor;
    
    public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
    {
        _rewardService = rewardService;
        _configuration = configuration;  
        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
        orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);
    }

    public async Task Start()
    {
        _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
        _rewardProcessor.ProcessErrorAsync += ErrorHandler;

        await _rewardProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _rewardProcessor.StartProcessingAsync();
        await _rewardProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs arg)
    {
        //This where you'll receive the message
       
        var message = arg.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        RewardsMessage objMsg = JsonConvert.DeserializeObject<RewardsMessage>(body);
        try
        {
            //try to log email
            await _rewardService.UpdateRewards(objMsg);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}