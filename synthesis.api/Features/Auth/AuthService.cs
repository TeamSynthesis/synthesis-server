using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using synthesis.api.Services.Email;
using Synthesis.Api.Services.BlobStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace synthesis.api.Features.Auth;

public interface IAuthService
{
    Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand, HttpRequest request);
    Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand);
    Task<GlobalResponse<LoginResponseDto>> GitHubLogin(string access_token);
    Task<GlobalResponse<string>> ConfirmEmail(Guid userId, string token);
}

[AllowAnonymous]
public class AuthService : IAuthService
{
    private readonly RepositoryContext _repository;
    private readonly IHttpClientFactory _httpClient;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly IJwtTokenManager _jwtManager;
    private readonly R2CloudStorage _r2Cloud;
    private readonly IEmailService _emailService;

    public AuthService(RepositoryContext repository, IMapper mapper, IPasswordHasher<UserModel> passwordHasher, IJwtTokenManager jwtManager, IHttpClientFactory httpClient, R2CloudStorage r2Cloud, IEmailService emailService)
    {
        _repository = repository;
        _httpClient = httpClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
        _r2Cloud = r2Cloud;
        _emailService = emailService;
    }

    public async Task<GlobalResponse<LoginResponseDto>> GitHubLogin(string accessToken)
    {
        using var httpClient = _httpClient.CreateClient("GitHub");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "synthesis.api/1.0");

        var userUrl = "https://api.github.com/user";

        var userResponse = await httpClient.GetAsync(userUrl);

        if (!userResponse.IsSuccessStatusCode)
        {
            return new GlobalResponse<LoginResponseDto>(false, "login failed", errors: ["Failed to fetch user data from GitHub API."]);
        }

        var githubUser = await userResponse.Content.ReadFromJsonAsync<GitHubUserDto>();

        var userExists = await _repository.Users.AnyAsync(u => u.GitHubId == githubUser.id);

        if (userExists)
        {
            var user = await _repository.Users.Where(u => u.GitHubId == githubUser.id).Select(x => new UserModel
            {
                Id = x.Id,
                UserName = x.UserName

            }).SingleOrDefaultAsync();

            var token = _jwtManager.GenerateToken(user);

            var existingUserReponse = new LoginResponseDto(token, user.Id.ToString());

            return new GlobalResponse<LoginResponseDto>(true, "Login successful", value: existingUserReponse);
        }

        var emailUrl = "https://api.github.com/user/emails";

        var emailResponse = await httpClient.GetAsync(emailUrl);

        if (!emailResponse.IsSuccessStatusCode)
        {
            return new GlobalResponse<LoginResponseDto>(false, "login failed", errors: ["Failed to fetch user data from GitHub API."]);

        }

        var emails = await emailResponse.Content.ReadFromJsonAsync<List<GitHubEmailDto>>();

        if (emails == null)
        {
            return new GlobalResponse<LoginResponseDto>(false, "login failed", errors: ["Failed to fetch user data from GitHub API."]);

        }


        var newUser = new UserModel
        {
            Email = FindPrimaryEmail(emails),
            GitHubId = githubUser.id,
            AvatarUrl = githubUser.avatar_url,
            EmailConfirmed = true,
            OnBoardingProgress = OnBoardingProgress.CreateAccount
        };
        await _repository.Users.AddAsync(newUser);
        await _repository.SaveChangesAsync();


        var jwtToken = _jwtManager.GenerateToken(newUser);

        var response = new LoginResponseDto(jwtToken, newUser.Id.ToString());

        return new GlobalResponse<LoginResponseDto>(true, "Login successful", value: response);

    }

    private string FindPrimaryEmail(List<GitHubEmailDto> emails)
    {
        if (emails == null)
        {
            return null;
        }

        var primary = emails.FirstOrDefault(e => e.primary == true && e.verified == true);
        return primary?.email ?? emails.FirstOrDefault(e => e.verified == true)?.email;
    }

    public async Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand)
    {
        var user = await _repository.Users
        .Where(u => u.UserName.ToLower() == loginCommand.UsernameEmail.ToLower() || u.Email.ToLower() == loginCommand.UsernameEmail.ToLower())
        .Select(x => new UserModel() { Id = x.Id, UserName = x.UserName, Email = x.Email, PasswordHash = x.PasswordHash })
        .FirstOrDefaultAsync();

        if (user == null)
        {
            return new GlobalResponse<LoginResponseDto>(false, "login failed", errors: ["bad credentials"]);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginCommand.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return new GlobalResponse<LoginResponseDto>(false, "login failed", errors: ["bad credentials"]);
        }

        var token = _jwtManager.GenerateToken(user);

        var loginResponse = new LoginResponseDto(token, user.Id.ToString());

        return new GlobalResponse<LoginResponseDto>(true, "login success", value: loginResponse);

    }

    public async Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand, HttpRequest request)
    {
        var isEmailTaken = await _repository.Users.AnyAsync(u => u.Email == registerCommand.Email);
        if (isEmailTaken)
        {
            return new GlobalResponse<RegisterResponseDto>(false, "register user failed", errors: ["email is registered to another user"]);
        }

        var validationResult = new AuthValidator().Validate(registerCommand);

        if (!validationResult.IsValid)
        {
            return new GlobalResponse<RegisterResponseDto>(false, "register user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = new UserModel
        {
            CreatedOn = DateTime.UtcNow,
            Email = registerCommand.Email,
            OnBoardingProgress = OnBoardingProgress.CreateAccount,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerCommand.Password);

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        await SendConfirmationEmail(user, request);

        var token = _jwtManager.GenerateToken(user);
        var userToReturn = _mapper.Map<UserDto>(user);

        var response = new RegisterResponseDto(token, userToReturn);

        return new GlobalResponse<RegisterResponseDto>(true, "register user success", value: response);
    }

    public async Task SendConfirmationEmail(UserModel user, HttpRequest request)
    {
        var code = _jwtManager.GenerateEmailConfirmationToken(user);

        var path = request.Scheme + "://" + request.Host + "/api/auth/confirm-email";
        var link = $"{path}?userId={user.Id}&code={code}";

        var response = await _emailService.SendConfirmationEmail(link, user.Email);

        if (!response.IsSuccess)
        {
            throw new Exception("Failed to send confirmation email");
        }
    }

    public async Task<GlobalResponse<string>> ConfirmEmail(Guid userId, string token)
    {
        var user = await _repository.Users.FindAsync(userId);

        if (user == null)
        {
            return new GlobalResponse<string>(false, "confirm email failed", errors: ["user not found"]);
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenData = tokenHandler.ReadJwtToken(token);

        var email = tokenData.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email != user.Email)
        {
            return new GlobalResponse<string>(false, "confirm email failed", errors: ["invalid token"]);
        }

        user.EmailConfirmed = true;

        await _repository.SaveChangesAsync();

        return new GlobalResponse<string>(true, "confirm email success", value: "email confirmed");
    }
}