namespace synthesis.api.Services.OpenAi.Dtos;

public class GenerateBrandingDto
{
    public ColorPaletteDto? ColorPalette { get; set; }
    public List<WireframeDto>? Wireframes { get; set; }
    public MoodBoardDto? MoodBoard { get; set; }
    public TypographyDto? Typography { get; set; }
    public List<IconDto>? Icons { get; set; }
    public string? Slogan { get; set; }
}

public class ColorPaletteDto
{
    public Dictionary<string, string>? Primary { get; set; }
    public Dictionary<string, string>? Secondary { get; set; }
    public Dictionary<string, string>? Neutral { get; set; }
    public string? PreviewUrl { get; set; }
    public string? Reason { get; set; }
}

public class WireframeDto
{
    public string? Screen { get; set; }
    public string? Description { get; set; }
    public string? ImagePrompt { get; set; }
}

public class MoodBoardDto
{
    public List<MoodBoardImage>? Images { get; set; }
}

public class MoodBoardImage
{
    public string? ImagePrompt { get; set; }
    public string? Description { get; set; }
}

public class TypographyDto
{
    public string? Font { get; set; }
    public string? Reason { get; set; }
}

public class IconDto
{
    public string? Reason { get; set; }
    public string? ImagePrompt { get; set; }
}
