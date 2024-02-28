namespace synthesis.api.Services.OpenAi.Dtos;

public class GptFeatureDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Type { get; set; }
    public List<GptTaskDto>? Tasks { get; set; }
}

public class GptTaskDto
{
    public string? Activity { get; set; }
    public int Priority { get; set; }
}
