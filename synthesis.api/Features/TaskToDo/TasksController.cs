using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using Scrutor;
using synthesis.api.Features.TaskToDo;


[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskToDoService _service;
    public TasksController(ITaskToDoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(Guid projectId, [FromBody] CreateTaskDto task)
    {
        if (task == null) return BadRequest("required body parameter is null");

        var response = await _service.CreateTask(projectId, task);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);

    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskById(Guid id)
    {
        var response = await _service.GetTaskById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, UpdateTaskDto task)
    {
        if (task == null) return BadRequest("required body parameter is null");

        var response = await _service.UpdateTask(id, task);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchTask(Guid id, UpdateTaskDto task)
    {
        if (task == null) return BadRequest("required body parameter is null");

        var response = await _service.PatchTask(id, task);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var response = await _service.DeleteTask(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }


}