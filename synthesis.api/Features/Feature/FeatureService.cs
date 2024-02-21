using AutoMapper;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Feature;

public interface IFeatureService
{

    Task<GlobalResponse<FeatureDto>> CreateFeature(Guid projectId, CreateFeatureDto createCommand);
    Task<GlobalResponse<FeatureDto>> GetFeatureById(Guid featureId);

}

public class FeatureService : IFeatureService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    public FeatureService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
    }
    public async Task<GlobalResponse<FeatureDto>> CreateFeature(Guid projectId, CreateFeatureDto createCommand)
    {
        var projectExists = await _repository.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists) return new GlobalResponse<FeatureDto>(false, "create feature success", errors: [$"project with id {projectId} not found"]);

        var feature = new FeatureModel()
        {
            Name = createCommand.Name,
            Description = createCommand.Description,
            Type = createCommand.Type
        };

        var validationResult = new FeatureValidator().Validate(feature);

        if (!validationResult.IsValid) return new GlobalResponse<FeatureDto>(false, "create feature failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        await _repository.Features.AddAsync(feature);

        await _repository.SaveChangesAsync();

        var featureToReturn = _mapper.Map<FeatureDto>(feature);

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
            Type = x.Type
        }).SingleOrDefaultAsync();

        if (feature == null) return new GlobalResponse<FeatureDto>(false, "get feature failed", errors: [$"feature with id: {id} not found"]);

        return new GlobalResponse<FeatureDto>(true, "get feature success", value: feature);
    }


}