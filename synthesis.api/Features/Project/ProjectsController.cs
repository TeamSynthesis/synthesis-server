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

    [HttpPost("ai-project")]
    public async Task<IActionResult> CreateAiProject(Guid planId)
    {
        var response = await _service.CreateAiGeneratedProject(planId);

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
    public async Task<IActionResult> GetGeneratedProject(Guid planId)
    {
        var response = await _service.GetGeneratedProject(planId);

        if (response.IsSuccess) return Accepted(response);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateProject([FromForm] Guid teamId, [FromForm] string prompt)
    {
        var response = await _service.GenerateProject(teamId, prompt);
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
