using Blogify_API.Dtos.Post;

namespace Blogify_API.Services.IServices
{
    public interface IPostService
    {
        Task<Guid> CreatePost(PostCreateDto postCreateDto, Guid userId);
            }
}
