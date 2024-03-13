using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CreateProject(Guid teamId, [FromForm] CreateProjectDto project)
    {
        var response = await _service.CreateProject(teamId, project);

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
    [HttpGet("generated-project")]
    public async Task<IActionResult> GetGeneratedProject(Guid processId)
    {
        var response = _service.GetGeneratedProject(processId.ToString());

        if (response.Message.Contains("pending")) return Accepted(response);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateProject([FromForm] string prompt)
    {
        var response = await _service.GenerateProject(prompt);
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
