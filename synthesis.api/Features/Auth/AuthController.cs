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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto user)
        {
            if (user == null)
                return BadRequest("required body param is null");

            var response = await _service.Login(user);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }
        

        [HttpGet("github")]
        public IActionResult GithubLogin()
        {
            var origin = Request.Headers.Origin.ToString();
            var authProps = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth")
            };
            authProps.Items.Add("origin", origin);
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

            var origin = authenticationResult.Properties.Items["origin"];

            if (!response.IsSuccess) return Redirect($"{origin}/auth/sign-up?error={response.Message}");

            return Redirect($"{origin}/account/auth?token={response.Data.Token} & userId ={response.Data.User.Id}");

        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }


    }
}
