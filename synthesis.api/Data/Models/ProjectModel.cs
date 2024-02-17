using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace synthesis.api.Data.Models;

public class ProjectModel
{
    [Column("ProjectId")]
    public Guid Id { get; set; }

    [ForeignKey(nameof(OrganisationModel))]
    public Guid OrganisationId { get; set; }

    public string? Name { get; set; }
    public string? IconUrl { get; set; }

    public OrganisationModel? Organisation { get; set; }

    [Column(TypeName = "jsonb")]
    public ProjectMetadata? ProjectMetadata { get; set; }
}

public class ProjectMetadata
{
    public Overview? Overview { get; set; }
    public CompetitiveAnalysis? CompetitiveAnalysis { get; set; }
    public Branding? Branding { get; set; }
    public Features? Features { get; set; }
    public Technology? Technology { get; set; }
}

public class Overview
{
    public string? Title { get; set; }
    public string? UserPrompt { get; set; }
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

    public List<Wireframe>? Wireframes { get; set; }

    public List<Image>? MoodBoards { get; set; }

    public ColorPalette? Palette { get; set; }

    public Typography? Typography { get; set; }

}

public class Features
{
    public List<Feature>? Must { get; set; }
    public List<Feature>? Should { get; set; }
    public List<Feature>? Could { get; set; }
    public List<string>? Wont { get; set; }
}

public class Feature
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<TaskToDo>? Tasks { get; set; }
}

public class TaskToDo
{
    public string? TaskId { get; set; }
    public string? Activity { get; set; }
    public int State { get; set; }
    public string? MemberId { get; set; }
}

public enum TaskState
{
    Pending,
    InProgress,
    Done
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
    public Dictionary<string, string>? Neutral { get; set; }
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