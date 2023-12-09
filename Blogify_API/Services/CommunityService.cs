using AutoMapper;
using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Dtos.Community;
using Blogify_API.Entities.Enums;
using Blogify_API.Exceptions;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Blogify_API.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CommunityService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CommunityDto>> GetCommunities()
        {
            var communities = await _context.Communities.ToListAsync();
            var communityDtos = new List<CommunityDto>();
            foreach (var community in communities)
            {
                var communityDto = _mapper.Map<CommunityDto>(community);
                communityDtos.Add(communityDto);
            }
            return communityDtos;
        }
        public async Task<CommunityFullDto> GetCommunity(Guid communityId)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId) ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            var admins = community.CommunityUsers.Where(member => member.Role == CommunityRole.Administrator).ToList();

            var adminUsers = new List<UserDto>();
            foreach (var admin in admins)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id== admin.UserId) ??
                           throw new NotFoundException(new Response { Status = "Error", Message = "User not found." });
                var userDto = _mapper.Map<UserDto>(user);
                adminUsers.Add(userDto);
            }

            var communityFullDto = new CommunityFullDto()
            {
                Id = community.Id,
                CreateTime = community.CreatedTime,
                Name = community.Name,
                Description = community.Description,
                IsClosed = community.IsClosed,
                SubscribersCount = community.SubscribersCount,
                Administrators = adminUsers
            };
            return communityFullDto;
        }
    }
}
