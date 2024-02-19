using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Features.Project;
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

    //project
    CreateMap<ProjectDto, ProjectModel>().ReverseMap();
    CreateMap<CreateProjectDto, ProjectModel>();
    CreateMap<UpdateProjectDto, ProjectModel>().ReverseMap();

    //tasks
    CreateMap<TaskDto, TaskToDoModel>().ReverseMap();
    CreateMap<UpdateTaskDto, TaskToDoModel>().ReverseMap();


    //Member
    CreateMap<MemberModel, MemberDto>()
      .ForMember(dto => dto.User,
      opt => opt.MapFrom(m => m.User));
    ;




  }

}