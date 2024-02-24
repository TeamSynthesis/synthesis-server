using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

namespace synthesis.api.Features.Auth;

public interface IAuthService
{
    Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand);
    Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand);
}

public class AuthService : IAuthService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<UserModel> _passwordHasher;
    private readonly IJwtTokenManager _jwtManager;


    public AuthService(RepositoryContext repository, IMapper mapper, IPasswordHasher<UserModel> passwordHasher, IJwtTokenManager jwtManager)
    {
        _repository = repository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtManager = jwtManager;
    }

    public Task<GlobalResponse<LoginResponseDto>> Login(LoginUserDto loginCommand)
    {
        throw new NotImplementedException();
    }

    public async Task<GlobalResponse<RegisterResponseDto>> Register(RegisterUserDto registerCommand)
    {
        var userExists = await _repository.Users.AnyAsync(u => u.Email == registerCommand.Email);

        // var validationResult = new AuthValidator(_repository, registerCommand).Validate(registerCommand);

        // if (!validationResult.IsValid)
        // {
        //     return new GlobalResponse<RegisterResponseDto>(false, "register user failed", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        // }

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