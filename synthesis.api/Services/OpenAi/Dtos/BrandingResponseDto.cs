namespace synthesis.api.Services.OpenAi.Dtos;

public class BrandingResponseDto
{
    public ColorPalette? ColorPalette { get; set; }
    public List<Wireframe>? Wireframes { get; set; }
    public MoodBoard? MoodBoard { get; set; }
    public Typography? Typography { get; set; }
    public List<Icon>? Icons { get; set; }
    public string? Slogan { get; set; }
}

public class ColorPalette
{
    public Dictionary<string, string>? Primary { get; set; }
    public Dictionary<string, string>? Secondary { get; set; }
    public Dictionary<string, string>? Neutral { get; set; }
    public string? PreviewUrl { get; set; }
    public string? Reason { get; set; }
}

public class Wireframe
{
    public string? Screen { get; set; }
    public string? Description { get; set; }
    public string? ImagePrompt { get; set; }
}

public class MoodBoard
{
    public List<MoodBoardImage>? Images { get; set; }
}

public class MoodBoardImage
{
    public string? ImagePrompt { get; set; }
    public string? Description { get; set; }
}

public class Typography
{
    public string? Font { get; set; }
    public string? Reason { get; set; }
}

public class Icon
{
    public string? Reason { get; set; }
    public string? ImagePrompt { get; set; }
}
