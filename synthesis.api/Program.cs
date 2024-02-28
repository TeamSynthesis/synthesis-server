using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Scrutor;
using synthesis.api.Data.Models;
using synthesis.api.Exceptions;
using synthesis.api.Features.Auth;
using Synthesis.Api.Services.BlobStorage;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.ConfigureCors();
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

builder.Services.AddHttpClient();
builder.Services.AddControllers()
.AddJsonOptions(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.UseExceptionHandler(opt => { });

app.Run();
