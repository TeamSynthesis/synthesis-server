using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace synthesis.api.Features.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("github")]
        public IActionResult SignInGithub()
        {
            // Redirect to GitHub for authentication
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Login), "Auth")
            };

            return Challenge(authenticationProperties, "GitHub");
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            // Read the result from the external authentication provider
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Succeeded != true)
            {
                return BadRequest("External authentication error");
            }

            // Get the access token
            var accessToken = Request.Cookies["access_token"];

            // Use the access token with Octokit
            var github = new GitHubClient(new ProductHeaderValue("Synthesis"))
            {
                Credentials = new Credentials(accessToken)
            };

            // Get the user's information
            var user = await github.User.Current();

            // Here, you would typically look up the user in your database using the GitHub ID
            // If the user doesn't exist yet, you would create a new user

            // For now, let's just return the user's information
            return Ok(new { user.Id, user.Name, user.Email });
        }


        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
