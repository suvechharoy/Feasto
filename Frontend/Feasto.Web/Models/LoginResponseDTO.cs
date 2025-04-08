namespace Feasto.Web.Models;

public class LoginResponseDTO
{
    public UserDTO User { get; set; } // Once a user logs in, we'll send the user details back as login response.
    public string Token { get; set; }
}