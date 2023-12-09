using Blogify_API.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blogify_API.Entities;

public class Author
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public int Posts { get; set; }

    public int Likes { get; set; }
}