﻿using Blogify_API.Dtos.Tag;
using Blogify_API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blogify_API.Controllers
{
    [Route("api/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }
        /// <summary>
        /// Get tag list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<TagDto>>> GetTags()
        {
            return Ok(await _tagService.GetTags());
        }

    }
}
