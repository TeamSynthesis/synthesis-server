using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Features.Auth;
using synthesis.api.Features.User;

namespace synthesis.api.Mappings;

public class MappingProfile : Profile
{
    //user
    public MappingProfile()
    {
        CreateMap<UserDto, UserModel>().ReverseMap();
        CreateMap<RegisterUserDto, UserModel>();
    }

}