﻿using Blogify_API.Dtos;
using System.IdentityModel.Tokens.Jwt;

namespace Blogify_API.Services.IServices
{
    public interface IJwtService
    {
        JwtSecurityToken GenerateToken(User user);
    }
}
