using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities.Enums;

namespace Blogify_API.Services.IServices
{
    public interface ICommunityService
    {
        Task<List<CommunityDto>> GetCommunities();
        Task<CommunityFullDto> GetCommunity(Guid communityId);
        Task<Guid> CreateCommunity(CommunityCreateDto createCommunityDto, Guid userId);
        Task UpdateCommunity(Guid communityId, Guid userId, CommunityUpdateDto communityUpdateDto);
        Task DeleteCommunity(Guid communityId, Guid userId);
        Task<PostPagedListDto> GetPostsInCommunity(Guid? userId, Guid communityId, List<Guid>? tags, PostSorting? sorting, int page, int size);
        Task<Guid> CreatePostInCommunity(PostCreateDto postCreateDto, Guid userId, Guid communityId);
        Task AssignAdminToSubscriber(Guid adminId, Guid communityId, Guid userId);
        Task<CommunityRoleDto> GetUserRoleInCommunity(Guid communityId, Guid userId);
        Task<List<CommunityUserDto>> GetUserCommunities(Guid userId);

        Task SubscribeUserToCommunity(Guid userId, Guid communityId);
        Task UnsubscribeUserFromCommunity(Guid userId, Guid communityId);
    }
}
