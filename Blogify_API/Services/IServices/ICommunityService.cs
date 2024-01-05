using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Post;

namespace Blogify_API.Services.IServices
{
    public interface ICommunityService
    {
        Task<List<CommunityDto>> GetCommunities();
        Task<CommunityFullDto> GetCommunity(Guid communityId);
        Task<Guid> CreatePostInCommunity(PostCreateDto postCreateDto, Guid userId, Guid communityId);

        Task SubscribeUserToCommunity(Guid userId, Guid communityId);
        Task UnsubscribeUserFromCommunity(Guid userId, Guid communityId);
    }
}
