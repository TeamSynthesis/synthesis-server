using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Project;

public interface IProjectService
{
    Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest);

    Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id);

    Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateRequest);

    Task<GlobalResponse<ProjectDto>> PatchProject(Guid id, UpdateProjectDto patchRequest);

    Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id);
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

    public async Task<GlobalResponse<ProjectDto>> CreateProject(Guid organisationId, Guid memberId, CreateProjectDto createRequest)
    {
        var organisationExists = await _repository.Organisations.AnyAsync(org => org.Id == organisationId);
        if (!organisationExists) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"organisation with id: {organisationId} not found"]);

        var member = await _repository.Members.FindAsync(memberId);
        if (member == null) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: [$"member with id: {memberId} not found"]);

        if (member.Roles != null && member.Roles.Contains(UserRoles.Owner))
        {
            member.Roles.Add(UserRoles.Manager);
        }

        var project = _mapper.Map<ProjectModel>(createRequest);

        project.OrganisationId = organisationId;

        var validationResult = new ProjectValidator().Validate(project);
        if (!validationResult.IsValid) return new GlobalResponse<ProjectDto>(false, "create project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Projects.AddAsync(project);

        await _repository.SaveChangesAsync();

        var projectToReturn = _mapper.Map<ProjectDto>(project);

        return new GlobalResponse<ProjectDto>(true, "create project success", value: projectToReturn);
    }

    public async Task<GlobalResponse<ProjectDto>> GetProjectById(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);
        if (project == null) return new GlobalResponse<ProjectDto>(false, "get project failed", errors: [$"project with id: {id} not found"]);

        var projectToReturn = _mapper.Map<ProjectDto>(project);

        return new GlobalResponse<ProjectDto>(true, "get project success", value: projectToReturn);
    }

    public async Task<GlobalResponse<ProjectDto>> UpdateProject(Guid id, UpdateProjectDto updateRequest)
    {
        var project = await _repository.Projects.FindAsync(id);

        if (project == null) return new GlobalResponse<ProjectDto>(false, "update project failed", errors: [$"project with id: {id} not found"]);

        var updatedProject = _mapper.Map(updateRequest, project);

        var validationResult = new ProjectValidator().Validate(updatedProject);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<ProjectDto>(false, "update project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<ProjectDto>(true, "update project success");

    }

    public async Task<GlobalResponse<ProjectDto>> PatchProject(Guid id, UpdateProjectDto patchRequest)
    {
        var project = await _repository.Projects.FindAsync(id);
        if (project == null) return new GlobalResponse<ProjectDto>(false, "patch project failed", errors: [$"project with id: {id} not found"]);

        var projectToBePatched = _mapper.Map<UpdateProjectDto>(project);

        var patchedProjectDto = Patcher.Patch(patchRequest, projectToBePatched);

        var patchedProject = _mapper.Map(patchedProjectDto, project);

        var validationResult = new ProjectValidator().Validate(patchedProject);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<ProjectDto>(false, "update project failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.SaveChangesAsync();

        return new GlobalResponse<ProjectDto>(true, "patch project success");



    }
    public async Task<GlobalResponse<ProjectDto>> DeleteProject(Guid id)
    {
        var project = await _repository.Projects.FindAsync(id);

        if (project == null) return new GlobalResponse<ProjectDto>(false, "delete project failed", errors: [$"project with id: {id} not found"]);

        _repository.Projects.Remove(project);

        await _repository.SaveChangesAsync();

        return new GlobalResponse<ProjectDto>(true, "delete project success");
    }
}