using Blogify_API.Dtos.Community;

namespace Blogify_API.Services.IServices
{
    public interface ICommunityService
    {
        Task<List<CommunityDto>> GetCommunities();
        Task<CommunityFullDto> GetCommunity(Guid communityId);
    }
}
