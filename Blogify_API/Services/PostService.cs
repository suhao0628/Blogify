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

namespace Blogify_API.Services
{
    public class PostService: IPostService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public PostService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
    }
}
