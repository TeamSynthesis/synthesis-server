using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly RepositoryContext _repository;
        public AuthController(IAuthService service, RepositoryContext repository)
        {
            _service = service;
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDto registerCommand)
        {
            if (registerCommand == null)
                return BadRequest("required body param is null");

            var response = await _service.Register(registerCommand, Request);

            if (!response.IsSuccess)
                return BadRequest(response);


            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginUserDto user)
        {
            if (user == null)
                return BadRequest("required body param is null");

            var response = await _service.Login(user);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpPost("github")]
        public async Task<IActionResult> GithubLogin(string accessToken)
        {
            if (accessToken == null) return BadRequest("required  param is null");

            var response = await _service.GitHubLogin(accessToken);

            if (!response.IsSuccess) return BadRequest(response);

            return Ok(response);

        }


        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
        {
            if (userId == Guid.Empty || code == null)
                return BadRequest("required query params are null");

            var response = await _service.ConfirmEmail(userId, code);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Redirect("https://www.google.com");
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return Ok("remove the token from your locals");
        }
    }
}
