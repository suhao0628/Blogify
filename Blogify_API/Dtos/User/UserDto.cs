using Blogify_API.Attributes;
using Blogify_API.Dtos.Enums;
using System.ComponentModel.DataAnnotations;
namespace Blogify_API.Dtos.User
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
        [Required]
        [MinLength(1)]
        public string FullName { get; set; }

        [BirthDateValidation]
        public DateTime? BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [RegularExpression(@"^\+7\d{3}\d{3}\d{2}\d{2}$", ErrorMessage = "Please enter a valid phone number in the format +7(xxx)xxx-xx-xx.")]
        public string? PhoneNumber { get; set; }
    }
}
