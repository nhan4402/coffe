using System.ComponentModel.DataAnnotations;

namespace Shopping_Coffee.Repository.Validation
{
    public class FileExtensionAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file) { 
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "png", "jpeg","PNG" };
                bool result = extensions.Any(x=>extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed extension are jpg, png, jpeg");

                }
            }
            return ValidationResult.Success;
        }
    }
}
