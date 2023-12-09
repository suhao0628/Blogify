using AutoMapper;
using Blogify_API.Dtos;
using Blogify_API.Dtos.Community;
using Blogify_API.Entities;

namespace Blogify_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserRegisterDto>().ReverseMap();

            CreateMap<Community, CommunityDto>().ReverseMap();
        }
    }
}
