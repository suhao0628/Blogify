using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(1000)]
        public string Title { get; set; }
        [Required]
        [MinLength(5)]
        [MaxLength(5000)]
        public string Description { get; set; }
        [Required]
        public int ReadingTime { get; set; }
        [Url]
        [MaxLength(1000)]
        public string? Image { get; set; }
        public List<Tag> Tags { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        [MinLength(1)]
        public string Author { get; set; }
        public Guid? CommunityId { get; set; }
        public string? CommunityName { get; set; }
        [Required]
        public int Likes { get; set; }
        [Required]
        public bool HasLike { get; set; }
        [Required]
        public int CommentsCount { get; set; }
    }
}
