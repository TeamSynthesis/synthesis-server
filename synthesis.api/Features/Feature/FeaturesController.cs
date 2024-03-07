using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using synthesis.api.Features.Project;

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
    public async Task<IActionResult> CreateFeature(Guid projectId, [FromForm] CreateFeatureDto feature)
    {
        if (feature == null) return BadRequest("required parameters are null");

        var response = await _service.CreateFeature(projectId, feature);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/all")]
    public async Task<IActionResult> GetFeaturesWithResources(Guid id)
    {
        var response = await _service.GetFeatureWithResources(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFeature(Guid id, [FromForm] UpdateFeatureDto feature)
    {
        if (id == Guid.Empty || feature == null)
        {
            return BadRequest("required parameters are null");
        }

        var response = await _service.UpdateFeature(id, feature);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchFeature(Guid id, [FromBody] UpdateFeatureDto feature)
    {
        if (id == Guid.Empty || feature == null)
        {
            return BadRequest("required param is null");
        }
        var response = await _service.PatchFeature(id, feature);

        if (!response.IsSuccess)
            return BadRequest(response);

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

