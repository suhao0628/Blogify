using Blogify_API.Dtos;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
using Blogify_API.Entities.Enums;
using Blogify_API.Services;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogify_API.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PostPagedListDto>> GetAllAvailablePosts([FromQuery] List<Guid>? tags, [FromQuery] string? author, [FromQuery] int? min, [FromQuery] int? max, [FromQuery] PostSorting? sorting, [FromQuery] bool onlyMyCommunities = false, [FromQuery] int page = 1, [FromQuery] int size = 5)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(
                await _postService.GetAvailablePosts(userId, tags, author, min, max, sorting, onlyMyCommunities, page, size)
            );
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePost(PostCreateDto postCreateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _postService.CreatePost(postCreateDto, userId));
        }
        [AllowAnonymous]
        [HttpGet("{postId}")]
        public async Task<ActionResult<PostFullDto>> GetPostDetails(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _postService.GetPostDetails(postId, userId));
        }

        [Authorize]
        [HttpPost("{postId}/like")]
        public async Task<IActionResult> AddLike(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _postService.AddLike(postId, userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{postId}/like")]
        public async Task<IActionResult> DeleteLike(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _postService.DeleteLike(postId, userId);
            return Ok();
        }

    }
}
