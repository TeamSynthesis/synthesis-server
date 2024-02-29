using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using synthesis.api.Data.Repository;
using synthesis.api.Features.Auth;
using System.Text;

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
     => services.AddDbContext<RepositoryContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgresCloud")));

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
            options.DefaultSignInScheme = "Cookies";
        })
        .AddCookie("Cookies", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
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
        })
        .AddGitHub(options =>
        {
            options.ClientId = "846d55a778c8a634c4e7";
            options.ClientSecret = "05ff1507f36557dca5ee592b7307c5c280967bd1";
            options.CallbackPath = "/api/auth/github/callback";
            options.SaveTokens = true;
            options.Scope.Add("user:email");
            options.Scope.Add("read:user");
        });

        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
    }

    //options.ClientId = "Iv1.07ed0182751867d4";
    //options.ClientSecret = "6cf27ae148402fb13b65f6d704def276ab3a628f";
    //options.CallbackPath = "/api/oauth/github-cb";
}