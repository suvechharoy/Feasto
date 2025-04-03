using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using static Feasto.Web.Utility.StaticDetails;
using Newtonsoft.Json;

namespace Feasto.Web.Service;

public class BaseService : IBaseService // This service is responsible to make calls to all the APIs/Services
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<ResponseDTO?> SendAsync(RequestDTO requestDto)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
        
            //token generation

            message.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage? apiResponse = null; // When we send a request, we'll get back a response. This variable is to hold that response.

            switch (requestDto.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }
        
            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new ResponseDTO() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new ResponseDTO() { IsSuccess = false, Message = "Access Denied" };
                case HttpStatusCode.Unauthorized:
                    return new ResponseDTO() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.InternalServerError:
                    return new ResponseDTO() { IsSuccess = false, Message = "Internal Server Error" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDTO = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                    return apiResponseDTO;
            }
        }
        catch (Exception e)
        {
            var dto = new ResponseDTO()
            {
                IsSuccess = false,
                Message = e.Message.ToString(),
            };
            return dto;
        }
    }
}