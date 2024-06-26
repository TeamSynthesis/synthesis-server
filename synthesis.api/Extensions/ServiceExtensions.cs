using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Auth;
using System.Text;

public static class ServiceExtensions
{
    public static void ConfigurePostgresContext(this IServiceCollection services, IConfiguration configuration)
     => services.AddDbContext<RepositoryContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgresCloud")));

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                SaveSigninToken = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetSection("JwtConfig:Issuer").Value,
                ValidAudience = configuration.GetSection("JwtConfig:Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(configuration.GetSection("JwtConfig:Secret").Value))
            };
        });
        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
    }


}