using EmployeeManagementSystem.Application.Common.Interfaces;
using EmployeeManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EmployeeManagementSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext,IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // EF Core 9 PendingModelChangesWarning ko ignore/suppress karne ke liye
            optionsBuilder.ConfigureWarnings(w =>
                w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "CEO" },
                new Role { Id = 2, Name = "COO" },
                new Role { Id = 3, Name = "HR" },
                new Role { Id = 4, Name = "ProjectManager" },
                new Role { Id = 5, Name = "Employee" }
            );

            modelBuilder.Entity<User>()
                .HasOne(u => u.ReportTo)
                .WithMany()
                .HasForeignKey(u => u.ReportToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmployeeProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<EmployeeProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}