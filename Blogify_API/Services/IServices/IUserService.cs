using Blogify_API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Services.IServices
{
    public interface IUserService
    {
        bool IsUniqueUser(UserRegisterDto register);
        Task<TokenResponse> Register(UserRegisterDto register);
        Task<TokenResponse> Login(LoginDto credentials);
    }
}
