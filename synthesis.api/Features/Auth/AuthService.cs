
namespace synthesis.api.Features.Auth;

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
        var validationResult = new RegisterUserDtoValidator().Validate(registerRequest);


        if (!validationResult.IsValid)
        {
            return new Response<UserDto>(false, "failed to register user", errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = _mapper.Map<UserModel>(registerRequest);

        await _repository.Users.AddAsync(user);
        await _repository.SaveChangesAsync();

        return new Response<UserDto>(true, "user registered successfully");

    }
}