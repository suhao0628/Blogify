using AutoMapper;
using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Dtos.Comment;
using Blogify_API.Entities;
using Blogify_API.Exceptions;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Blogify_API.Services
{
    public class CommentService: ICommentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CommentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CommentDto>> GetNestedComments(Guid commentId)
        {
            var parentComment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Comment with Guid={commentId} not found."
                });
                               

            if (parentComment.ParentId != null)
            throw new NotFoundException(new Response
            {
                Status = "Error",
                Message = $"Comment with Guid={commentId} is not a root comment."
            });

            var stack = new Stack<Comment>();

            var comments = new List<Comment>();

            var subComments = await _context.Comments.Where(c => c.ParentId == parentComment.Id).OrderByDescending(c => c.CreateTime).ToListAsync();

            foreach (var subComment in subComments)
            {
                stack.Push(subComment);
            }


            while (stack.Count > 0)
            {
                var currentComment = stack.Pop();

                comments.Add(currentComment);

                var subCommens = await _context.Comments.Where(c => c.ParentId == currentComment.Id).OrderByDescending(c => c.CreateTime).ToListAsync();

                foreach (var subComment in subCommens)
                {
                    stack.Push(subComment);
                }
            }
            var commentDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var commentDto = new CommentDto()
                {
                    Id = comment.Id,
                    CreateTime = comment.CreateTime,
                    Content = comment.Content,
                    ModifiedDate = comment.ModifiedDate,
                    DeleteDate = comment.DeleteDate,
                    AuthorId = comment.AuthorId,
                    Author = comment.Author,
                    SubComments = comment.SubComments
                };
                commentDtos.Add(commentDto);
            }
            return commentDtos;
        }
        public async Task AddComment(Guid postId, Guid userId, CommentCreateDto commentCreateDto)
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

            //Guid? topLevelCommentId = null;

            if (commentCreateDto.ParentId != null)
            {
                var parentComment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == (Guid)commentCreateDto.ParentId);
                if (parentComment == null || parentComment.PostId != postId)
                    throw new NotFoundException(new Response
                    {
                        Status = "Error",
                        Message = $"Comment with ParentId = {commentCreateDto.ParentId} not found."
                    });
                parentComment.SubComments++;
            }

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                PostId = postId,
                ParentId = commentCreateDto.ParentId,
                Content = commentCreateDto.Content,
                ModifiedDate = null,
                DeleteDate = null,
                AuthorId = user.Id,
                Author = user.FullName,
                SubComments = 0
            }; 

            post.CommentsCount++;

            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateComment(Guid commentId, Guid userId, CommentUpdateDto commentUpdateDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                        ?? throw new NotFoundException(new Response
                        {
                            Status = "Error",
                            Message = "User not found."
                        });
            var comment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Comment with Guid={commentId} not found."
                });
            var post = await _context.Posts.Include(post => post.Tags).Include(post => post.Comments).Include(post => post.LikeLists).FirstOrDefaultAsync(post => post.Id == comment.PostId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Post with Guid={comment.PostId} not found."
                });

            if (post.CommunityId != null)
            {
                var community = await _context.Communities.Include(community => community.CommunityUsers).FirstOrDefaultAsync(community => community.Id == post.CommunityId)
                                ?? throw new NotFoundException(new Response
                                {
                                    Status = "Error",
                                    Message = $"Community with Guid={post.CommunityId} not found."
                                });
                if (community.IsClosed && await _context.CommunityUsers.FirstOrDefaultAsync(cu =>cu.CommunityId == community.Id&& cu.UserId == userId) == null)
                    throw new NotFoundException(new Response
                    {
                        Status = "Error",
                        Message = $"This user can't interact with closed group with Guid={community.Id}"
                    });
            }

            if (comment.AuthorId != user.Id)
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = "Users can only interact with their own comments."
                });

            comment.Content = commentUpdateDto.Content;
            comment.ModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(Guid commentId, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId)
                        ?? throw new NotFoundException(new Response
                        {
                            Status = "Error",
                            Message = "User not found."
                        });

            var comment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Comment with Guid={commentId} not found."
                });

            var post = await _context.Posts.Include(post => post.Tags).Include(post => post.Comments).Include(post => post.LikeLists).FirstOrDefaultAsync(post => post.Id == comment.PostId)
                ?? throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = $"Post with Guid={comment.PostId} not found."
                });

            if (post.CommunityId != null)
            {
                var community = await _context.Communities.Include(community => community.CommunityUsers).FirstOrDefaultAsync(community => community.Id == post.CommunityId)
                                ?? throw new NotFoundException(new Response
                                {
                                    Status = "Error",
                                    Message = $"Community with Guid={post.CommunityId} not found."
                                });
                if (community.IsClosed && await _context.CommunityUsers.FirstOrDefaultAsync(cu => cu.CommunityId == community.Id && cu.UserId == userId) == null)
                    throw new NotFoundException(new Response
                    {
                        Status = "Error",
                        Message = $"This user can't interact with closed group with Guid={community.Id}"
                    });
            }

            if (comment.AuthorId != user.Id)
                throw new NotFoundException(new Response
                {
                    Status = "Error",
                    Message = "Users can only interact with their own comments."
                });
            if (comment.DeleteDate != null)
            throw new NotFoundException(new Response
            {
                Status = "Error",
                Message = "This comment is already deleted."
            });
            if (comment.SubComments == 0)
            {
                _context.Comments.Remove(comment);

                if (comment.ParentId != null)
                {
                    var parentComment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == (Guid)comment.ParentId)

                       ?? throw new NotFoundException(new Response
                       {
                           Status = "Error",
                           Message = $"Comment with Guid={comment.ParentId} not found."});
                    parentComment.SubComments--;
                }
            }
            else
            {
                comment.Content = "";
                comment.DeleteDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
