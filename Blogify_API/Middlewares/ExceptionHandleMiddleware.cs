﻿using Blogify_API.Dtos;
using Blogify_API.Exceptions;
using Newtonsoft.Json;
namespace Delivery_API.Middleware
{
    public class ExceptionHandleMiddleware: IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BadRequestException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = new Response { Status = ex.ErrorResponse.Status, Message = ex.ErrorResponse.Message };
                var jsonResponse = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(jsonResponse);
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";

                var response = new Response { Status = ex.ErrorResponse.Status, Message = ex.ErrorResponse.Message };
                var jsonResponse = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }

    //public static class ExceptionHandlingMiddlewareExtensions
    //{
    //    public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
    //    {
    //        return builder.UseMiddleware<ExceptionHandleMiddleware>();
    //    }
    //}
}