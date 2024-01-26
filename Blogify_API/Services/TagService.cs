using AutoMapper;
using Blogify_API.Datas;
using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Tag;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Blogify_API.Services
{
    public class TagService: ITagService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public TagService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<TagDto>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            var tagDtos = new List<TagDto>();
            foreach (var tag in tags)
            {
                var tagDto = _mapper.Map<TagDto>(tag);
                tagDtos.Add(tagDto);
            }
            return tagDtos;
        }
    }
}
