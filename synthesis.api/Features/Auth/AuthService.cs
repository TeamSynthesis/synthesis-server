using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using Synthesis.Api.Services.BlobStorage;
using System.Net.Http.Headers;

namespace synthesis.api.Features.Auth;

public interface IAuthService
{
    Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand);
    Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand);
    Task<GlobalResponse<LoginResponseDto>> GitHubLogin(string access_token);
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

    public AuthService(RepositoryContext repository, IMapper mapper, IPasswordHasher<UserModel> passwordHasher, IJwtTokenManager jwtManager, IHttpClientFactory httpClient, R2CloudStorage r2Cloud)
    {
        _repository = repository;
        _httpClient = httpClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
        _r2Cloud = r2Cloud;
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
            UserName = "gh_" + githubUser.login,
            Email = FindPrimaryEmail(emails),
            GitHubId = githubUser.id,
            AvatarUrl = githubUser.avatar_url,
            EmailConfirmed = true,
            OnBoarding = OnBoardingProgress.CreateAccount
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
        .Where(u => u.UserName.ToLower() == loginCommand.UsernameEmail.ToLower()
        || u.Email.ToLower() == loginCommand.UsernameEmail.ToLower())
        .Select(x => new UserModel() { Id = x.Id, UserName = x.UserName, PasswordHash = x.PasswordHash })
        .SingleOrDefaultAsync();

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

    public async Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand)
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
            Email = registerCommand.Email,
            OnBoarding = OnBoardingProgress.CreateAccount,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, registerCommand.Password);

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        var token = _jwtManager.GenerateToken(user);
        var userToReturn = _mapper.Map<UserDto>(user);

        var response = new RegisterResponseDto(token, userToReturn);

        return new GlobalResponse<RegisterResponseDto>(true, "register user success", value: response);
    }
}