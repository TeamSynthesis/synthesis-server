using Microsoft.AspNetCore.Mvc;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.Project;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;
    public ProjectsController(IProjectService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Guid organisationId, Guid memberId, [FromBody] CreateProjectDto project)
    {
        var response = await _service.CreateProject(organisationId, memberId, project);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:guid}", Name = "ProjectById")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var response = await _service.GetProjectById(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto project)
    {
        var response = await _service.UpdateProject(id, project);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchProject(Guid id, [FromBody] UpdateProjectDto project)
    {
        var response = await _service.PatchProject(id, project);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var response = await _service.DeleteProject(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

}