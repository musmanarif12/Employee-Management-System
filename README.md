# Employee Management System (EMS)

A secure, high-performance Employee Management System backend built with .NET 9 following Clean Architecture principles, CQRS (Command Query Responsibility Segregation) pattern with MediatR, and Entity Framework Core 9.

---

## Key Features

* Authentication & Role-Based Access Control (RBAC):
  * Secure JWT (JSON Web Token) authentication.
  * Hierarchical claim-based authorization supporting roles: CEO, COO, HR, Project Manager, and Employee.

* Field-Level Authorization & Employee Profiles:
  * 1-to-1 Profile Extension: Segregated personal details and sensitive corporate data into an EmployeeProfile model.
  * Self-Service Portal: Employees can update their own contact information (Phone, Address).
  * Restricted HR/CEO Controls: Sensitive fields (Salary, Designation, Department) are protected and can only be updated/modified by authorized HR or CEO roles.
  * Automatic Upsert Logic: Profiles are seamlessly created on initial update requests.

* Hierarchical Leave Management System:
  * Automated leave application workflows with manager/reporting hierarchy approvals.
  * Real-time leave balance/quota calculation and restoration upon rejection or cancellation.

* Administrative Personnel Firing & Audit:
  * Exclusive firing authority for CEO role.
  * Built-in security guards preventing self-termination and unauthorized hierarchy operations.
  * Historical audit logging for terminated personnel.

* Interactive API Documentation:
  * Fully configured Scalar UI / OpenAPI Reference for interactive API exploration and testing.

---

## Tech Stack & Tools

* Framework: .NET 9 Web API
* Architecture: Clean Architecture (Domain, Application, Infrastructure, API)
* Design Patterns: CQRS using MediatR, Repository Pattern
* Database & ORM: SQL Server / EF Core 9 (Code-First Migrations, Fluent API Mapping)
* Security: JWT Bearer Authentication, Password Hashing
* API Documentation: Scalar API Reference / OpenAPI

---

## Project Architecture Overview

```text
src/
├── Domain/                 # Enterprise Entities (User, EmployeeProfile, BaseEntity, etc.)
├── Application/            # CQRS Features, DTOs, Handlers, Interfaces, Business Logic
├── Infrastructure/         # Persistence (AppDbContext), Migrations, Services
└── API/                    # Controllers, Middlewares, Program.cs Configuration

Getting Started
Prerequisites

    .NET 9 SDK

    SQL Server (LocalDB or Enterprise instance)

    Visual Studio 2022 / Antigravity IDE / VS Code

Database Setup

Execute the following Entity Framework Core CLI command to apply migrations and build the database schema:
dotnet ef database update --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.API

Running the Project

dotnet run --project EmployeeManagementSystem.API

Navigate to https://localhost:<port>/scalar/v1 in your browser to test endpoints via Scalar UI.
