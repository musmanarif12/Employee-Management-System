using EmployeeManagementSystem.Application.Features.Auth.Dtos;
using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IAppDbContext _context;
        private readonly IPasswordHasher _passHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public RegisterCommandHandler(IAppDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var emailExist = await _context.Users
                .AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExist)
                throw new Exception("Email is already Registered.");
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);
            if (role == null)
                throw new Exception("Invalid Role Selected.");

            var hashedPassword = _passHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                RoleId = request.RoleId,
                ReportToId = request.ReportToId,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenGenerator.GenerateToken(user, role.Name);

            return new AuthResponseDto(
                user.Id,
                user.FullName,
                user.Email,
                role.Name,
                token
            );
        }
    }
     
 }
