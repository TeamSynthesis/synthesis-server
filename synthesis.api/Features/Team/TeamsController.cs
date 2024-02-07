using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Team;

public class TeamsController : ControllerBase
{
    private readonly ITeamService _service;
    public TeamsController(ITeamService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam(Guid projectId, [FromBody] CreateTeamDto team)
    {
        var response = await _service.CreateTeam(projectId, team);
        if (!response.IsSuccess) return BadRequest(team);

        return CreatedAtRoute("TeamById", new { response.Value.id }, response.Value);
    }

    [HttpGet("{id:guid}", Name = "TeamById")]
    public async Task<IActionResult> GetTeamById(Guid id)
    {
        var response = await _service.GetTeamById(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] UpdateTeamDto team)
    {
        var response = await _service.UpdateTeam(id, team);
        if (!response.IsSuccess) return BadRequest(response);

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchTeam(Guid id, [FromBody] UpdateTeamDto team)
    {
        var response = await _service.PatchTeam(id, team);
        if (!response.IsSuccess) return BadRequest(response);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var response = await _service.DeleteTeam(id);
        if (!response.IsSuccess) return BadRequest(response);

        return NoContent();
    }

}