using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Features.User;

namespace synthesis.api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //user
        CreateMap<UserDto, UserModel>().ReverseMap();
        CreateMap<RegisterUserDto, UserModel>();
        CreateMap<UpdateUserDto, UserModel>().ReverseMap();

        //organisation
        CreateMap<OrganisationDto, OrganisationModel>().ReverseMap();
        CreateMap<CreateOrganisationDto, OrganisationModel>();
        CreateMap<UpdateOrganisationDto, OrganisationModel>().ReverseMap();

        //Member
        CreateMap<MemberModel, MemberDto>()
          .ForMember(dto => dto.User,
          opt => opt.MapFrom(m => m.User));

    }

}