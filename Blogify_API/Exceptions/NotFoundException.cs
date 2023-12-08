using Blogify_API.Dtos;

namespace Blogify_API.Exceptions
{
    public class NotFoundException:Exception
    {
        public Response ErrorResponse { get; }
        public NotFoundException(Response errorResponse) : base(errorResponse.Message)
        {
            ErrorResponse = errorResponse;
        }
    }
}
