namespace Feasto.Services.ProductAPI.Models.DTO;

public class ResponseDTO
{
    public object? Result { get; set; } //final response data
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = "";
}