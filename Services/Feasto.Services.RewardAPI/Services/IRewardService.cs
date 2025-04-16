using Feasto.Services.RewardAPI.Message;

namespace Feasto.Services.RewardAPI.Services;

public interface IRewardService
{
    Task UpdateRewards(RewardsMessage rewardsMessage);
}