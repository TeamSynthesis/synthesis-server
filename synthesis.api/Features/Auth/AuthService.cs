using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;
using System.Net.Http.Headers;

namespace synthesis.api.Features.Auth;

public interface IAuthService
{
    Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand);
    Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand);
    Task<GlobalResponse<LoginResponseDto>> GitHubLogin(string access_token);
}

public class AuthService : IAuthService
{
    private readonly RepositoryContext _repository;
    private readonly IHttpClientFactory _httpClient;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly IJwtTokenManager _jwtManager;


    public AuthService(RepositoryContext repository, IMapper mapper, IPasswordHasher<UserModel> passwordHasher, IJwtTokenManager jwtManager, IHttpClientFactory httpClient)
    {
        _repository = repository;
        _httpClient = httpClient;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
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
            var user = await _repository.Users.Where(u => u.GitHubId == githubUser.id).Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.UserName,
                Email = x.Email,
                AvatarUrl = x.AvatarUrl,
                OnBoarding = x.OnBoardingProgress.GetDisplayName()

            }).SingleOrDefaultAsync();

            var userCredentials = _mapper.Map<UserModel>(user);
            var token = _jwtManager.GenerateToken(userCredentials);

            var existingUserReponse = new LoginResponseDto(token, user);

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
            OnBoardingProgress = OnBoardingProgress.CreateAccount
        };
        await _repository.Users.AddAsync(newUser);
        await _repository.SaveChangesAsync();


        var jwtToken = _jwtManager.GenerateToken(newUser);

        var userToReturn = new UserDto()
        {
            Id = newUser.Id,
            Username = newUser.UserName,
            Email = newUser.Email,
            AvatarUrl = newUser.AvatarUrl,
            OnBoarding = newUser.OnBoardingProgress.GetDisplayName()
        };

        var response = new LoginResponseDto(jwtToken, userToReturn);

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
        var user = await _repository.Users.Where(u => u.UserName.ToLower() == loginCommand.UsernameEmail.ToLower() || u.Email.ToLower() == loginCommand.UsernameEmail.ToLower()).SingleOrDefaultAsync();

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
        var userToReturn = _mapper.Map<UserDto>(user);

        var loginResponse = new LoginResponseDto(token, userToReturn);

        return new GlobalResponse<LoginResponseDto>(true, "login success", value: loginResponse);

    }

    public async Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand)
    {

        var isUsernameTaken = await _repository.Users.AnyAsync(u => u.UserName.ToLower() == registerCommand.Username.ToLower());
        if (isUsernameTaken)
        {
            return new GlobalResponse<RegisterResponseDto>(false, "register user failed ", errors: ["username is already taken "]);
        }

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
            UserName = registerCommand.Username,
            Email = registerCommand.Email,
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