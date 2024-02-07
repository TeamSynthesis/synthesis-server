using Microsoft.AspNetCore.Mvc;
using synthesis.api.Mappings;

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


    [HttpGet("{id:guid}", Name = "MemberProfileById")]
    public async Task<IActionResult> GetMemberProfileById(Guid id)
    {
        var response = await _service.GetMemberProfileById(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);

    }

    [HttpPost("{id:guid}/assign-role")]
    public async Task<IActionResult> AssignMemberRole(Guid id, [FromBody] string role)
    {
        var response = await _service.AssignMemberRole(id, role.ToLower());

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:guid}/resign-role")]
    public async Task<IActionResult> ResignMemberRole(Guid id, [FromBody] string role)
    {
        var response = await _service.ResignMemberRole(id, role.ToLower());

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMemberProfile(Guid id)
    {
        var response = await _service.DeleteMemberProfile(id);

        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);
    }
}