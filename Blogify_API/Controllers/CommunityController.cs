using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
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
        /// Create a post in a specified community
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
