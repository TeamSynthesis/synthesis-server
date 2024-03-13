namespace synthesis.api.Data.Models;

public class ProjectMetadata
{
    public Overview? Overview { get; set; }
    public CompetitiveAnalysis? CompetitiveAnalysis { get; set; }
    public Branding? Branding { get; set; }
    public Technology? Technology { get; set; }
}


public class Overview
{
    public string? Description { get; set; }
    public List<SuggestedName>? SuggestedNames { get; set; }
    public List<SuggestedDomain>? SuggestedDomains { get; set; }
}

public class CompetitiveAnalysis
{
    public List<Competitor>? Competitors { get; set; }
    public Swot? Swot { get; set; }
    public TargetAudience? TargetAudience { get; set; }
}
public class Branding
{

    public Image? Icon { get; set; }
    public string? Slogan { get; set; }

    public Wireframe? Wireframe { get; set; }

    public Image? MoodBoard { get; set; }

    public ColorPalette? Palette { get; set; }

    public Typography? Typography { get; set; }

}

public class SuggestedName
{
    public string? Name { get; set; }
    public string? Reason { get; set; }
}

public class SuggestedDomain
{
    public string? Name { get; set; }
    public string? Reason { get; set; }
}

public class Image
{
    public string? ImgUrl { get; set; }
    public string? Description { get; set; }
}

public class Competitor
{
    public string? Name { get; set; }
    public string? Size { get; set; }
    public double ReviewSentiment { get; set; }
    public List<string>? Features { get; set; }
    public string? PricingModel { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
}

public class Swot
{
    public List<string>? Strengths { get; set; }
    public List<string>? Weaknesses { get; set; }
    public List<string>? Opportunities { get; set; }
    public List<string>? Threats { get; set; }
}

public class ColorPalette
{

    public Dictionary<string, string>? Primary { get; set; }
    public Dictionary<string, string>? Secondary { get; set; }
    public Dictionary<string, string>? Accent { get; set; }
    public string? PreviewUrl { get; set; }
    public string? Reason { get; set; }
}

public class Wireframe
{
    public string? Screen { get; set; }
    public Image? Image { get; set; }
}

public class Typography
{
    public string? Font { get; set; }
    public string? Reason { get; set; }
}

public class Technology
{
    public List<TechStack>? TechStacks { get; set; }
}
public class TechStack
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Reason { get; set; }

}

public class TargetAudience
{
    public Demographics? Demographics { get; set; }
}

public class Demographics
{
    public string? Age { get; set; }
}