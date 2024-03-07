using synthesis.api.Data.Models;

namespace synthesis.api.Features.Feature;

public record FeatureDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public List<TaskDto>? Tasks { get; set; }
}

public record UpdateFeatureDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public FeatureType Type { get; set; }
}


public record CreateFeatureDto(string Name, string Description, FeatureType Type = FeatureType.Must);