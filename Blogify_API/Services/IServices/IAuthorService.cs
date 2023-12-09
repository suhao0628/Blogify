using Blogify_API.Dtos;

namespace Blogify_API.Services.IServices
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAuthors();
    }
}
