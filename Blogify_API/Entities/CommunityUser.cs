using Blogify_API.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogify_API.Entities
{
    public class CommunityUser
    {
        [Key] 
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid CommunityId { get; set; }
        [Required]
        public CommunityRole Role { get; set; }
    }
}
