using AutoMapper;
using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Dtos.Community;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
using Blogify_API.Entities.Enums;
using Blogify_API.Exceptions;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

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


        public async Task<Guid> CreatePostInCommunity(PostCreateDto postCreateDto, Guid userId, Guid communityId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                       ?? throw new NotFoundException(new Response
                       {
                           Status = "Error",
                           Message = "User not found."
                       });

            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId) ?? throw new NotFoundException(new Response
            {
                Status = "Error",
                Message = $"Community with Guid={communityId} not found."
            });


            var administrator = await _context.CommunityUsers.FirstOrDefaultAsync(cu => cu.CommunityId == communityId && cu.UserId == userId && cu.Role == CommunityRole.Administrator)

             ?? throw new NotFoundException(new Response
             {
                 Status = "Error",
                 Message = "Access denied"
             });

            var tags = new List<Tag>();
            if (!postCreateDto.Tags.IsNullOrEmpty())
            {
                foreach (var tagGuid in postCreateDto.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == tagGuid)
                    ?? throw new NotFoundException(new Response
                    {
                        Status = "Error",
                        Message = $"Tag with Guid={tagGuid} not found."
                    });
                    tags.Add(tag);
                }
            }

            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.UtcNow,
                Title = postCreateDto.Title,
                Description = postCreateDto.Description,
                ReadingTime = postCreateDto.ReadingTime,
                Image = postCreateDto.Image,
                AuthorId = user.Id,
                Author = user.FullName,
                CommunityId = community.Id,
                CommunityName = community.Name,
                Likes = 0,
                CommentsCount = 0,
                Tags = tags
            };

            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();
            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(author => author.UserId == userId);

            if (existingAuthor == null)
            {
                var newAuthor = new Author
                {
                    UserId = userId,
                    Likes = 0,
                    Posts = 1
                };
                await _context.Authors.AddAsync(newAuthor);
                await _context.SaveChangesAsync();
            }
            else
            {
                existingAuthor.Posts++;
            }


            return newPost.Id;
        }

        public async Task SubscribeUserToCommunity(Guid userId, Guid communityId)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId) 
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            var communityUser = await _context.CommunityUsers.FirstOrDefaultAsync(cu => cu.CommunityId == communityId && cu.UserId == userId);
            if (communityUser != null)
            {
                throw new BadRequestException(
                new Response
                {
                    Status = "Error",
                    Message = "User is already a member of this community."
                });
            }

            var newSubscriber = new CommunityUser
            {
                CommunityId = communityId,
                UserId = userId,
                Role = CommunityRole.Subscriber
            };

            await _context.CommunityUsers.AddAsync(newSubscriber);
            community.SubscribersCount++;
            await _context.SaveChangesAsync();
        }

        public async Task UnsubscribeUserFromCommunity(Guid userId, Guid communityId)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            var subscriber = await _context.CommunityUsers.FirstOrDefaultAsync(cu =>cu.CommunityId == communityId&& cu.UserId == userId&& cu.Role == CommunityRole.Subscriber)
                ?? throw new BadRequestException(
                new Response
                {
                    Status = "Error",
                    Message = "User is not a subscriber of this community."
                });

            _context.CommunityUsers.Remove(subscriber);
            community.SubscribersCount--;
            await _context.SaveChangesAsync();
        }

    }
}
