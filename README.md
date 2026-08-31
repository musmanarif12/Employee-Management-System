# Employee Management System (.NET 9 - Clean Architecture)

A secure, scalable, and enterprise-ready Employee Management System built using ASP.NET Core (.NET 9) and Clean Architecture. The system enforces organizational hierarchy, dynamic leave quota management, claim-based authentication, and administrative personnel management.

---

## Key Features

### 1. Authentication and Role-Based Authorization
- JWT Bearer Authentication: Secure API access using JWT tokens containing claims (NameIdentifier, Email, Role).
- 4-Level Hierarchy Access Control: Role definitions for CEO (1), COO (2), HR (3), Project Manager (4), and Employee (5).
- Interactive API Documentation: Full OpenAPI / Scalar UI integration with built-in Bearer Token authorization support.

### 2. Hierarchical Leave Management System
- Reporting Hierarchy Enforcement: Project Managers can only view and process leave requests submitted by their direct reportees (ReportToId).
- Dynamic Leave Quota Calculations: Automatically tracks used and remaining leaves based on Approved and Pending requests (5-leave annual limit).
- Quota Restoration: Rejected leave requests automatically restore the employee's available quota.

### 3. Administrative Control and Personnel Firing ("Fire" Feature)
- CEO-Only Authority: Only users authenticated with the CEO role can terminate employees (updates status to IsActive = 0).
- Self-Termination Guard: Built-in security check prevents CEOs from terminating their own accounts.
- Admin Hierarchy Protection: Security guard prevents the termination of other CEOs or Root System Administrators (RoleId == 1).
- Fired Personnel Audit: Restricted endpoint allowing CEOs to view and audit all terminated staff members.

---

## Tech Stack and Architecture

- Framework: ASP.NET Core (.NET 9)
- Architecture Pattern: Clean Architecture (Domain, Application, Infrastructure, API)
- CQRS Pattern: MediatR for Command and Query Segregation
- ORM: Entity Framework Core 9
- Database: SQL Server
- Validation: FluentValidation
- Authentication: JWT (JSON Web Tokens)
- API Documentation: Scalar UI / OpenAPI / Swagger UI

---

## Project Architecture Overview

EmployeeManagementSystem/
├── src/
│   ├── Core/
│   │   ├── Domain/                 # Entities, Enums, Interfaces
│   │   └── Application/            # CQRS Features (Commands, Queries, DTOs, Handlers, Validators)
│   ├── Infrastructure/
│   │   ├── Persistence/            # AppDbContext, Entity Configurations
│   │   └── Security/               # JwtTokenGenerator, PasswordHasher
│   └── Presentation/
│       └── API/                    # Controllers, Program.cs, Middleware

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server

### Installation and Setup

1. Clone the Repository:
   git clone https://github.com/your-username/EmployeeManagementSystem.git
   cd EmployeeManagementSystem

2. Configure Database Connection and JWT Settings:
   Update your appsettings.json in the EmployeeManagementSystem.API project:
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=EmployeeManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "JwtSettings": {
       "Secret": "YOUR_SUPER_SECRET_KEY_THAT_IS_AT_LEAST_32_BYTES_LONG!",
       "Issuer": "EmployeeManagementSystem",
       "Audience": "EmployeeManagementSystemUser",
       "ExpiryMinutes": 60
     }
   }

3. Apply Database Migrations:
   dotnet ef database update --project src/Infrastructure/Persistence --startup-project src/Presentation/API

4. Run the Application:
   dotnet run --project src/Presentation/API

5. Access Scalar UI Documentation:
   http://localhost:60665/scalar/v1

---

## Usage Workflow

1. Login: Send a request to POST /api/Auth/login to obtain your JWT Bearer token.
2. Authorize: Paste the token into the Scalar UI Authorize section (Bearer <token>).
3. Manager Operations: 
   - Fetch team leaves: GET /api/Leaves/manager-pending-leaves
   - Approve/Reject leave: POST /api/Leaves/review
4. CEO Operations:
   - Fire an employee: POST /api/Employees/fire/{employeeId}
   - View fired employees list: GET /api/Employees/fired-list
