using System.Text;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Auth.Commands.Login;
using EmployeeManagementSystem.Application.Features.Auth.Commands.Register;
using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using EmployeeManagementSystem.Infrastructure.Persistence;
using EmployeeManagementSystem.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Database & AppDbContext Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// 2. Settings Registration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 3. Infrastructure Custom Security Services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// 4. Controllers & MediatR Setup
builder.Services.AddControllers();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// 5. Manual Handlers & Validators Registration
// Register Feature
builder.Services.AddTransient<IRequestHandler<RegisterCommand, AuthResponseDto>, RegisterCommandHandler>();
builder.Services.AddTransient<IValidator<RegisterCommand>, RegisterCommandValidator>();

// Login Feature
builder.Services.AddTransient<IRequestHandler<LoginCommand, AuthResponseDto>, LoginCommandHandler>();
builder.Services.AddTransient<IValidator<LoginCommand>, LoginCommandValidator>();

// 6. Safe Configuration for JWT Authentication
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettingsSection["Secret"] ?? "SUPER_SECRET_KEY_THAT_IS_AT_LEAST_32_BYTES_LONG_12345!";
var issuer = jwtSettingsSection["Issuer"] ?? "EmployeeManagementSystem";
var audience = jwtSettingsSection["Audience"] ?? "EmployeeManagementSystemUser";

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 7. OpenAPI / Swagger Setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Map Scalar UI
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Employee Management API")
               .WithTheme(ScalarTheme.Moon)
               .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();