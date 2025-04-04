namespace Feasto.Services.AuthAPI.Models.DTO;

public class LoginResponseDTO
{
    public UserDTO User { get; set; } // Once a user logs in, we'll send the user details back as login response.
    public string Token { get; set; }
}