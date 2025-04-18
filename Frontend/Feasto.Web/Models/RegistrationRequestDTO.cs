using System.ComponentModel.DataAnnotations;

namespace Feasto.Web.Models;

public class RegistrationRequestDTO
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string? Role { get; set; }
}