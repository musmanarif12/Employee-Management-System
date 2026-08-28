using EmployeeManagementSystem.Application.Features.Auth.Commands.Register;
using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using EmployeeManagementSystem.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext with SQL Server Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add Controllers & MediatR Core
builder.Services.AddControllers();

builder.Services.AddMediatR(cfg =>
{
    // Empty config for manual handler registration
});

// 3. Manual Registration for Register Feature
builder.Services.AddTransient<IRequestHandler<RegisterCommand, AuthResponseDto>, RegisterCommandHandler>();
builder.Services.AddTransient<IValidator<RegisterCommand>, RegisterCommandValidator>();

// 4. API Documentation (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();