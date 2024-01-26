using System.ComponentModel.Design.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace synthesis.api.Features.Organisation;


[ApiController]
[Route("api/[controller]")]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationService _service;

    public OrganisationController(IOrganisationService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrganisation(Guid userId, [FromBody] CreateOrganisationDto organisation)
    {
        var response = await _service.CreateOrganisation(userId, organisation);

        if (!response.IsSuccess) return BadRequest(response);

        return Created("", value: response);
    }
}