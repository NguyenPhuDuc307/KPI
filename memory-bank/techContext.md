# KPI Management System - Technical Context

## Core Technologies

### Backend Framework

- **ASP.NET Core MVC 8.0**: Primary application framework
- **.NET 8.0**: Runtime and base class libraries
- **C#**: Primary programming language

### Database & ORM

- **SQL Server**: Primary database for production
- **Entity Framework Core 8.0**: Object-Relational Mapper
- **SQLite**: Development database option

### Authentication & Authorization

- **ASP.NET Core Identity**: User management, authentication, and role-based security
- **Custom Authorization Handlers**: Resource-based authorization

### Frontend Technologies

### CSS Frameworks and Libraries

- **Bootstrap 5**: Primary CSS framework used throughout the application
  - Utilizing utility classes (e.g., `d-flex`, `justify-content-between`)
  - Component classes (e.g., `card`, `badge`, `progress`)
  - Grid system for responsive layouts
  - Minimizing custom CSS in favor of Bootstrap's built-in classes

### JavaScript Libraries

- **jQuery**: Used for DOM manipulation and event handling
- **Select2**: Enhanced select boxes with search functionality
  - Configured with Bootstrap 5 theme using `select2-bootstrap-5-theme`
  - Applied to all dropdown selects in forms
  - Customized with consistent styling (width, dropdownParent)

### Font and Icon Libraries

- **Bootstrap Icons**: Primary icon library (`bi-*` classes)
- **Font Awesome**: Secondary icon set (`fas fa-*` classes)

### Responsive Design

- Mobile-first approach using Bootstrap's responsive breakpoints
- Navbar collapses on smaller screens
- Responsive tables and forms
- Media queries for specific customizations
- **Dynamic UI scaling options (75%, 80%, 90%, 100%)**
- **Optimized font sizes and component padding for better space utilization**
- **CSS adjustments for compact layout on all screen sizes**

### UI Components

- Form inputs styled consistently with Bootstrap classes
- Progress bars with contextual colors based on percentage
- Badges for status indicators
- Cards for content organization
- Modals for confirmations and additional information
- **UI Scaling feature for adjustable interface size**
  - Custom CSS variables for scaling control
  - Zoom/transform based scaling mechanism
  - Persistent user preferences via localStorage

### Others

- **AutoMapper**: Object-to-object mapping between entities and view models
- **Serilog**: Structured logging
- **DinkToPdf**: PDF generation for reports
- **EPPlus**: Excel file generation for exports

## Development Environment

### Required Tools

- Visual Studio 2022 or later (preferred) or Visual Studio Code
- .NET 8.0 SDK
- SQL Server Management Studio (optional for database management)
- Git for version control

### Local Setup

1. Clone the repository
2. Run database migrations (`dotnet ef database update`)
3. Configure application settings in `appsettings.Development.json`
4. Run the application (`dotnet run` or via Visual Studio)

### Development Configuration

The development environment is configured with:

- Detailed error pages
- SQL Server or SQLite database (configurable)
- Development email sender (logs emails instead of sending)
- EF Core migrations for database setup
- Seed data for initial testing

## Key NuGet Packages

### Core Dependencies

- `Microsoft.AspNetCore.App`: Core ASP.NET packages
- `Microsoft.EntityFrameworkCore.SqlServer`: SQL Server provider for EF Core
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore`: Identity integration with EF Core
- `AutoMapper.Extensions.Microsoft.DependencyInjection`: AutoMapper integration
- `Serilog.AspNetCore`: Logging integration with ASP.NET Core

### UI & Client-side

- `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`: Runtime compilation of Razor views
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore`: EF Core error pages

### Reporting & Data Processing

- `EPPlus`: Excel file generation
- `DinkToPdf`: PDF generation
- `Newtonsoft.Json`: JSON processing

### Testing

- `xunit`: Testing framework
- `Moq`: Mocking library for unit tests
- `Microsoft.EntityFrameworkCore.InMemory`: In-memory database for tests

## Project Structure

```
KPI/
├── src/                           # Source code
│   ├── Areas/                     # Feature areas (Identity, etc.)
│   ├── Authorization/             # Authorization policies and handlers
│   ├── Controllers/               # MVC controllers
│   ├── Data/                      # Data access layer
│   │   ├── Repositories/          # Repository implementations
│   │   └── Migrations/            # EF Core migrations
│   ├── Extensions/                # Extension methods
│   ├── Infrastructure/            # Cross-cutting concerns
│   ├── Models/                    # Domain models and view models
│   │   ├── Entities/              # Entity classes
│   │   ├── Enums/                 # Enumeration types
│   │   └── ViewModels/            # View-specific models
│   ├── Services/                  # Business logic services
│   ├── Views/                     # Razor views
│   └── wwwroot/                   # Static files (CSS, JS, images)
├── tests/                         # Test projects
│   ├── KPI.UnitTests/             # Unit tests
│   └── KPI.IntegrationTests/      # Integration tests
```

## Database Schema Overview

The database follows a relational model with the following key tables:

- **AspNetUsers/AspNetRoles**: Identity tables for authentication
- **Department/Unit**: Organizational structure
- **Employee/Position**: Personnel management
- **KPI**: Key Performance Indicators
- **CSF**: Critical Success Factors
- **KpiMeasurement**: Actual measurements for KPIs
- **KpiTarget**: Target values for KPIs
- **Dashboard/Report**: Dashboard and reporting configurations

## Deployment Considerations

### Hosting Requirements

- Windows or Linux server with .NET 8.0 runtime
- SQL Server database (2019 or later recommended)
- HTTPS certificate for secure communication
- Sufficient storage for database growth and document uploads
- Recommended minimum: 2 CPU cores, 4GB RAM, 50GB storage

### Deployment Options

- **Azure App Service**: Managed hosting with easy scaling
- **IIS on Windows Server**: Traditional Windows hosting
- **Linux with Nginx/Apache**: Self-hosted on Linux
- **Docker**: Containerized deployment

### CI/CD Pipeline

The project supports continuous integration and deployment with:

- GitHub Actions or Azure DevOps for build automation
- Automated testing through test projects
- Database migration scripts for schema updates
- Environment-specific configuration settings

## Security Considerations

### Authentication Security

- HTTPS enforcement
- Secure cookie handling
- Strong password requirements
- Account lockout for failed attempts
- Email confirmation for registration

### Data Protection

- Connection string encryption
- Sensitive data encryption at rest
- HTTPS for data in transit
- Anti-forgery tokens for forms
- Input validation to prevent injection attacks

### Audit & Compliance

- User action logging
- Login attempt tracking
- Critical data change auditing
- Role-based access control
- Resource-based authorization
