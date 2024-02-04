using System.ComponentModel.Design.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Organisation;


[ApiController]
[Route("api/[controller]")]
public class OrganisationsController : ControllerBase
{
    private readonly IOrganisationService _service;

    public OrganisationsController(IOrganisationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganisation(Guid userId, [FromBody] CreateOrganisationDto organisation)
    {
        var response = await _service.CreateOrganisation(userId, organisation);

        if (!response.IsSuccess) return BadRequest(response);

        return CreatedAtAction("OrganisationById", new { id = response.Value.Id }, response.Value);
    }

    [HttpGet("{id:guid}", Name = "OrganisationById")]
    public async Task<IActionResult> GetOrganisationById(Guid id)
    {
        var response = await _service.GetOrganisationById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/all", Name = "OrganisationWithResourcesById")]
    public async Task<IActionResult> GetOrganisationWithResourcesById(Guid id)
    {
        var response = await _service.GetOrganisationWithResourcesById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetOrganisationMembers(Guid id)
    {
        var response = await _service.GetOrganisationMembers(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/projects")]
    public async Task<IActionResult> GetOrganisationProjects(Guid id)
    {
        var response = await _service.GetOrganisationProjects(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrganisation(Guid id, [FromBody] UpdateOrganisationDto organisation)
    {
        var response = await _service.UpdateOrganisation(id, organisation);
        if (!response.IsSuccess) return BadRequest(response);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchOrganisation(Guid id, [FromBody] UpdateOrganisationDto organisation)
    {
        var response = await _service.PatchOrganisation(id, organisation);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return NoContent();
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrganisation(Guid id)
    {
        var response = await _service.DeleteOrganisation(id);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return NoContent();
    }
}