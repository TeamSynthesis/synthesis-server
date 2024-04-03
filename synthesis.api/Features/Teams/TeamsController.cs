using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CreateTeam(Guid userId, [FromForm] CreateTeamDto team)
    {
        var response = await _service.CreateTeam(userId, team);

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


    [HttpPost("{id:guid}/members/remove")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
    {
        var response = await _service.RemoveMember(id, userId);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTeamById(Guid id)
    {
        var response = await _service.GetTeamById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }


    [HttpGet("{id:guid}/all")]
    public async Task<IActionResult> GetTeamWithResourcesById(Guid id)
    {
        var response = await _service.GetTeamWithResourcesById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<IActionResult> GetTeamMembers(Guid id)
    {
        var response = await _service.GetTeamMembers(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{id:guid}/projects")]
    public async Task<IActionResult> GetTeamProjects(Guid id)
    {
        var response = await _service.GetTeamProjects(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromForm] UpdateTeamDto team)
    {
        var response = await _service.UpdateTeam(id, team);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchTeam(Guid id, [FromForm] UpdateTeamDto team)
    {
        var response = await _service.PatchTeam(id, team);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        var response = await _service.DeleteTeam(id);
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}