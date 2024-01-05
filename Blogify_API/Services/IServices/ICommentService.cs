using Blogify_API.Dtos.Comment;

namespace Blogify_API.Services.IServices
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetNestedComments(Guid commentId);
        Task AddComment(Guid postId, Guid userId, CommentCreateDto commentCreateDto);
        Task UpdateComment(Guid commentId, Guid userId, CommentUpdateDto commentUpdateDto);
        Task DeleteComment(Guid commentId, Guid userId);
    }
}
