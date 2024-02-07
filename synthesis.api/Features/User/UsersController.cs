using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Internal;

namespace synthesis.api.Features.User;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserDto user)
    {
        var response = await _service.RegisterUser(user);
        if (!response.IsSuccess) return BadRequest(response);

        return CreatedAtRoute("UserById", new { id = response.Data.Id }, response.Data);
    }


    [HttpGet("{id:guid}", Name = "UserById")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _service.GetUserById(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto user)
    {
        var response = await _service.UpdateUser(id, user);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchUser(Guid id, [FromBody] UpdateUserDto user)
    {
        var response = await _service.PatchUser(id, user);
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


}