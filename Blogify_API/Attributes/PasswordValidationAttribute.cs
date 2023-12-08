using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Blogify_API.Attributes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
            {
                return new ValidationResult("Password must contain at least one digit");
            }

            // Password should contain at least one digit, one letter, and one special character
            var hasDigit = new Regex(@"[0-9]").IsMatch(password);
            if (!hasDigit)
            {
                return new ValidationResult("Password must contain at least one digit");
            }
            var hasLetter = new Regex(@"[a-zA-Z]").IsMatch(password);
            if (!hasLetter)
            {
                return new ValidationResult("Password must contain at least one letter");
            }
            var hasSpecialChar = new Regex(@"\W|_").IsMatch(password);
            if (!hasSpecialChar)
            {
                return new ValidationResult("Password must contain at least one special char");
            }

            return ValidationResult.Success;
        }
    }
}
