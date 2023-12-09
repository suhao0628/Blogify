using Blogify_API.Dtos.Community;
using Blogify_API.Services.IServices;
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
        [HttpGet]
        public async Task<ActionResult<List<CommunityDto>>> GetCommunities()
        {
            return Ok(await _communityService.GetCommunities());
        }
        [HttpGet("{communityId}")]
        public async Task<ActionResult<CommunityFullDto>> GetCommunityInfo(Guid communityId)
        {
            return Ok(await _communityService.GetCommunity(communityId));
        }

    }
}
