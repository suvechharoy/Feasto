using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace Feasto.Services.ShoppingCartAPI.Utility;

public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) 
    {
        var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token"); //fetch token
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); //add token to header
        return await base.SendAsync(request, cancellationToken);
    }
}