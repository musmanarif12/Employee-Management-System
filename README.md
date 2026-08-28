Employee Management System API

Project Overview
This project is a secure Employee Management System built using .NET 9, Clean Architecture, CQRS pattern with MediatR, FluentValidation, and EF Core 9 with SQL Server.

Architecture Structure

    EmployeeManagementSystem.Domain: Contains domain entities like User, Role, LeaveRequest, and Enums.

    EmployeeManagementSystem.Application: Contains CQRS Commands, Queries, DTOs, Handlers, and FluentValidation rules.

    EmployeeManagementSystem.Infrastructure: Handles EF Core AppDbContext database interactions, PBKDF2 Password Hashing, and JWT Token Generation.

    EmployeeManagementSystem.API: Exposes RESTful endpoints, configures JWT Bearer authentication, and renders Scalar API Reference UI.

Features Implemented

    Authentication Module

    User Registration with password hashing using PBKDF2 (SHA256, 100,000 iterations).

    User Login with credentials verification and JWT token generation.

    Role-based validation during user setup.

    Leave Management Module

    Apply Leave: Employees can submit leave requests with a date and reason.

    Review Leave: Project Managers can approve or reject leave requests with comments.

    Employee Leave Status: Employees can fetch their submitted leaves and view real-time approval or rejection status messages.

    Developer Tooling and Documentation

    Interactive API Documentation powered by Scalar UI (accessible at /scalar/v1).

    Manual Dependency Injection registrations configured in Program.cs.

Database Setup Instructions

    Configure your SQL Server connection string in appsettings.json.

    Open Package Manager Console in Visual Studio.

    Run the following command to create and update the database:
    Update-Database

How to Run the Application

    Press F5 in Visual Studio.

    The application will automatically launch the Scalar API Reference interface.

    Use the Auth endpoints to register or login a user.

    Copy the generated JWT Token to test authenticated requests or use the Leave Management endpoints directly.
