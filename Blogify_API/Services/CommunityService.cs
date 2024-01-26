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
using System.Net.WebSockets;

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
            var community = await _context.Communities.Include(c=>c.CommunityUsers).FirstOrDefaultAsync(c => c.Id == communityId) ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            var admins = community.CommunityUsers.Where(cu => cu.Role == CommunityRole.Administrator).ToList();

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

        public async Task<PostPagedListDto> GetPostsInCommunity(Guid? userId, Guid communityId, List<Guid>? tags, PostSorting? sorting, int page, int size)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            if (community.IsClosed && (userId == null || await _context.CommunityUsers.FirstOrDefaultAsync(cm => cm.CommunityId == communityId && cm.UserId == userId) == null))
            {
                throw new ForbiddenException(new Response
                {
                    Status = "Error",
                    Message = "This community is closed."
                });

            }

            IQueryable<Post> communityPostsQueryable = _context.Posts.Where(post => post.CommunityId == community.Id).Include(post => post.Tags).Include(post => post.LikeLists);

            var postsCount = communityPostsQueryable.Count();
            var paginationCount =
                !communityPostsQueryable.IsNullOrEmpty() ? (int)Math.Ceiling((double)postsCount / size) : 0;

            if (page < 1 || (paginationCount != 0 && page > paginationCount))
            {
                throw new BadRequestException(new Response
                {
                    Status = "Error",
                    Message = "Invalid value for attribute page."
                });
            }

            var pagination = new PageInfoModel
            {
                Size = size,
                Count = paginationCount,
                Current = page
            };

            if (!tags.IsNullOrEmpty())
            {
                foreach (var guid in tags)
                {
                    if (await _context.Tags.FirstOrDefaultAsync(t => t.Id == guid) == null)
                    {
                        throw new NotFoundException(new Response
                        {
                            Status = "Error",
                            Message = $"Tag with Guid={guid} not found."
                        });
                    }
                }
                communityPostsQueryable = communityPostsQueryable.ToList().Where(post => post.Tags.Select(tag => tag.Id).Intersect(tags).Count() == tags.Count).AsQueryable();
            }

            if (sorting != null)
            {
                communityPostsQueryable = GetSortedPosts(communityPostsQueryable, (PostSorting)sorting);
            }

            var posts = communityPostsQueryable.Skip((pagination.Current - 1) * pagination.Size).Take(pagination.Size).ToList();

            User? user = null;
            if (userId != null)
            {
               user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                       ?? throw new NotFoundException(new Response
                       {
                           Status = "Error",
                           Message = "User not found."
                       });
            }

            var postsDto = posts.Select(post =>
            {
                var hasLike = false;
                if (user != null)
                {
                    hasLike = post.LikeLists.Any(liked => liked.UserId == user.Id);
                }

                var tagDtos = post.Tags
                    .Select(tag => _mapper.Map<TagDto>(tag))
                    .ToList();

                var postDto = new PostDto
                {
                    Id = post.Id,
                    CreateTime = post.CreateTime,
                    Title = post.Title,
                    Description = post.Description,
                    ReadingTime = post.ReadingTime,
                    Image = post.Image,
                    AuthorId = post.AuthorId,
                    Author = post.Author,
                    CommunityId = post.CommunityId,
                    CommunityName = post.CommunityName,
                    Likes = post.Likes,
                    HasLike = hasLike,
                    CommentsCount = post.CommentsCount,
                    Tags = tagDtos
                };
                return postDto;
            }
            ).ToList();

            return new PostPagedListDto
            {
                Posts = postsDto,
                Pagination = pagination
            };
        }
        public IQueryable<Post> GetSortedPosts(IQueryable<Post> posts, PostSorting postSorting)
        {
            return postSorting switch
            {
                PostSorting.CreateAsc => posts.OrderBy(post => post.CreateTime),
                PostSorting.CreateDesc => posts.OrderByDescending(post => post.CreateTime),
                PostSorting.LikeAsc => posts.OrderBy(post => post.Likes),
                PostSorting.LikeDesc => posts.OrderByDescending(post => post.Likes),
                _ => throw new ArgumentOutOfRangeException(nameof(postSorting), postSorting, null)
            };
        }


        public async Task<Guid> CreatePostInCommunity(PostCreateDto postCreateDto, Guid userId, Guid communityId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = "User not found."
                });

            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId) 
                ?? throw new NotFoundException(new Response
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
        public async Task AssignAdminToSubscriber(Guid adminId,Guid communityId,Guid userId)
        {
            var community = await _context.Communities.FirstOrDefaultAsync(c => c.Id == communityId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Community with Guid={communityId} not found."
                });

            if (!await _context.CommunityUsers.AnyAsync(cu => cu.CommunityId == communityId && cu.UserId == adminId && cu.Role == CommunityRole.Administrator))
            {
                throw new BadRequestException(new Response
                {
                    Status = "Error",
                    Message = $"User with id={adminId} is not administrator community with id={communityId}"
                });
            }
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                       ?? throw new NotFoundException(new Response
                       {
                           Status = "Error",
                           Message = "User not found."
                       });

            var communityUser =await _context.CommunityUsers.FirstOrDefaultAsync(cu =>cu.CommunityId == communityId && cu.UserId == userId) 
                ?? throw new BadRequestException(new Response
                {
                    Status = "Error",
                    Message = $"User with id={userId} not subscribed to the community with id={communityId}"
                });
            if (communityUser.Role == CommunityRole.Administrator)
            {
                throw new BadRequestException(new Response
                {
                    Status = "Error",
                    Message = $"User with id={userId} is already an administrator"
                });
            }

            communityUser.Role = CommunityRole.Administrator;
            community.SubscribersCount--;

            await _context.SaveChangesAsync();
        }

        public async Task<CommunityRoleDto> GetUserRoleInCommunity(Guid communityId, Guid userId)
        {
            var user = await _context.CommunityUsers.Where(cu => cu.CommunityId == communityId && cu.UserId == userId).FirstOrDefaultAsync();
            return new CommunityRoleDto() { Role = user?.Role };
        }

        public async Task<List<CommunityUserDto>> GetUserCommunities(Guid userId)
        {
            var communities = await _context.CommunityUsers.Where(cm => cm.UserId == userId).ToListAsync();
            var communityUserDtos = communities.Select(community => _mapper.Map<CommunityUserDto>(community)).ToList();
            return communityUserDtos;
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
