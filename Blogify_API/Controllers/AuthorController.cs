using Blogify_API.Dtos.Author;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogify_API.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        /// <summary>
        /// Get author list(Users who sent the post)
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<ActionResult<List<AuthorDto>>> GetAuthors()
        {
            return Ok(await _authorService.GetAuthors());
        }
    }
}
