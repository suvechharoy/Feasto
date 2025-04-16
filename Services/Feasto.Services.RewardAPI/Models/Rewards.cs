namespace Feasto.Services.RewardAPI.Models;

public class Rewards
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime RewardsDate { get; set; }
    public int RewardsActivity { get; set; } // reward points received i.e., basically the order total
    public int OrderId { get; set; }
}