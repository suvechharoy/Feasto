using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface IBaseService
{
    Task<ResponseDTO?> SendAsync(RequestDTO requestDto, bool withBearer = true);
}