using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Attributes
{
    public class BirthDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                if (birthDate > DateTime.Today)
                {
                    return new ValidationResult("Birth date can't be later than today");
                }
            }
            return ValidationResult.Success;
        }
    }
}
