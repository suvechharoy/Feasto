using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public TokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    public void SetToken(string token)
    {
        _contextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.TokenCookie, token);
    }

    public string? GetToken()
    {
        string? token = null;
        bool? hasToken = _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(StaticDetails.TokenCookie, out token);
        return hasToken is true ? token : null;
    }

    public void ClearToken()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.TokenCookie);
    }
}