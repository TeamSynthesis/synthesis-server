using Microsoft.EntityFrameworkCore;
using synthesis.api.Data.Repository;

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

}