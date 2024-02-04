using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<Response<ProjectDto>> GetProjectById(Guid id);
    Task<Response<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest);
}

public class ProjectService : IProjectService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;

    public ProjectService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<ProjectDto>> GetProjectById(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);
        if (project == null) return new Response<ProjectDto>(false, "get project failed", errors: [$"project with id: {id} not found"]);

        var projectToReturn = _mapper.Map<ProjectDto>(project);

        return new Response<ProjectDto>(true, "get project success", value: projectToReturn);
    }

    public async Task<Response<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest)
    {
        var organisationExists = await _repository.Organisations.AnyAsync(org => org.Id == organisationId);
        if (!organisationExists) return new Response<ProjectDto>(false, "create project failed", errors: [$"organisation with id: {organisationId} not found"]);

        var member = await _repository.Members.FindAsync(memberId);
        if (member == null) return new Response<ProjectDto>(false, "create project failed", errors: [$"member with id: {memberId} not found"]);

        if (member.Roles != null && member.Roles.Contains(UserRoles.Owner))
        {
            member.Roles.Add(UserRoles.Manager);
        }

        var project = _mapper.Map<ProjectModel>(createRequest);

        project.OrganisationId = organisationId;

        var validationResult = new ProjectValidator().Validate(project);
        if (!validationResult.IsValid) return new Response<ProjectDto>(false, "create project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Projects.AddAsync(project);

        await _repository.SaveChangesAsync();

        var projectToReturn = _mapper.Map<ProjectDto>(project);

        return new Response<ProjectDto>(true, "create project success", value: projectToReturn);
    }
}