using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface IAuthService
{
    Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDto);
    Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDto);
    Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDto);
}