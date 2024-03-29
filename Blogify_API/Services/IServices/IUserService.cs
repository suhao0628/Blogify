﻿using Blogify_API.Dtos;
using Blogify_API.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace Delivery_API.Services.IServices
{
    public interface IUserService
    {
        bool IsUniqueUser(UserRegisterDto register);
        Task<TokenResponse> Register(UserRegisterDto register);
        Task<TokenResponse> Login(LoginDto login);
        Task Logout(string token);
        Task<bool> IsActiveToken(string token);
        Task<UserDto> GetProfile(Guid userId);
        Task EditProfile(UserEditDto userEditDto, Guid userId);
    }
}
