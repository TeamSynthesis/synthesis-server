using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using synthesis.api.Data.Models;

namespace synthesis.api.Features.Auth
{
    public interface IJwtTokenManager
    {
        string GenerateToken(UserModel user);
        string GenerateEmailConfirmationToken(UserModel user);
    }

    public class JwtTokenManager : IJwtTokenManager
    {
        private readonly IConfiguration _config;

        public JwtTokenManager(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtConfig:Secret").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config.GetSection("JwtConfig:Issuer").Value,
                audience: _config.GetSection("JwtConfig:Audience").Value,
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateEmailConfirmationToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtConfig:Secret").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}