using Blogify_API.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos.Community
{
    public class CommunityUserDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid CommunityId { get; set; }
        [Required]
        public CommunityRole Role { get; set; }
    }
}
