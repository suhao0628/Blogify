using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Dtos.Community
{
    public class CommunityUpdateDto
    {
        [MinLength(1)]
        public string Name { get; set; }
        [MaxLength(225)]
        public string? Description { get; set; }
        public bool IsClosed { get; set; }
    }
}
