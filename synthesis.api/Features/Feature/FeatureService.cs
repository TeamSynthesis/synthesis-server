using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Feature;

public interface IFeatureService
{

    Task<GlobalResponse<FeatureDto>> CreateFeature(Guid projectId, CreateFeatureDto createCommand);
    Task<GlobalResponse<FeatureDto>> GetFeatureById(Guid id);
    Task<GlobalResponse<FeatureDto>> GetFeatureWithResources(Guid id);
    Task<GlobalResponse<FeatureDto>> UpdateFeature(Guid id, UpdateFeatureDto updateCommand);
    Task<GlobalResponse<FeatureDto>> PatchFeature(Guid id, UpdateFeatureDto patchCommand);
    Task<GlobalResponse<FeatureDto>> DeleteFeature(Guid id);

}

public class FeatureService : IFeatureService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    public FeatureService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<GlobalResponse<FeatureDto>> CreateFeature(Guid projectId, CreateFeatureDto createCommand)
    {
        var projectExists = await _repository.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return new GlobalResponse<FeatureDto>(false, "create feature success", errors: [$"project with id {projectId} not found"]);

        var feature = new FeatureModel()
        {
            ProjectId = projectId,
            Name = createCommand.Name,
            Description = createCommand.Description,
            Type = createCommand.Type
        };

        var validationResult = new FeatureValidator().Validate(feature);

        if (!validationResult.IsValid) return new GlobalResponse<FeatureDto>(false, "create feature failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Features.AddAsync(feature);

        await _repository.SaveChangesAsync();

        var featureToReturn = new FeatureDto()
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description,
            Type = feature.Type.GetDisplayName()
        };


        return new GlobalResponse<FeatureDto>(true, "create feature success", value: featureToReturn);
    }

    public async Task<GlobalResponse<FeatureDto>> GetFeatureById(Guid id)
    {
        var feature = await _repository.Features.Where(ft => ft.Id == id)
        .Select(x => new FeatureDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Type = x.Type.GetDisplayName()
        }).FirstOrDefaultAsync();

        if (feature == null) return new GlobalResponse<FeatureDto>(false, "get feature failed", errors: [$"feature with id: {id} not found"]);

        return new GlobalResponse<FeatureDto>(true, "get feature success", value: feature);
    }

    public async Task<GlobalResponse<FeatureDto>> GetFeatureWithResources(Guid id)
    {
        var feature = await _repository.Features.Where(ft => ft.Id == id)
            .Select(ft => new FeatureDto()
            {
                Id = ft.Id,
                Name = ft.Name,
                Description = ft.Description,
                Type = ft.Type.GetDisplayName(),

                Tasks = ft.Tasks.Select(t => new TaskDto()
                {
                    Id = t.Id,
                    Activity = t.Activity,
                    State = t.State.GetDisplayName(),
                    Priority = t.Priority.GetDisplayName(),
                    IsComplete = t.IsComplete

                }).ToList()
            }).FirstOrDefaultAsync();
        if (feature == null) return new GlobalResponse<FeatureDto>(false, "get feature with resources failed", errors: [$"feature with id: {id} not found"]);

        var featureDto = _mapper.Map<FeatureDto>(feature);

        return new GlobalResponse<FeatureDto>(true, "get feature with resources success", value: featureDto);
    }

    public async Task<GlobalResponse<FeatureDto>> DeleteFeature(Guid id)
    {
        var feature = await _repository.Features.FirstOrDefaultAsync(ft => ft.Id == id);

        if (feature == null) return new GlobalResponse<FeatureDto>(false, "delete feature failed", errors: [$"feature with id: {id} not found"]);

        _repository.Features.Remove(feature);
        await _repository.SaveChangesAsync();

        return new GlobalResponse<FeatureDto>(true, "delete feature success");


    }

    public async Task<GlobalResponse<FeatureDto>> UpdateFeature(Guid id, UpdateFeatureDto updateCommand)
    {
        var feature = await _repository.Features.FirstOrDefaultAsync(ft => ft.Id == id);

        if (feature == null) return new GlobalResponse<FeatureDto>(false, "update feature failed", errors: [$"feature with id: {id} not found"]);

        feature.Name = updateCommand.Name;
        feature.Description = updateCommand.Description;
        feature.Type = updateCommand.Type;

        var validationResult = new FeatureValidator().Validate(feature);

        if (!validationResult.IsValid)
            return new GlobalResponse<FeatureDto>(false, "update feature failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.SaveChangesAsync();

        return new GlobalResponse<FeatureDto>(true, "update feature success");

    }

    public async Task<GlobalResponse<FeatureDto>> PatchFeature(Guid id, UpdateFeatureDto patchCommand)
    {
        var feature = await _repository.Features.FindAsync(id);

        if (feature == null) return new GlobalResponse<FeatureDto>(false, "patch feature failed", errors: [$"feature with id: {id} not found"]);

        var featureToBePatched = _mapper.Map<UpdateFeatureDto>(feature);

        var patchedFeature = Patcher.Patch(patchCommand, featureToBePatched);

        _mapper.Map(patchedFeature, feature);

        var validationResult = new FeatureValidator().Validate(feature);

        if (!validationResult.IsValid)
            return new GlobalResponse<FeatureDto>(false, "patch feature failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.SaveChangesAsync();

        return new GlobalResponse<FeatureDto>(true, "patch feature success");

    }

}