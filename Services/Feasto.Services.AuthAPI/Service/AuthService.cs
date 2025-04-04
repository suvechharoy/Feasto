using Feasto.Services.AuthAPI.Data;
using Feasto.Services.AuthAPI.Models;
using Feasto.Services.AuthAPI.Models.DTO;
using Feasto.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Feasto.Services.AuthAPI.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    public readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<string> Register(RegistrationRequestDTO registrationRequestDto)
    {
        ApplicationUser user = new ApplicationUser()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name,
            PhoneNumber = registrationRequestDto.PhoneNumber,
        };
        try
        {
            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded) 
            {
                var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                UserDTO userDto = new UserDTO()
                {
                    ID = userToReturn.Id,
                    Email = userToReturn.Email,
                    Name = userToReturn.Name,
                    PhoneNumber = userToReturn.PhoneNumber
                };
                return "";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception e)
        {
            
        }
        return "Error Encountered";
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
        if (user == null || isValid == false)
        {
            return new LoginResponseDTO()
            {
                User = null,
                Token = "",
            };
        }
        
        //If user is found, generate JWT Token
        var token = _jwtTokenGenerator.GenerateToken(user);
        
        UserDTO userDto = new UserDTO()
        {
            ID = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        };

        LoginResponseDTO loginResponseDto = new LoginResponseDTO()
        {
            User = userDto,
            Token = token,
        };
        return loginResponseDto;
    }

    public async Task<bool> AssignRole(string email, string role)
    {
        var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        if (user != null)
        {
            if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
            {
                //Create role if it does not exist
                _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
            }
            await _userManager.AddToRoleAsync(user, role);
            return true;
        }
        return false;
    }
}