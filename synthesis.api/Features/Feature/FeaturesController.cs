using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Feature;

[ApiController]
[Route("api/[controller]")]
public class FeaturesController : ControllerBase
{

    private readonly IFeatureService _service;
    public FeaturesController(IFeatureService service)
    {
        _service = service;

    }

    [HttpPost]
    public async Task<IActionResult> CreateFeature(Guid projectId, [FromBody] CreateFeatureDto feature)
    {
        if (projectId == default || feature == null) return BadRequest("required parameters are null");

        var response = await _service.CreateFeature(projectId, feature);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFeatureById(Guid id)
    {
        if (id == default) return BadRequest("required parameters are null");

        var response = await _service.GetFeatureById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

}

