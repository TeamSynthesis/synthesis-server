using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Auth;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services) =>
    services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        }
    );

    public static void ConfigurePostgresContext(this IServiceCollection services, IConfiguration configuration)
     => services.AddDbContext<RepositoryContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgresLocal")));

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetSection("JwtConfig:Issuer").Value,
                ValidAudience = configuration.GetSection("JwtConfig:Issuer").Value,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(configuration.GetSection("JwtConfig:Secret").Value))
            };
        })

         .AddGitHub(options =>
        {
            options.ClientId = "846d55a778c8a634c4e7";
            options.ClientSecret = "05ff1507f36557dca5ee592b7307c5c280967bd1";
            options.CallbackPath = "/api/oauth/github-cb";
            options.SaveTokens = true;
        });

        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
    }
}