using System.Text;
using Feasto.Services.EmailAPI.Data;
using Feasto.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Feasto.Services.EmailAPI.Services;

public class EmailService : IEmailService
{
    private DbContextOptions<AppDbContext> _dbOptions;
    public EmailService(DbContextOptions<AppDbContext> dbOptions)
    {
        _dbOptions = dbOptions;
    }
    
    public async Task EmailCartAndLog(CartDTO cartDTO)
    {
        StringBuilder message = new StringBuilder();

        message.AppendLine("<br/>Cart Email Requested ");
        message.AppendLine("<br/>Total "+ cartDTO.CartHeader.CartTotal);
        message.Append("<br/>");
        message.Append("<ul>");
        foreach (var item in cartDTO.CartDetails)
        {
            message.Append("<li>");
            message.Append(item.Product.Name + " x " + item.Product.Count);
            message.Append("</li>");
        }
        message.Append("</ul>");
        await LogAndEmail(message.ToString(), cartDTO.CartHeader.Email);
    }

    public async Task RegisterUserEmailAndLog(string email)
    {
        string message = "User Registration Successful. <br/> Email : " + email;
        await LogAndEmail(message, "suvechharoy06@gmail.com"); //provide an admin email
    }

    private async Task<bool> LogAndEmail(string message, string email)
    {
        try
        {
            EmailLogger emailLog = new()
            {
                Email = email,
                Message = message,
                EmailSent = DateTime.UtcNow
            };
            await using var _db = new AppDbContext(_dbOptions);
            _db.EmailLoggers.AddAsync(emailLog);
            await _db.SaveChangesAsync();   
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}