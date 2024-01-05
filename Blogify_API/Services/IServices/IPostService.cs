using Blogify_API.Dtos.Post;
using Blogify_API.Entities.Enums;

namespace Blogify_API.Services.IServices
{
    public interface IPostService
    {
        Task<PostPagedListDto> GetAvailablePosts(Guid? userId, List<Guid>? tags, string? author, int? min, int? max, PostSorting? sorting, bool onlyMyCommunities, int page, int size)


;        Task<Guid> CreatePost(PostCreateDto postCreateDto, Guid userId);
        Task<PostFullDto> GetPostDetails(Guid postId, Guid? userId);
        Task AddLike(Guid postId, Guid userId);
        Task DeleteLike(Guid postId, Guid userId);
    }
}
