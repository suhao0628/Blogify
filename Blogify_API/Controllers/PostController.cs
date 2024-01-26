using Blogify_API.Dtos;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
using Blogify_API.Entities.Enums;
using Blogify_API.Services;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        /// <summary>
        /// Get a list of available posts
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="author"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="sorting"></param>
        /// <param name="onlyMyCommunities"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PostPagedListDto>> GetAllAvailablePosts([FromQuery] List<Guid>? tags, [FromQuery] string? author, [FromQuery][Range(0, int.MaxValue)] int? min, [FromQuery][Range(0, int.MaxValue)] int? max, [FromQuery] PostSorting? sorting, [FromQuery] bool onlyMyCommunities = false, [FromQuery][Range(1, int.MaxValue)] int page = 1, [FromQuery][Range(1, int.MaxValue)] int size = 5)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(
                await _postService.GetAvailablePosts(userId, tags, author, min, max, sorting, onlyMyCommunities, page, size)
            );
        }
        /// <summary>
        /// Create a personal user post
        /// </summary>
        /// <param name="postCreateDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePost(PostCreateDto postCreateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _postService.CreatePost(postCreateDto, userId));
        }
        /// <summary>
        /// Get information about concrete post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{postId}")]
        public async Task<ActionResult<PostFullDto>> GetPostDetails(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            return Ok(await _postService.GetPostDetails(postId, userId));
        }
        /// <summary>
        /// Add Like to concrete post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{postId}/like")]
        public async Task<IActionResult> AddLike(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);
            await _postService.AddLike(postId, userId);
            return Ok();
        }
        /// <summary>
        /// Delete like from concrete post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
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
