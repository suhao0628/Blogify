using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Entities
{
    public class Community
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreatedTime { get; set; }
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public bool IsClosed { get; set; } = false;
        [Required]
        public int SubscribersCount { get; set; } = 0;

        public List<CommunityUser> CommunityUsers { get; set; }
    }
}
