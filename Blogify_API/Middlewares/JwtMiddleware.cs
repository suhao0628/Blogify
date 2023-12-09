using Delivery_API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
namespace Delivery_API.Middleware
{
    public class JwtMiddleware : IMiddleware
    {
        private readonly IUserService _userService;

        public JwtMiddleware(IUserService userService)
        {
            _userService = userService;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (HasAuthorizeAttribute(context))
            {
                var token = string.Empty;

                if (context.Request.Headers.TryGetValue("authorization", out var authHeaderValue))
                {
                    token = authHeaderValue.FirstOrDefault()? .Split(" ").Last() ?? string.Empty;
                }
                if (!await _userService.IsActiveToken(token))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    return;
                }
            }

            await next(context);
        }
        private bool HasAuthorizeAttribute(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var authorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();
            return authorizeAttribute != null;
        }
    }
}