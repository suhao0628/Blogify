using Blogify_API.Dtos.Author;

namespace Blogify_API.Services.IServices
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAuthors();
    }
}
