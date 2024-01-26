using Blogify_API.Dtos;
using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
using Blogify_API.Entities.Enums;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogify_API.Controllers
{
    [Route("api/community")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }
        /// <summary>
        /// Get community list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<CommunityDto>>> GetCommunities()
        {
            return Ok(await _communityService.GetCommunities());
        }
        /// <summary>
        /// Get information about community
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [HttpGet("{communityId}")]
        public async Task<ActionResult<CommunityFullDto>> GetCommunityInfo(Guid communityId)
        {
            return Ok(await _communityService.GetCommunity(communityId));
        }
        /// <summary>
        /// Get concrete community's posts
        /// </summary>
        /// <param name="communityId">Unique identifier of the community</param>
        /// <param name="tags"> tag list to filter by tags</param>
        /// <param name="sorting">option to sort posts</param>
        /// <param name="page">page number</param>
        /// <param name="size">required number of elements per page</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{communityId}/post")]
        public async Task<ActionResult<PostPagedListDto>> GetPostsInCommunity(Guid communityId,[FromQuery] List<Guid>? tags,[FromQuery] PostSorting? sorting,[FromQuery] int page = 1,[FromQuery] int size = 5)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _communityService.GetPostsInCommunity(userId, communityId, tags, sorting, page, size));
        }

        /// <summary>
        /// Create a post in a concrete community
        /// </summary>
        /// <param name="postCreateDto"></param>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{communityId}/post")]
        public async Task<ActionResult<Guid>> CreatePostInCommunity(PostCreateDto postCreateDto, Guid communityId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _communityService.CreatePostInCommunity(postCreateDto, userId, communityId));
        }
        /// <summary>
        /// Assign admin to user subscribed to the community
        /// </summary>
        /// <param name="communityId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{userId}/admin")]
        public async Task<ActionResult> AssignAdminToSubscriber(Guid communityId, Guid userId)
        {
            var adminId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _communityService.AssignAdminToSubscriber(adminId, communityId, userId);
            return Ok();
        }
        /// <summary>
        /// Get the greatest user's role in the community (or null if the user is not a member of the community)
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{communityId}/role")]
        public async Task<ActionResult<CommunityRoleDto>> GetUserRoleInCommunity(Guid communityId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _communityService.GetUserRoleInCommunity(communityId, userId));
        }
        /// <summary>
        /// Get user's community list (with the greatest user's role in the community)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<List<CommunityUserDto>>> GetUserCommunities()
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _communityService.GetUserCommunities(userId));
        }
        /// <summary>
        /// Subscribe a user to the community
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{communityId}/subscribe")]
        public async Task<IActionResult> SubscribeUserToCommunity(Guid communityId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _communityService.SubscribeUserToCommunity(userId,communityId);
            return Ok();
        }
        /// <summary>
        /// Unsubscribe a user from the community
        /// </summary>
        /// <param name="communityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{communityId}/unsubscribe")]
        public async Task<IActionResult> UnsubscribeUserFromCommunity(Guid communityId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _communityService.UnsubscribeUserFromCommunity(userId, communityId);
            return Ok();
        }
    }
}
