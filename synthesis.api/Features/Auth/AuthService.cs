using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public async Task<GlobalResponse<LoginResponseDto>> GitHubLogin(string access_token)
    {
        using var httpClient = _httpClient.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "synthesis.api/1.0");

        var userUrl = "https://api.github.com/user";
        var userResponse = await httpClient.GetAsync(userUrl);
        if (!userResponse.IsSuccessStatusCode)
        {
            return new GlobalResponse<LoginResponseDto>(false, "error", errors: ["something went wrong"]);
        }
        var githubUser = await userResponse.Content.ReadFromJsonAsync<GitHubUserDto>();

        var emailUrl = "https://api.github.com/user/emails";
        var emailResponse = await httpClient.GetAsync(emailUrl);
        if (!emailResponse.IsSuccessStatusCode)
        {
            return new GlobalResponse<LoginResponseDto>(false, "error", errors: ["something went wrong"]);
        }
        var emails = await emailResponse.Content.ReadFromJsonAsync<List<GitHubEmailDto>>();

        var existingUser = await _repository.Users.AnyAsync(u => u.GitHubId == githubUser.id);

        UserModel user;
        if (!existingUser)
        {
            user = new UserModel
            {
                UserName = githubUser.name,
                Email = FindPrimaryEmail(emails),
                AvatarUrl = githubUser.avatar_url,
                EmailConfirmed = true,
                OnBoardingProgress = OnBoardingProgress.CreateAccount
            };
            await _repository.Users.AddAsync(user);
            await _repository.SaveChangesAsync();
        }
        else
        {
            user = await _repository.Users.FirstAsync(u => u.GitHubId == githubUser.id);
        }

        var jwtToken = _jwtManager.GenerateToken(user);

        var response = new LoginResponseDto(jwtToken);

        return new GlobalResponse<LoginResponseDto>(true, "login success", value: response);
    }
    private string FindPrimaryEmail(List<GitHubEmailDto> emails)
    {
        if (emails == null)
        {
            return null;
        }

        var primary = emails.FirstOrDefault(e => e.primary == true && e.verified == true);
        return primary.email != null ? primary.email : emails.FirstOrDefault(e => e.verified == true).email;
    }

    public Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand)
    {
        throw new NotImplementedException();
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