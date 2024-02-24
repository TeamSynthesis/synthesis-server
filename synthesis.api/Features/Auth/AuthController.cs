using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using Octokit;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;

namespace synthesis.api.Features.Auth
{
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
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerCommand)
        {
            if (registerCommand == null)
                return BadRequest("required body param is null");

            var response = await _service.Register(registerCommand);

            if (!response.IsSuccess)
                return BadRequest(response);


            return Ok(response);
        }


        [HttpGet("github")]
        public IActionResult GithubAuth()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(SignInGithub), "Auth")
            };

            return Challenge(authenticationProperties, "GitHub");
        }

        [Authorize]
        [HttpGet("github/sign-in")]
        public async Task<IActionResult> SignInGithub()
        {

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Succeeded != true)
            {
                return BadRequest("External authentication error");
            }
            var accessToken = result.Properties.GetTokenValue("access_token");

            // Use the access token with Octokit
            var github = new GitHubClient(new ProductHeaderValue("Synthesis"))
            {
                Credentials = new Credentials(accessToken)
            };

            // Get the user's information
            var githubUser = await github.User.Current();

            var githubEmails = await github.User.Email.GetAll();

            // var userExists = await _repository.Users.AnyAsync(u => u.GitHubId == githubUser.Id.ToString());

            // if (!userExists)
            // {
            //     var user = new UserModel()
            //     {
            //         UserName = githubUser.Name,
            //         Email = githubEmails.First().Email,
            //         AvatarUrl = githubUser.AvatarUrl,
            //         EmailConfirmed = true
            //     };

            //     await _repository.Users.AddAsync(user);
            //     await _repository.SaveChangesAsync();
            // }

            return Ok(githubUser.Bio);

        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }


    }
}
