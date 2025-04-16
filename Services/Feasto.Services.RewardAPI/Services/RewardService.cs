using Feasto.Services.RewardAPI.Data;
using Feasto.Services.RewardAPI.Message;
using Feasto.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Feasto.Services.RewardAPI.Services;

public class RewardService : IRewardService
{
    private DbContextOptions<AppDbContext> _dbOptions;
    public RewardService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }

    public async Task UpdateRewards(RewardsMessage rewardsMessage)
    {
        try
        {
            Rewards rewards = new Rewards()
            {
                OrderId = rewardsMessage.OrderId,
                RewardsActivity = rewardsMessage.RewardsActivity,
                UserId = rewardsMessage.UserId,
                RewardsDate = DateTime.UtcNow
            };
            await using var context = new AppDbContext(_dbOptions);
            await context.Rewards.AddAsync(rewards);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}