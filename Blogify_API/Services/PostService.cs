using AutoMapper;
using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Dtos.Post;
using Blogify_API.Entities;
using Blogify_API.Exceptions;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Blogify_API.Entities.Enums;
using Blogify_API.Dtos.Comment;
using Blogify_API.Dtos.Tag;
using System;

namespace Blogify_API.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public PostService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PostPagedListDto> GetAvailablePosts(Guid? userId, List<Guid>? tags, string? author, int? min, int? max, PostSorting? sorting, bool onlyMyCommunities, int page, int size)
        {
            IQueryable<Post> postsQueryable = _context.Posts.Include(p=>p.LikeLists).Include(p => p.Tags);
            if (!tags.IsNullOrEmpty())
            {
                foreach (var guid in tags)
                {
                    if (await _context.Tags.FirstOrDefaultAsync(tag => tag.Id == guid) == null)
                        throw new NotFoundException(new Response
                        {
                            Status = "Error",
                            Message = $"Tag with Guid={guid} not found."
                        });
                }

                postsQueryable = postsQueryable.ToList().Where(post => post.Tags.Select(tag => tag.Id).Intersect(tags).Count() == tags.Count).AsQueryable();
            }
            if (author != null)
            {
                postsQueryable = postsQueryable.Where(post => post.Author.Contains(author));
            }

            if (min != null)
            {
                postsQueryable = postsQueryable.Where(post => post.ReadingTime >= min);
            }

            if (max != null)
            {
                postsQueryable = postsQueryable.Where(post => post.ReadingTime <= max);
            }
            if (sorting != null)
            {
                postsQueryable = GetSortedPosts(postsQueryable, (PostSorting)sorting);
            }

            if (onlyMyCommunities && userId != null)
            {
                var communityUsers = await _context.CommunityUsers.Where(cu => cu.UserId == userId).ToListAsync();
                var communityIds = communityUsers.Where(cu => cu.UserId == userId).Select(cu => cu.CommunityId).ToList();
                postsQueryable = postsQueryable.Where(post => post.CommunityId != null && communityIds.Contains((Guid)post.CommunityId));
            }

            var postsCount = postsQueryable.Count();
            var paginationCount = !postsQueryable.IsNullOrEmpty() ? (int)Math.Ceiling((double)postsCount / size) : 0;
            if(page > (int)Math.Ceiling((double)paginationCount / size))
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

            var posts = postsQueryable.Skip((pagination.Current - 1) * pagination.Size).Take(pagination.Size).ToList();

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
                var tagDtos = post.Tags.Select(tag => _mapper.Map<TagDto>(tag)).ToList();
                return new PostDto
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
            }).ToList();

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

        public async Task<Guid> CreatePost(PostCreateDto postCreateDto, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = "User not found."
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
                CreateTime = DateTime.Now,
                Title = postCreateDto.Title,
                Description = postCreateDto.Description,
                ReadingTime = postCreateDto.ReadingTime,
                Image = postCreateDto.Image,
                AuthorId = user.Id,
                Author = user.FullName,
                CommunityId = null,
                CommunityName = null,
                Likes = 0,
                CommentsCount = 0,
                Tags = tags
            };
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();

            var postId = newPost.Id;
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
            return postId;
        }


        public async Task<PostFullDto> GetPostDetails(Guid postId, Guid? userId)
        {
            var post = await _context.Posts.Include(post => post.Tags).Include(post => post.Comments).Include(post => post.LikeLists).FirstOrDefaultAsync(post => post.Id == postId);


            if (post == null)
            {
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Post with Guid={postId} not found."
                });
            }
            var hasLike = false;
            if (userId != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                           ?? throw new NotFoundException(new Response
                           {
                               Status = "Error",
                               Message = "User not found."
                           });

                hasLike = post.LikeLists.Any(liked => liked.UserId == user.Id);
            }

            var tagDtos = post.Tags
                .Select(tag => _mapper.Map<TagDto>(tag))
                .ToList();

            var comments = post.Comments
                .Select(comment => _mapper.Map<CommentDto>(comment))
                .ToList();

            var postFullDto = new PostFullDto
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
                Tags = tagDtos,
                Comments = comments
            };

            return postFullDto;
        }

        public async Task AddLike(Guid postId, Guid userId)
        {
            var post = await _context.Posts.Include(post => post.Tags).Include(post => post.Comments).Include(post => post.LikeLists).FirstOrDefaultAsync(post => post.Id == postId)
                ??throw new NotFoundException(new Response
            {
                Status = "Error",
                Message = $"Post with Guid={postId} not found."
            });

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                            ?? throw new NotFoundException(new Response
                            {
                                Status = "Error",
                                Message = "User not found."
                            });

            var author = await _context.Authors.FirstOrDefaultAsync(author => author.UserId == post.AuthorId)
                         ?? throw new NotFoundException(new Response
                         {
                             Status = "Error",
                             Message = "Author not found."
                         });


            if (post.LikeLists.Any(liked => liked.UserId == user.Id))
                throw new Exception("User has already liked this post.");


            post.LikeLists.Add(
                new Like { PostId = post.Id, UserId = user.Id }
            );
            post.Likes++;
            author.Likes++;

            await _context.SaveChangesAsync();

        }

        public async Task DeleteLike(Guid postId, Guid userId)
        {
            var post = await _context.Posts.Include(post => post.Tags).Include(post => post.Comments).Include(post => post.LikeLists).FirstOrDefaultAsync(post => post.Id == postId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Post with Guid={postId} not found."
                });

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                            ?? throw new NotFoundException(new Response
                            {
                                Status = "Error",
                                Message = "User not found."
                            });


            var existingLike = await _context.Likes.FirstOrDefaultAsync(like =>like.PostId == post.Id&& like.UserId == user.Id)
                               ?? throw new Exception("User has not liked this post.");

            var author = await _context.Authors.FirstOrDefaultAsync(author => author.UserId == post.AuthorId)
                        ?? throw new NotFoundException(new Response
                        {
                            Status = "Error",
                            Message = "Author not found."
                        });


            post.LikeLists.Remove(existingLike);
            post.Likes--;
            author.Likes--;
            await _context.SaveChangesAsync();
        }

    }
}