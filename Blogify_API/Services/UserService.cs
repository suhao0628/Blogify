using AutoMapper;
using Blogify_API;
using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Exceptions;
using Blogify_API.Services.IServices;
using Delivery_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Delivery_API.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IDistributedCache _cache;
        private readonly IOptions<JwtConfig> _jwtOptions;

        public UserService(AppDbContext context, IMapper mapper, IJwtService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
            
        }
        public bool IsUniqueUser(UserRegisterDto register)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == register.Email);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        public async Task<TokenResponse> Register(UserRegisterDto register)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == register.Email);
            var registerUser = _mapper.Map<User>(register);

            await _context.Users.AddAsync(registerUser);
            await _context.SaveChangesAsync();

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtService.GenerateToken(registerUser))
            };
        }
        public async Task<TokenResponse> Login(LoginDto login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == login.Email);

            if (user == null)
            {
                throw new BadRequestException(new Response
                {
                    Message = "User does not exist. Login failed"
                });
            }
            if (user.Password != login.Password)
            {
                throw new BadRequestException(new Response
                {
                    Message = "Incorrect Password. Login failed"
                });
            }
            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(_jwtService.GenerateToken(user))
            };
        }

        public async Task<UserDto> GetProfile(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            return user == null
                ? throw new NotFoundException(new Response
                {
                    Message = "User does not exist"
                })
                : _mapper.Map<UserDto>(user);
        }
        public async Task EditProfile(UserEditDto profile, Guid userId)
        {
            var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException(new Response
                {
                    Message = "User does not exist"
                });
            user.FullName = profile.FullName;
            user.BirthDate = profile.BirthDate;
            user.Gender = profile.Gender;
            user.PhoneNumber = profile.PhoneNumber;

            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task Logout(string token)
        {
            await _cache.SetStringAsync(token,
                " ", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(_jwtOptions.Value.Expires)
                });
        }
        public async Task<bool> IsActiveToken(string token)
        {
            return await _cache.GetStringAsync(token) == null;
        }
    }
}
