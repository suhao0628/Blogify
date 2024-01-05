using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos.Comment
{
    public class CommentCreateDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(1000)]
        public string Content { get; set; }

        public Guid? ParentId { get; set; }
    }
}
