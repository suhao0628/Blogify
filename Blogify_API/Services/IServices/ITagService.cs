using Blogify_API.Dtos.Tag;

namespace Blogify_API.Services.IServices
{
    public interface ITagService
    {
        Task<List<TagDto>> GetTags();
    }
}
