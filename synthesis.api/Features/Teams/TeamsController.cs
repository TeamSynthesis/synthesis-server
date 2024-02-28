using System.ComponentModel.Design.Serialization;
using Microsoft.AspNetCore.Mvc;
using Scrutor;

namespace synthesis.api.Features.Teams;


[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _service;

    public TeamsController(ITeamService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganisation(Guid userId, [FromBody] CreateTeamDto organisation)
    {
        var response = await _service.CreateTeam(userId, organisation);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response.Data);
    }

    [HttpPost("{id:guid}/members/add")]
    public async Task<IActionResult> AddMember(Guid id, Guid userId)
    {
        var response = await _service.AddMember(id, userId);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}", Name = "OrganisationById")]
    public async Task<IActionResult> GetOrganisationById(Guid id)
    {
        var response = await _service.GetTeamById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/all", Name = "OrganisationWithResourcesById")]
    public async Task<IActionResult> GetOrganisationWithResourcesById(Guid id)
    {
        var response = await _service.GetTeamWithResourcesById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetOrganisationMembers(Guid id)
    {
        var response = await _service.GetTeamMembers(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/projects")]
    public async Task<IActionResult> GetOrganisationProjects(Guid id)
    {
        var response = await _service.GetTeamProjects(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrganisation(Guid id, [FromBody] UpdateTeamDto organisation)
    {
        var response = await _service.UpdateTeam(id, organisation);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchOrganisation(Guid id, [FromBody] UpdateTeamDto organisation)
    {
        var response = await _service.PatchTeam(id, organisation);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOrganisation(Guid id)
    {
        var response = await _service.DeleteTeam(id);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}