# KPI Management System - System Patterns

## System Architecture

The KPI Management System follows a clean architecture approach with distinct layers:

```
┌───────────────────────────────────────┐
│             Presentation              │
│  (Controllers, Views, View Models)    │
├───────────────────────────────────────┤
│             Service Layer             │
│    (Business Logic, Validation)       │
├───────────────────────────────────────┤
│             Data Access               │
│     (Repositories, DbContext)         │
├───────────────────────────────────────┤
│               Entities                │
│      (Domain Models, Enums)           │
└───────────────────────────────────────┘
```

### Presentation Layer

- ASP.NET Core MVC Controllers and Razor Views
- View Models for presentation-specific data structures
- UI Components using Bootstrap and jQuery
- Visualization components using Chart.js

### Service Layer

- Business logic encapsulated in service classes
- Input validation and business rule enforcement
- Transaction management for complex operations
- Integration with external systems (email, etc.)

### Data Access Layer

- Repository pattern for data access abstraction
- Unit of Work pattern for transaction management
- Entity Framework Core for ORM capabilities
- Custom queries for performance-critical operations

### Entity Layer

- Domain models representing core business concepts
- Rich domain models with behavior where appropriate
- Clear relationships between entity types
- Enum types for consistent categorization

## Cấu trúc phân cấp của chỉ số (Indicator Hierarchy)

Theo yêu cầu, hệ thống sẽ triển khai cấu trúc phân cấp đầy đủ sau:

```
┌───────────────┐
│   Objective   │
└───────┬───────┘
        │ 1:n
┌───────▼───────┐
│ Success Factor│
│      (SF)     │
└───────┬───────┘
        │ 1:n
┌───────▼───────┐
│Critical Success│
│  Factor (CSF) │
└───────┬───────┘
        │ 1:n
        ▼
  ┌─────┴─────┐
  │           │
┌─▼─┐       ┌─▼─┐
│RI │       │PI │
└─┬─┘       └─┬─┘
  │           │
  │ 1:n       │ 1:n
┌─▼─┐       ┌─▼─┐
│KRI│       │KPI│
└───┘       └───┘
```

### Mô tả cấu trúc phân cấp

- **Objective (Mục tiêu)**: Mục tiêu chiến lược của tổ chức
- **SF (Success Factor)**: Yếu tố thành công, hỗ trợ đạt được Objective
- **CSF (Critical Success Factor)**: Yếu tố thành công quan trọng, là tập con của SF
- **RI (Result Indicator)**: Chỉ số kết quả, đo lường kết quả liên quan đến CSF
- **PI (Performance Indicator)**: Chỉ số hiệu suất, đo lường hiệu suất liên quan đến CSF
- **KRI (Key Result Indicator)**: Chỉ số kết quả chính, là tập con quan trọng của RI
- **KPI (Key Performance Indicator)**: Chỉ số hiệu suất chính, là tập con quan trọng của PI

## Key Design Patterns

### Repository Pattern

All data access is abstracted through repositories, providing a consistent interface for CRUD operations and queries.

```csharp
// Generic repository interface
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(object id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### Unit of Work Pattern

Coordinates operations across multiple repositories and ensures transaction integrity.

```csharp
// Unit of Work interface
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
}
```

### Service Pattern

Business logic encapsulated in service classes with clear interfaces.

```csharp
// Example service interface
public interface IKpiService
{
    Task<KpiViewModel> GetByIdAsync(Guid id);
    Task<IEnumerable<KpiViewModel>> GetAllAsync();
    Task<IEnumerable<KpiViewModel>> GetByDepartmentAsync(Guid departmentId);
    Task<KpiViewModel> CreateAsync(KpiCreateViewModel model);
    Task UpdateAsync(KpiUpdateViewModel model);
    Task DeleteAsync(Guid id);
}
```

### View Model Pattern

Separate data models for UI presentation and binding.

```csharp
// Example view model
public class KpiViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Formula { get; set; }
    public decimal Target { get; set; }
    public MeasurementFrequency Frequency { get; set; }
    public string DepartmentName { get; set; }
    public string ResponsiblePersonName { get; set; }
    public ICollection<MeasurementViewModel> RecentMeasurements { get; set; }
}
```

### Policy-based Authorization

Combines role and resource-based authorization using ASP.NET Core's policy system.

```csharp
// Authorization handlers and policies
services.AddKpiPolicies();
```

## Mô hình Thực thể Mở rộng (Extended Entity Model)

Với cấu trúc phân cấp mới, các thực thể chính sẽ bao gồm:

```csharp
// Objective - Mục tiêu
public class Objective
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ObjectiveStatus Status { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    // Relationships
    public ICollection<SuccessFactor> SuccessFactors { get; set; }
}

// Success Factor (SF) - Yếu tố thành công
public class SuccessFactor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public Guid ObjectiveId { get; set; }
    public Objective Objective { get; set; }

    // Relationships
    public ICollection<CriticalSuccessFactor> CriticalSuccessFactors { get; set; }
}

// Critical Success Factor (CSF) - Yếu tố thành công quan trọng
public class CriticalSuccessFactor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Weight { get; set; }
    public Guid SuccessFactorId { get; set; }
    public SuccessFactor SuccessFactor { get; set; }

    // Relationships
    public ICollection<ResultIndicator> ResultIndicators { get; set; }
    public ICollection<PerformanceIndicator> PerformanceIndicators { get; set; }
}

// Result Indicator (RI) - Chỉ số kết quả
public class ResultIndicator
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Formula { get; set; }
    public bool IsKey { get; set; } // Xác định xem đây có phải KRI không
    public Guid CsfId { get; set; }
    public CriticalSuccessFactor Csf { get; set; }

    // Measurements
    public ICollection<Measurement> Measurements { get; set; }
}

// Performance Indicator (PI) - Chỉ số hiệu suất
public class PerformanceIndicator
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Formula { get; set; }
    public bool IsKey { get; set; } // Xác định xem đây có phải KPI không
    public Guid CsfId { get; set; }
    public CriticalSuccessFactor Csf { get; set; }

    // Measurements
    public ICollection<Measurement> Measurements { get; set; }
}
```

## Component Relationships

### Core Domain Components

1. **Strategic Management**

   - Objective Management
   - SF (Success Factor) Management
   - CSF (Critical Success Factor) Management

2. **Indicator Management**

   - RI (Result Indicator) Management
   - PI (Performance Indicator) Management
   - KRI (Key Result Indicator) Management
   - KPI (Key Performance Indicator) Management

3. **Measurement & Tracking**

   - Indicator Measurement
   - Target Setting
   - Progress Tracking
   - Threshold Alerts

4. **Organization Structure**

   - Departments
   - Units
   - Positions
   - Employees

5. **Dashboard & Reporting**

   - Executive Dashboards
   - Department Dashboards
   - Custom Reports
   - Trend Analysis

6. **User & Security**

   - Authentication
   - Authorization
   - Audit Trails
   - User Preferences

7. **Notification System**
   - Alerts
   - Reminders
   - Subscriptions
   - Email Templates

## Data Flow Architecture

```
┌───────────────┐     ┌───────────────┐     ┌───────────────┐
│  User Input   │────►│  Validation   │────►│  Processing   │
└───────────────┘     └───────────────┘     └───────┬───────┘
                                                    │
┌───────────────┐     ┌───────────────┐     ┌───────▼───────┐
│   Response    │◄────│  Formatting   │◄────│  Data Access  │
└───────────────┘     └───────────────┘     └───────────────┘
```

## Integration Points

- **Email Notifications**: SMTP integration for alerts and reminders
- **Excel Import/Export**: EPPlus library for data exchange
- **PDF Generation**: DinkToPdf for report generation
- **Authentication**: ASP.NET Core Identity with external provider support
- **Logging**: Serilog for structured logging

## Security Patterns

- **Authentication**: ASP.NET Core Identity with multi-factor support
- **Authorization**: Policy-based with both role and resource checks
- **Data Protection**: Encryption of sensitive data
- **Input Validation**: Server-side validation with ModelState
- **CSRF Protection**: Anti-forgery tokens
- **SSL/TLS**: HTTPS enforcement
- **Password Policy**: Strong password requirements
- **Account Lockout**: Prevents brute force attacks

## Scalability Considerations

- **Caching**: Strategic caching of dashboard and report data
- **Query Optimization**: Efficient database queries with pagination
- **Asynchronous Processing**: Async/await pattern throughout
- **Background Processing**: For report generation and notifications
- **Database Indexing**: Strategic indexes for performance

## UI and Design Patterns

### Bootstrap Integration

- The application uses Bootstrap 5 as its primary UI framework
- Custom CSS is minimized in favor of Bootstrap's utility classes
- Common Bootstrap components used throughout the application:
  - Cards for content organization
  - Badges for status and priority indicators
  - Progress bars for visual representation of completion
  - Dropdowns for navigation and action menus
  - Forms with consistent styling and validation

### Layout Patterns

- Base layout defined in `_Layout.cshtml` with a responsive navbar
- Page titles use `_PageTitle.cshtml` partial view that supports command buttons via ViewData
- Command buttons follow a consistent pattern:
  - Primary buttons for main actions (create, save)
  - Secondary buttons for navigation (back to list)
  - Danger buttons for destructive actions (delete)
- Forms follow a consistent structure:
  - Required fields are visually highlighted
  - Validation messages appear below form fields
  - Submit buttons at the bottom of forms

### Component Styling Standards

- Progress bars: Use Bootstrap's `progress` and `progress-bar` classes with contextual colors
- Badges: Use Bootstrap's `badge` class with `bg-*` colors
- Cards: Use `card`, `card-header`, and `card-body` with border colors to differentiate types
- Form controls: Use Bootstrap's `form-control`, `form-select`, and `form-range` classes
- Buttons: Use Bootstrap's `btn` class with `btn-*` contextual colors

### Responsive Design

- Mobile-first approach with responsive breakpoints
- Navbar collapses to hamburger menu on smaller screens
- Grid system used for layout with appropriate column sizes for different screen sizes
- Tables are responsive using Bootstrap's `table-responsive` class

### UI Scaling Pattern

- Custom solution for adjusting overall interface scale:

  ```
  ┌────────────────┐      ┌───────────────────┐      ┌───────────────┐
  │ Scale Controls │─────►│ CSS Custom Props  │─────►│ HTML Attribute│
  └────────────────┘      └───────────────────┘      └───────┬───────┘
         ▲                                                    │
         │                                                    ▼
  ┌──────┴───────┐                                     ┌─────────────┐
  │ LocalStorage │◄────────────────────────────────────┤ zoom/scale  │
  └──────────────┘                                     └─────────────┘
  ```

- Implementation details:

  - CSS custom properties (`--ui-scale`) for storing scale factor
  - HTML data attribute (`data-scale`) to trigger scaling mechanism
  - CSS classes (`ui-scale-*`) to define preset scale factors (75%, 80%, 90%, 100%)
  - LocalStorage persistence for user preference
  - Fixed-position UI control element for easily adjusting scale
  - Self-correcting scale for UI controls to ensure they remain usable

- Benefits:
  - Allows users to optimize interface density based on preference
  - Maintains aspect ratios and proportions consistently
  - Preserves all functionality at any scale
  - Provides sensible defaults while allowing customization
  - Solves the problem of oversized interface without sacrificing readability
