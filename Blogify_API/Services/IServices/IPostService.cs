using Blogify_API.Dtos.Post;
using Blogify_API.Entities.Enums;

namespace Blogify_API.Services.IServices
{
    public interface IPostService
    {
        Task<Guid> CreatePost(PostCreateDto postCreateDto, Guid userId);
        Task<PostFullDto> GetPostDetails(Guid postId, Guid? userId);
    }
}
