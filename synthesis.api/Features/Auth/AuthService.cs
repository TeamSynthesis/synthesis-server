
namespace synthesis.api.Features.Auth;

using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Razor.TagHelpers;
using synthesis.api.Data.Models;
using synthesis.api.Data.Repository;
using synthesis.api.Features.User;
using synthesis.api.Mappings;

public interface IAuthService
{
    Task<Response<UserDto>> RegisterUser(RegisterUserDto registerRequest);
}

public class AuthService : IAuthService
{
    private readonly RepositoryContext _repository;
    private readonly IMapper _mapper;
    public AuthService(RepositoryContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Response<UserDto>> RegisterUser(RegisterUserDto registerRequest)
    {

        var user = _mapper.Map<UserModel>(registerRequest);

        var validationResult = new UserValidator().Validate(user);

        if (!validationResult.IsValid)
        {
            return new Response<UserDto>(false, "failed to register user", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        var userToReturn = _mapper.Map<UserDto>(user);

        return new Response<UserDto>(true, "user registered successfully", value: userToReturn);

    }
}