using AutoMapper;
using synthesis.api.Data.Models;
using synthesis.api.Features.Auth;
using synthesis.api.Features.Feature;
using synthesis.api.Features.Project;
using synthesis.api.Features.User;

namespace synthesis.api.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    //user
    CreateMap<UserDto, UserModel>().ReverseMap();
    CreateMap<UpdateUserDto, UserModel>().ReverseMap();

    //auth
    CreateMap<RegisterUserDto, UserModel>();


    //organisation
    CreateMap<TeamDto, TeamModel>().ReverseMap();
    CreateMap<CreateTeamDto, TeamModel>();
    CreateMap<UpdateTeamDto, TeamModel>().ReverseMap();

    //project
    CreateMap<ProjectDto, ProjectModel>().ReverseMap();
    CreateMap<CreateProjectDto, ProjectModel>();
    CreateMap<UpdateProjectDto, ProjectModel>().ReverseMap();

    //tasks
    CreateMap<TaskDto, TaskToDoModel>().ReverseMap();
    CreateMap<UpdateTaskDto, TaskToDoModel>().ReverseMap();

    //feature
    CreateMap<FeatureDto, FeatureModel>().ReverseMap();
    CreateMap<UpdateFeatureDto, FeatureModel>().ReverseMap();
    //Member
    CreateMap<MemberModel, MemberDto>()
      .ForMember(dto => dto.User,
      opt => opt.MapFrom(m => m.User));
    ;




  }

}