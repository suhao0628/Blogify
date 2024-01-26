using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos.Post
{
    public class PostCreateDto
    {
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
        public List<Guid> Tags { get; set; } 
    }
}
