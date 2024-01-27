using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Member;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{

    private readonly IMemberService _service;

    public MembersController(IMemberService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMemberProfile(Guid organisationId, Guid userId)
    {
        var response = await _service.CreateMemberProfile(organisationId, userId);

        if (!response.IsSuccess) return BadRequest(response);

        return Created("", value: response);
    }
}