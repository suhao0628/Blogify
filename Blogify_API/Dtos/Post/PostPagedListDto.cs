namespace Blogify_API.Dtos.Post
{
    public class PostPagedListDto
    {
        public List<PostDto>? Posts { get; set; }
        public PageInfoModel Pagination { get; set; }
    }
}
