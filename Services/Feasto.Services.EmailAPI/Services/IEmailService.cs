using Feasto.Services.EmailAPI.Message;
using Feasto.Services.EmailAPI.Models;

namespace Feasto.Services.EmailAPI.Services;

public interface IEmailService
{
    Task EmailCartAndLog(CartDTO cartDTO);
    Task RegisterUserEmailAndLog(string email);
    Task LogOrderPlaced(RewardsMessage rewardsMessage);
}