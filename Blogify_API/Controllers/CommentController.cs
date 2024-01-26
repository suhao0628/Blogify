using Blogify_API.Dtos.Comment;
using Blogify_API.Entities;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace Blogify_API.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        /// <summary>
        /// Get all nested comments
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("{commentId}/tree")]
        public async Task<ActionResult<List<CommentDto>>> GetNestedComments(Guid commentId)
        {
            var comments = await _commentService.GetNestedComments(commentId);
            return Ok(comments);
        }
        /// <summary>
        /// Add a comment to concrete post
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="commentCreateDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("/api/post/{postId}/comment")]
        public async Task<IActionResult> AddComment(Guid postId, CommentCreateDto commentCreateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _commentService.AddComment(postId, userId, commentCreateDto);
            return Ok();
        }
        /// <summary>
        /// Edit concrete comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="commentUpdateDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(Guid commentId, CommentUpdateDto commentUpdateDto)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _commentService.UpdateComment(commentId, userId, commentUpdateDto);
            return Ok();
        }
        /// <summary>
        /// Delete concrete comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            var userId = Guid.Parse(User.Claims.Where(w => w.Type == "UserId").First().Value);

            await _commentService.DeleteComment(commentId, userId);
            return Ok();
        }
    }
}
