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


    [HttpGet("{id:guid}", Name = "ProjectById")]
    public async Task<IActionResult> GetProjectById(Guid id)
    {
        var response = await _service.GetProjectById(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateProject(Guid organisationId, Guid memberId, [FromBody] CreateProjectDto project)
    {
        var response = await _service.CreateProject(organisationId, memberId, project);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return CreatedAtRoute("ProjectById", new { id = response.Value.Id }, response.Value);
    }
}