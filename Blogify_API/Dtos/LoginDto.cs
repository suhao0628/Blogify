using Blogify_API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos
{
    public class LoginDto
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
}
