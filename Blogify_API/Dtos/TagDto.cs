using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos
{
    public class TagDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        [Required]
        [MinLength(1)]
        public string Name { get; set; }
    }
}
