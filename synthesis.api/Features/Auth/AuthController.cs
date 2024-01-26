using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;
    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto user)
    {
        var response = await _service.RegisterUser(user);

        if (!response.IsSuccess) return BadRequest(response);

        return Created("",value:response);
    }
}