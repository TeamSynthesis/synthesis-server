using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Scrutor;
using synthesis.api.Data.Models;
using synthesis.api.Exceptions;
using synthesis.api.Features.Auth;
using Synthesis.Api.Services.BlobStorage;
using System.Text.Json.Serialization;
using Microsoft.Net.Http.Headers;
using synthesis.api.Services.Notifications;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigurePostgresContext(builder.Configuration);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddSingleton<IPasswordHasher<UserModel>, PasswordHasher<UserModel>>();

builder.Services.AddSingleton<R2CloudStorage>();
builder.Services.AddSingleton<JwtConfig>();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.Scan(x =>
    x.FromAssemblies(typeof(Program).Assembly)
    .AddClasses()
    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
    .AsMatchingInterface()
    .WithScopedLifetime()
);

builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddControllers()
.AddJsonOptions(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

app.UseCors(p => { p.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod(); });

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapHub<NotificationHub>("/notifications");

app.MapControllers().RequireAuthorization();

app.UseExceptionHandler(opt => { });

app.Run();
