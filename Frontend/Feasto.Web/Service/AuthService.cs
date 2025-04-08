using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;

    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    public async Task<ResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = loginRequestDto,
            Url = StaticDetails.AuthAPIBase + "/api/auth/login"
        }, withBearer: false);
    }

    public async Task<ResponseDTO?> RegisterAsync(RegistrationRequestDTO registrationRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = registrationRequestDto,
            Url = StaticDetails.AuthAPIBase + "/api/auth/register"
        });
    }

    public async Task<ResponseDTO?> AssignRoleAsync(RegistrationRequestDTO registrationRequestDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = registrationRequestDto,
            Url = StaticDetails.AuthAPIBase + "/api/auth/AssignRole"
        });
    }
}