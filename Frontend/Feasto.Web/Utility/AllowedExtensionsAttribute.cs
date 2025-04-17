using System.ComponentModel.DataAnnotations;

namespace Feasto.Web.Utility;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;
    
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _allowedExtensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) //add custom validation with data annotation
    {
        var file = value as IFormFile;
        
        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_allowedExtensions.Contains(extension.ToLower()))
            {
                return new ValidationResult("This photo extension is not allowed!");
            }
        }

        return ValidationResult.Success;
    }
}