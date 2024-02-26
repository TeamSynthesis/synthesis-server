using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerCommand, IFormFile avatar)
        {
            if (registerCommand == null)
                return BadRequest("required body param is null");

            var response = await _service.Register(registerCommand);

            if (!response.IsSuccess)
                return BadRequest(response);


            return Ok(response);
        }


        [HttpGet("github")]
        public IActionResult GithubLogin()
        {
            var authProps = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth")
            };
            return Challenge(authProps, "GitHub");

        }


        [HttpGet("github/callback")]
        public async Task<IActionResult> Callback()
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Cookies");

            if (!authenticationResult.Succeeded)
            {
                return BadRequest("Authentication failed");
            }
            var accessToken = authenticationResult.Properties.GetTokenValue("access_token");

            var response = await _service.GitHubLogin(accessToken);

            if (!response.IsSuccess) return BadRequest(response);

            return Ok(response.Data);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }


    }
}
