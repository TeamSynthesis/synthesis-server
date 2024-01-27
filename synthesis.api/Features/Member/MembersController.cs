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

    [HttpPost]
    public async Task<IActionResult> CreateMemberProfile(Guid organisationId, Guid userId)
    {
        var response = await _service.CreateMemberProfile(organisationId, userId);

        if (!response.IsSuccess) return BadRequest(response);

        return CreatedAtRoute("MemberProfileById", new { id = response.Value.Id }, response.Value);
    }

    [HttpGet("{id:guid}", Name = "MemberProfileById")]
    public async Task<IActionResult> GetMemberProfileById(Guid id)
    {
        var response = await _service.GetMemberProfileById(id);
        if (!response.IsSuccess) return BadRequest(response);

        return Ok(response);

    }
}