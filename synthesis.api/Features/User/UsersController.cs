using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.User;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id:guid}", Name = "UserById")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _service.GetUserById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserDto user)
    {
        if (user == null) return BadRequest("required body param is null");
        var response = await _service.UpdateUser(id, user);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchUser(Guid id, [FromForm] UpdateUserDto user)
    {
        var response = await _service.PatchUser(id, user);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("{id:guid}/details")]
    public async Task<IActionResult> PostUserDetails(Guid id, [FromForm] PostUserDetailsDto userDetails)
    {
        if (userDetails == null) return BadRequest("required body param is null");
        var response = await _service.PostUserDetails(id, userDetails);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("{id:guid}/skills")]
    public async Task<IActionResult> PostUserSkills(Guid id, [FromForm] List<string> skills)
    {
        if (skills == null) return BadRequest("required body param is null");
        var response = await _service.PostUserSkills(id, skills);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _service.DeleteUser(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);

    }

    [HttpPut("{id:guid}/change-avatar")]
    public async Task<IActionResult> ChangeAvatar(Guid id, [FromForm] IFormFile avatar)
    {
        if (avatar == null) return BadRequest("required body param is null");
        var response = await _service.ChangeAvatar(id, avatar);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }
}