using Blogify_API.Dtos;

namespace Blogify_API.Exceptions
{
    public class ForbiddenException:Exception
    {
        public Response ErrorResponse { get; }
        public ForbiddenException(Response errorResponse) : base(errorResponse.Message)
        {
            ErrorResponse = errorResponse;
        }
    }
}
