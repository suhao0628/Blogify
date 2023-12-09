using System.ComponentModel.DataAnnotations;

namespace Blogify_API.Entities;

public class Author
{
    [Key]
    public Guid UserId { get; set; }

    public int Posts { get; set; }

    public int Likes { get; set; }
}