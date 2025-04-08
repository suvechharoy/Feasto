using Feasto.Services.AuthAPI.Models;

namespace Feasto.Services.AuthAPI.Service.IService;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
}