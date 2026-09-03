using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Application.Features.Attendance.Commands;
using EmployeeManagementSystem.Application.Features.Attendance.Handlers;
using EmployeeManagementSystem.Application.Features.Attendance.Queries;
using EmployeeManagementSystem.Application.Features.Auth.Commands.Login;
using EmployeeManagementSystem.Application.Features.Auth.Commands.Register;
using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using EmployeeManagementSystem.Application.Features.Employees.Commands.FireEmployee;
using EmployeeManagementSystem.Application.Features.Employees.Commands.UpdateProfile;
using EmployeeManagementSystem.Application.Features.Employees.Queries.GetFiredEmployees;
using EmployeeManagementSystem.Application.Features.Leaves.Commands.ApplyLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Commands.ReviewLeave;
using EmployeeManagementSystem.Application.Features.Leaves.Dtos;
using EmployeeManagementSystem.Application.Features.Leaves.Queries.GetEmployeeLeaves;
using EmployeeManagementSystem.Application.Features.Leaves.Queries.GetManagerLeaves;
using EmployeeManagementSystem.Infrastructure.Persistence;
using EmployeeManagementSystem.Infrastructure.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System;
using System.Collections.Generic;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 0. CORS Policy Configuration
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // React Vite & CRA origins
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

// Apply Leave
builder.Services.AddTransient<IRequestHandler<ApplyLeaveCommand, string>, ApplyLeaveCommandHandler>();
builder.Services.AddTransient<IValidator<ApplyLeaveCommand>, ApplyLeaveCommandValidator>();

// Review Leave (Manager Action)
builder.Services.AddTransient<IRequestHandler<ReviewLeaveCommand, string>, ReviewLeaveCommandHandler>();

// Get Leaves (Employee Status View)
builder.Services.AddTransient<IRequestHandler<GetEmployeeLeavesQuery, List<LeaveResponseDto>>, GetEmployeeLeavesQueryHandler>();

// Get Leave (ProjectManager Status View)
builder.Services.AddTransient<IRequestHandler<GetManagerLeavesQuery, List<LeaveResponseDto>>, GetManagerLeavesQueryHandler>();

// Fire Employee & View Fired Employees Handlers
builder.Services.AddTransient<IRequestHandler<FireEmployeeCommand, string>, FireEmployeeCommandHandler>();
builder.Services.AddTransient<IRequestHandler<GetFiredEmployeesQuery, List<FiredEmployeeDto>>, GetFiredEmployeesQueryHandler>();

// Profile Management Handlers
builder.Services.AddTransient<IRequestHandler<UpdateSelfProfileCommand, string>, UpdateSelfProfileCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateEmployeeByHrCommand, string>, UpdateEmployeeByHrCommandHandler>();

// Attendance Handlers
builder.Services.AddTransient<IRequestHandler<AddAttendanceCommand, string>, AddAttendanceCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateAttendanceByHrCommand, string>, UpdateAttendanceByHrCommandHandler>();
builder.Services.AddTransient<IRequestHandler<GetAllAttendanceForHrQuery, List<AttendanceResponseDto>>, GetAllAttendanceForHrQueryHandler>();

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

// 7. OpenAPI / Swagger Setup with Security Definition for Scalar UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT Bearer token only. Example: eyJhbGciOi...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ----------------------------------------------------
// IMPORTANT PIPELINE ORDER FOR CORS & AUTH
// ----------------------------------------------------

// 1. MUST BE FIRST: UseCors is top-priority so OPTIONS preflights pass immediately
app.UseCors("AllowReactApp");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Employee Management API")
               .WithTheme(ScalarTheme.Moon)
               .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json");
    });
}

// Disable or keep HttpsRedirection after CORS
// app.UseHttpsRedirection(); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();