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

Theo yêu cầu, hệ thống sẽ triển khai cấu trúc phân cấp đầy đủ sau, với việc gộp thực thể cùng loại:

```
┌───────────────┐
│   Objective   │
└───────┬───────┘
        │ 1:n
┌───────▼───────┐
│ Success Factor│
│     (SF)      │  Một entity với cờ IsCritical
└───────┬───────┘  SF: IsCritical = false
        │ 1:n      SF: IsCritical = true cho Success Factor quan trọng
┌───────▼───────┐
│ Success Factor│
│               │
└───────┬───────┘
        │ 1:n
        ▼
  ┌─────┴─────┐
  │           │
┌─▼─┐       ┌─▼─┐
│RI │       │PI │  Hai entity riêng biệt với cờ IsKey
└─┬─┘       └─┬─┘  RI: Result Indicator (IsKey = true/false)
  │           │    PI: Performance Indicator (IsKey = true/false)
  │ có cùng   │
  │ bảng đo   │
  │ lường     │
  ▼           ▼
┌───────────────┐
│  Measurement  │
└───────────────┘
```

### Mô tả cấu trúc phân cấp gộp

- **Objective (Mục tiêu)**: Mục tiêu chiến lược của tổ chức
- **SuccessFactor**: Entity dùng chung cho tất cả các yếu tố thành công
  - **Success Factor** - IsCritical = false: Yếu tố thành công, hỗ trợ đạt được Objective
  - **Success Factor** - IsCritical = true: Yếu tố thành công quan trọng
- **ResultIndicator**: Entity dùng chung cho các chỉ số đo lường kết quả
  - **Result Indicator (RI)** - IsKey = false: Chỉ số kết quả, đo lường kết quả liên quan đến Success Factor
  - **Result Indicator (RI)** - IsKey = true: Chỉ số kết quả quan trọng
- **PerformanceIndicator**: Entity dùng chung cho các chỉ số đo lường hiệu suất
  - **Performance Indicator (PI)** - IsKey = false: Chỉ số hiệu suất, đo lường hiệu suất liên quan đến Success Factor
  - **Performance Indicator (PI)** - IsKey = true: Chỉ số hiệu suất quan trọng
- **Measurement**: Bảng đo lường chung cho tất cả các loại chỉ số

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

Với cấu trúc phân cấp mới và yêu cầu gộp các entity, các thực thể chính sẽ bao gồm:

```csharp
// Objective - Mục tiêu (Entity chuẩn hóa, không phân biệt BusinessObjective)
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

// Success Factor (SF) - Yếu tố thành công - Gộp cả SF và CSF
public class SuccessFactor
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public bool IsCritical { get; set; } // Cờ xác định đây là SF hay CSF (true = CSF, false = SF)
    public int? Weight { get; set; } // Chỉ áp dụng cho CSF

    // Quan hệ với cấp cao hơn
    public Guid? ParentId { get; set; } // SF cha (null nếu là SF cấp cao nhất)
    public SuccessFactor Parent { get; set; }
    public Guid? ObjectiveId { get; set; } // Luôn liên kết với Objective cơ sở
    public Objective Objective { get; set; }

    // Relationships
    public ICollection<SuccessFactor> Children { get; set; } // SF con (CSF)
    public ICollection<ResultIndicator> ResultIndicators { get; set; }
    public ICollection<PerformanceIndicator> PerformanceIndicators { get; set; }
}

// Result Indicator (RI) - Chỉ số kết quả - Gộp cả RI và KRI
public class ResultIndicator
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Formula { get; set; }
    public bool IsKey { get; set; } // Cờ xác định đây là RI hay KRI (true = KRI, false = RI)
    public decimal? Target { get; set; }
    public MeasurementFrequency Frequency { get; set; }
    public MeasurementUnit Unit { get; set; }

    // Trách nhiệm
    public Guid? ResponsiblePersonId { get; set; }
    public Employee ResponsiblePerson { get; set; }

    // Quan hệ với SF/CSF
    public Guid SuccessFactorId { get; set; }
    public SuccessFactor SuccessFactor { get; set; }

    // Measurements
    public ICollection<Measurement> Measurements { get; set; }
}

// Performance Indicator (PI) - Chỉ số hiệu suất - Gộp cả PI và KPI
public class PerformanceIndicator
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Formula { get; set; }
    public bool IsKey { get; set; } // Cờ xác định đây là PI hay KPI (true = KPI, false = PI)
    public decimal? Target { get; set; }
    public MeasurementFrequency Frequency { get; set; }
    public MeasurementUnit Unit { get; set; }

    // Trách nhiệm
    public Guid? ResponsiblePersonId { get; set; }
    public Employee ResponsiblePerson { get; set; }

    // Quan hệ với SF/CSF
    public Guid SuccessFactorId { get; set; }
    public SuccessFactor SuccessFactor { get; set; }

    // Measurements
    public ICollection<Measurement> Measurements { get; set; }
}

// Measurement - Đo lường (dùng chung cho cả RI/KRI và PI/KPI)
public class Measurement
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
    public string Note { get; set; }

    // Phân biệt loại measurement
    public MeasurementType Type { get; set; } // Enum: ResultIndicator, PerformanceIndicator

    // Quan hệ (chỉ một trong hai trường này sẽ có giá trị, tùy thuộc vào Type)
    public Guid? ResultIndicatorId { get; set; }
    public ResultIndicator ResultIndicator { get; set; }

    public Guid? PerformanceIndicatorId { get; set; }
    public PerformanceIndicator PerformanceIndicator { get; set; }

    // Người đo lường
    public Guid CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }
}

// Enum để xác định loại measurement
public enum MeasurementType
{
    ResultIndicator = 1,
    PerformanceIndicator = 2
}
```

## Component Relationships

### Core Domain Components

1. **Strategic Management**

   - Objective Management
   - SF (Success Factor) Management
   - SuccessFactor Management

2. **Indicator Management**

   - RI (Result Indicator) Management
   - PI (Performance Indicator) Management
   - Result Indicator (RI) Management
   - Performance Indicator (PI) Management

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
- **Mapping**: AutoMapper profiles cho các entity và ViewModel
  - Cập nhật từ `CsfMappingProfile` thành `SuccessFactorMappingProfile`
  - Cập nhật các model từ `KpiViewModel` thành `IndicatorViewModel`

## Security Patterns

- **Authentication**: ASP.NET Core Identity with multi-factor support
- **Authorization**: Policy-based with both role and resource checks
  - **Chuẩn hóa tên Policy**: Đổi từ `CanViewCsfs` thành `CanViewSuccessFactors`, `CanManageCsfs` thành `CanManageSuccessFactors`
  - **Chuẩn hóa tên Handler**: Đổi từ `CsfAuthorizationHandler` thành `SuccessFactorAuthorizationHandler`
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

## Enum Structure

The system uses a comprehensive set of enums organized by domain area to provide consistent categorization and status tracking. The main enum namespaces include:

### KPISolution.Models.Enums.Indicator

Contains enums related to performance and result indicators:

```csharp
// BasicIndicatorEnums.cs - Core indicator type definitions
public enum IndicatorType
{
    [Display(Name = "Leading")]
    Leading = 1,

    [Display(Name = "Lagging")]
    Lagging = 2,

    [Display(Name = "Diagnostic")]
    Diagnostic = 3
}

public enum IndicatorStatus
{
    [Display(Name = "Active")]
    Active = 1,

    [Display(Name = "On Hold")]
    OnHold = 2,

    [Display(Name = "Archived")]
    Archived = 3,

    [Display(Name = "Proposed")]
    Proposed = 4
}

// IndicatorPropertyEnums.cs - Properties specific to indicators
public enum DataSource
{
    [Display(Name = "Manual Entry")]
    ManualEntry = 1,

    [Display(Name = "System Integration")]
    SystemIntegration = 2,

    [Display(Name = "Calculated")]
    Calculated = 3,

    [Display(Name = "External Import")]
    ExternalImport = 4
}

public enum PriorityLevel
{
    [Display(Name = "Low")]
    Low = 1,

    [Display(Name = "Medium")]
    Medium = 2,

    [Display(Name = "High")]
    High = 3,

    [Display(Name = "Critical")]
    Critical = 4
}

// MeasurementEnums.cs - Measurement-related enums
public enum MeasurementFrequency
{
    [Display(Name = "Daily")]
    Daily = 1,

    [Display(Name = "Weekly")]
    Weekly = 2,

    [Display(Name = "Monthly")]
    Monthly = 3,

    [Display(Name = "Quarterly")]
    Quarterly = 4,

    [Display(Name = "Annually")]
    Annually = 5,

    [Display(Name = "Ad Hoc")]
    AdHoc = 6
}

public enum MeasurementUnit
{
    [Display(Name = "Number")]
    Number = 1,

    [Display(Name = "Percentage")]
    Percentage = 2,

    [Display(Name = "Currency")]
    Currency = 3,

    [Display(Name = "Time")]
    Time = 4,

    [Display(Name = "Score")]
    Score = 5
}
```

### KPISolution.Models.Enums.Organization

Contains enums related to organizational structures:

```csharp
public enum DepartmentType
{
    [Display(Name = "Operations")]
    Operations = 1,

    [Display(Name = "Finance")]
    Finance = 2,

    [Display(Name = "Human Resources")]
    HumanResources = 3,

    [Display(Name = "Marketing")]
    Marketing = 4,

    [Display(Name = "Sales")]
    Sales = 5,

    [Display(Name = "IT")]
    IT = 6,

    [Display(Name = "Research & Development")]
    RnD = 7,

    [Display(Name = "Other")]
    Other = 8
}

public enum ObjectiveStatus
{
    [Display(Name = "Proposed")]
    Proposed = 1,

    [Display(Name = "In Progress")]
    InProgress = 2,

    [Display(Name = "Completed")]
    Completed = 3,

    [Display(Name = "On Hold")]
    OnHold = 4,

    [Display(Name = "Cancelled")]
    Cancelled = 5
}

public enum TimeframeType
{
    [Display(Name = "Short-term")]
    ShortTerm = 1,

    [Display(Name = "Medium-term")]
    MediumTerm = 2,

    [Display(Name = "Long-term")]
    LongTerm = 3
}
```

### KPISolution.Models.Enums.Business

Contains enums related to business perspectives and categorization:

```csharp
public enum BusinessPerspective
{
    [Display(Name = "Financial")]
    Financial = 1,

    [Display(Name = "Customer")]
    Customer = 2,

    [Display(Name = "Internal Process")]
    InternalProcess = 3,

    [Display(Name = "Learning & Growth")]
    LearningAndGrowth = 4,

    [Display(Name = "Other")]
    Other = 5
}

public enum SuccessFactorCategory
{
    [Display(Name = "Financial")]
    Financial = 1,

    [Display(Name = "Customer")]
    Customer = 2,

    [Display(Name = "Process")]
    Process = 3,

    [Display(Name = "People")]
    People = 4,

    [Display(Name = "Technology")]
    Technology = 5,

    [Display(Name = "Innovation")]
    Innovation = 6,

    [Display(Name = "Regulatory")]
    Regulatory = 7,

    [Display(Name = "Risk")]
    Risk = 8,

    [Display(Name = "Other")]
    Other = 9
}
```

### KPISolution.Models.Enums.Relationship

Contains enums related to relationships between entities:

```csharp
public enum RelationshipStrength
{
    [Display(Name = "None")]
    None = 0,

    [Display(Name = "Very Weak")]
    VeryWeak = 1,

    [Display(Name = "Weak")]
    Weak = 2,

    [Display(Name = "Moderate")]
    Moderate = 3,

    [Display(Name = "Strong")]
    Strong = 4,

    [Display(Name = "Very Strong")]
    VeryStrong = 5,

    [Display(Name = "Direct")]
    Direct = 6
}

public enum RelationshipType
{
    [Display(Name = "Contributes To")]
    ContributesTo = 1,

    [Display(Name = "Depends On")]
    DependsOn = 2,

    [Display(Name = "Correlates With")]
    CorrelatesWith = 3,

    [Display(Name = "Causes")]
    Causes = 4,

    [Display(Name = "Inhibits")]
    Inhibits = 5,

    [Display(Name = "Parent Of")]
    ParentOf = 6,

    [Display(Name = "Child Of")]
    ChildOf = 7,

    [Display(Name = "Mutual Influence")]
    MutualInfluence = 8,

    [Display(Name = "Overlaps With")]
    OverlapsWith = 9,

    [Display(Name = "Other")]
    Other = 10
}
```

## Business Objective Entity

The BusinessObjective entity extends the base Objective entity with additional properties specific to business-focused objectives:

```csharp
// BusinessObjective - Business-specific objectives
public class BusinessObjective : Objective
{
    [Display(Name = "Business Value")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? BusinessValue { get; set; }

    [Display(Name = "Stakeholders")]
    [MaxLength(500)]
    public string Stakeholders { get; set; }

    [Display(Name = "Success Criteria")]
    [MaxLength(1000)]
    [DataType(DataType.MultilineText)]
    public string SuccessCriteria { get; set; }

    [Display(Name = "Strategy Reference")]
    [MaxLength(200)]
    public string StrategyReference { get; set; }
}
```

The inheritance hierarchy allows BusinessObjective to leverage all properties from the base Objective class while adding business-specific attributes.
