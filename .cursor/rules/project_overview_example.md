# KPI Management System Project Overview

## Project Structure

The project follows MVC pattern with clear separation of concerns:

```
KPI/
├── src/
│   ├── Models/           # Data models and business logic
│   │   ├── Entities/     # Database entities
│   │   └── ViewModels/   # View-specific models
│   ├── Views/            # View templates and components
│   │   ├── Shared/       # Shared layouts and partials
│   │   ├── KPI/          # KPI management views
│   │   ├── CSF/          # CSF management views
│   │   └── Dashboard/    # Dashboard view components
│   ├── Controllers/      # Request handlers
│   │   ├── Api/          # API controllers
│   │   ├── KPI/          # KPI controllers
│   │   └── Admin/        # Admin controllers
│   ├── Services/         # Business services
│   │   ├── Interfaces/   # Service contracts
│   │   └── Implementation/ # Service implementations
│   ├── Data/             # Data access
│   │   ├── Context/      # Database context
│   │   └── Repositories/ # Data repositories
│   └── wwwroot/          # Static files (CSS, JS, images)
└── tests/                # Test projects
    ├── Unit/             # Unit tests
    └── Integration/      # Integration tests
```

## Key Patterns & Concepts

1. **MVC Pattern**:
   - Models represent data and business logic
   - Views handle presentation
   - Controllers manage user interactions

```7:34:KPI.Controllers/KPI/KpiController.cs
public class KpiController : Controller
{
    private readonly IKpiService _kpiService;
    private readonly ICsfService _csfService;

    public KpiController(IKpiService kpiService, ICsfService csfService)
    {
        _kpiService = kpiService;
        _csfService = csfService;
    }

    public async Task<IActionResult> Create(CreateKpiViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Load dependent data for form
            model.AvailableCsfs = await _csfService.GetAllCsfsForSelectionAsync();
            return View(model);
        }

        var result = await _kpiService.CreateKpiAsync(model);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError("", result.Error);
            model.AvailableCsfs = await _csfService.GetAllCsfsForSelectionAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = "KPI created successfully";
        return RedirectToAction(nameof(Details), new { id = result.Data });
    }
}
```

2. **Model-View Binding**:
   - ViewModels for form handling
   - Data validation attributes

```5:35:KPI.Models/ViewModels/KpiViewModel.cs
public class CreateKpiViewModel
{
    [Required(ErrorMessage = "KPI name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "KPI definition is required")]
    [StringLength(500, ErrorMessage = "Definition cannot exceed 500 characters")]
    public string Definition { get; set; }

    [Required(ErrorMessage = "Measurement unit is required")]
    public string MeasurementUnit { get; set; }

    [Required(ErrorMessage = "Measurement frequency is required")]
    public string MeasurementFrequency { get; set; }

    [Required(ErrorMessage = "Target value is required")]
    public decimal TargetValue { get; set; }

    public string CalculationFormula { get; set; }

    [Required(ErrorMessage = "At least one CSF must be selected")]
    public List<Guid> SelectedCsfIds { get; set; } = new();

    // For dropdown population
    public List<SelectListItem> AvailableCsfs { get; set; } = new();

    public string DataSource { get; set; }

    public KpiType KpiType { get; set; }
}

public enum KpiType
{
    KRI, // Key Result Indicator
    RI,  // Result Indicator
    PI,  // Performance Indicator
    KPI  // Key Performance Indicator
}
```

3. **Data Models**:
   - Entity classes with validation

```3:30:KPI.Models/Entities/Kpi.cs
public class Kpi : BaseEntity
{
    public string Name { get; set; }
    public string Definition { get; set; }
    public string MeasurementUnit { get; set; }
    public string MeasurementFrequency { get; set; }
    public decimal TargetValue { get; set; }
    public decimal? WarningThreshold { get; set; }
    public decimal? CriticalThreshold { get; set; }
    public string CalculationFormula { get; set; }
    public string DataSource { get; set; }
    public KpiType KpiType { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<KpiValue> Values { get; set; } = new List<KpiValue>();
    public virtual ICollection<KpiCsf> KpiCsfs { get; set; } = new List<KpiCsf>();

    public Kpi()
    {
        IsActive = true;
    }

    public void Update(string name, string definition, string measurementUnit,
                       string measurementFrequency, decimal targetValue,
                       string calculationFormula, string dataSource)
    {
        Name = name;
        Definition = definition;
        MeasurementUnit = measurementUnit;
        MeasurementFrequency = measurementFrequency;
        TargetValue = targetValue;
        CalculationFormula = calculationFormula;
        DataSource = dataSource;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetThresholds(decimal? warningThreshold, decimal? criticalThreshold)
    {
        WarningThreshold = warningThreshold;
        CriticalThreshold = criticalThreshold;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

4. **Services Layer**:
   - Business logic implementation

```7:110:KPI.Services/Implementation/KpiService.cs
public class KpiService : IKpiService
{
    private readonly IRepository<Kpi> _kpiRepository;
    private readonly IRepository<KpiCsf> _kpiCsfRepository;
    private readonly ILogger<KpiService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public KpiService(
        IRepository<Kpi> kpiRepository,
        IRepository<KpiCsf> kpiCsfRepository,
        IUnitOfWork unitOfWork,
        ILogger<KpiService> logger)
    {
        _kpiRepository = kpiRepository;
        _kpiCsfRepository = kpiCsfRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> CreateKpiAsync(CreateKpiViewModel model)
    {
        try
        {
            var existingKpi = await _kpiRepository.FirstOrDefaultAsync(
                k => k.Name == model.Name && k.IsActive);

            if (existingKpi != null)
            {
                return Result<Guid>.Failure($"KPI '{model.Name}' already exists");
            }

            var kpi = new Kpi
            {
                Name = model.Name,
                Definition = model.Definition,
                MeasurementUnit = model.MeasurementUnit,
                MeasurementFrequency = model.MeasurementFrequency,
                TargetValue = model.TargetValue,
                CalculationFormula = model.CalculationFormula,
                DataSource = model.DataSource,
                KpiType = model.KpiType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _kpiRepository.AddAsync(kpi);
            await _unitOfWork.SaveChangesAsync();

            // Create KPI-CSF relationships
            foreach (var csfId in model.SelectedCsfIds)
            {
                var kpiCsf = new KpiCsf
                {
                    KpiId = kpi.Id,
                    CsfId = csfId,
                    CreatedAt = DateTime.UtcNow
                };
                await _kpiCsfRepository.AddAsync(kpiCsf);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created KPI: {KpiId}", kpi.Id);

            return Result<Guid>.Success(kpi.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating KPI");
            return Result<Guid>.Failure("Failed to create KPI. Please try again later.");
        }
    }
}
```

## Core Features

1. **KPI Management**:
   - CRUD operations for KPIs, KRIs, RIs, and PIs
   - CSF relationship mapping
   - Threshold configuration

```7:60:KPI.Models/Entities/KpiValue.cs
public class KpiValue : BaseEntity
{
    public Guid KpiId { get; set; }
    public virtual Kpi Kpi { get; set; }

    public decimal Value { get; set; }
    public DateTime MeasurementDate { get; set; }
    public string Comments { get; set; }

    // For batch imports or integrations
    public string DataSource { get; set; }
    public bool IsAdjusted { get; set; }
    public string AdjustmentReason { get; set; }

    // For calculated KPIs
    public bool IsCalculated { get; set; }
    public string CalculationParameters { get; set; }

    public KpiValueStatus Status { get; set; }

    public KpiValue(Guid kpiId, decimal value, DateTime measurementDate)
    {
        KpiId = kpiId;
        Value = value;
        MeasurementDate = measurementDate;
        Status = KpiValueStatus.Submitted;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdjustValue(decimal newValue, string reason)
    {
        Value = newValue;
        IsAdjusted = true;
        AdjustmentReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(KpiValueStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum KpiValueStatus
{
    Submitted,
    Verified,
    Rejected,
    Adjusted
}
```

2. **CSF Management**:
   - Critical Success Factors tracking
   - KPI linkage

```7:33:KPI.Models/Entities/Csf.cs
public class Csf : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public decimal ProgressPercentage { get; set; }

    public virtual ICollection<KpiCsf> KpiCsfs { get; set; } = new List<KpiCsf>();

    public Csf(string name, string description)
    {
        Name = name;
        Description = description;
        IsActive = true;
        ProgressPercentage = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal progressPercentage)
    {
        ProgressPercentage = progressPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

## Infrastructure Components

1. **Data Access**:
   - Entity Framework Core with SQL Server
   - Repository pattern with Unit of Work

```7:70:KPI.Data/Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
```

2. **Dashboard Services**:
   - Data aggregation
   - Chart generation

```7:95:KPI.Services/Implementation/DashboardService.cs
public class DashboardService : IDashboardService
{
    private readonly IRepository<Kpi> _kpiRepository;
    private readonly IRepository<KpiValue> _kpiValueRepository;
    private readonly IRepository<Csf> _csfRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IRepository<Kpi> kpiRepository,
        IRepository<KpiValue> kpiValueRepository,
        IRepository<Csf> csfRepository,
        ILogger<DashboardService> logger)
    {
        _kpiRepository = kpiRepository;
        _kpiValueRepository = kpiValueRepository;
        _csfRepository = csfRepository;
        _logger = logger;
    }

    public async Task<ExecutiveDashboardViewModel> GetExecutiveDashboardAsync()
    {
        try
        {
            var dashboard = new ExecutiveDashboardViewModel();

            // Get KRIs for executive dashboard
            var kris = await _kpiRepository.GetAllAsync(k => k.KpiType == KpiType.KRI && k.IsActive);

            foreach (var kri in kris)
            {
                var latestValue = await _kpiValueRepository
                    .GetAllAsync(v => v.KpiId == kri.Id)
                    .Result
                    .OrderByDescending(v => v.MeasurementDate)
                    .FirstOrDefaultAsync();

                if (latestValue != null)
                {
                    var status = DetermineKpiStatus(kri, latestValue.Value);

                    dashboard.KriSummaries.Add(new KpiSummaryViewModel
                    {
                        Id = kri.Id,
                        Name = kri.Name,
                        CurrentValue = latestValue.Value,
                        TargetValue = kri.TargetValue,
                        MeasurementUnit = kri.MeasurementUnit,
                        MeasurementDate = latestValue.MeasurementDate,
                        Status = status
                    });
                }
            }

            // Get CSF progress
            var csfs = await _csfRepository.GetAllAsync(c => c.IsActive);
            dashboard.CsfSummaries = csfs.Select(c => new CsfSummaryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Progress = c.ProgressPercentage
            }).ToList();

            return dashboard;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating executive dashboard");
            throw;
        }
    }

    private KpiStatus DetermineKpiStatus(Kpi kpi, decimal currentValue)
    {
        if (kpi.CriticalThreshold.HasValue)
        {
            if (currentValue <= kpi.CriticalThreshold.Value)
            {
                return KpiStatus.Critical;
            }
        }

        if (kpi.WarningThreshold.HasValue)
        {
            if (currentValue <= kpi.WarningThreshold.Value)
            {
                return KpiStatus.Warning;
            }
        }

        if (currentValue >= kpi.TargetValue)
        {
            return KpiStatus.Good;
        }

        return KpiStatus.Satisfactory;
    }
}
```

3. **Notification System**:
   - Email notifications
   - In-app alerts

```7:85:KPI.Services/Implementation/NotificationService.cs
public class NotificationService : INotificationService
{
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IEmailSender emailSender,
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> SendKpiAlertAsync(Guid kpiId, string kpiName, decimal currentValue,
                                               KpiStatus status, List<string> recipients)
    {
        try
        {
            // Create in-app notification
            foreach (var recipient in recipients)
            {
                var notification = new Notification
                {
                    Title = $"KPI Alert: {kpiName}",
                    Message = $"KPI '{kpiName}' has reached {status} status with value {currentValue}.",
                    Type = NotificationType.Alert,
                    Recipient = recipient,
                    IsRead = false,
                    RelatedEntityId = kpiId,
                    RelatedEntityType = "KPI",
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.AddAsync(notification);
            }

            await _unitOfWork.SaveChangesAsync();

            // Send email notification
            var subject = $"KPI Alert: {kpiName} Status {status}";
            var message = $@"
                <h3>KPI Alert</h3>
                <p>The KPI '{kpiName}' has reached {status} status.</p>
                <p>Current Value: {currentValue}</p>
                <p>Please check the KPI Management System for more details.</p>
            ";

            await _emailSender.SendEmailAsync(recipients, subject, message);

            _logger.LogInformation("Sent KPI alert for {KpiId} to {RecipientCount} recipients",
                                  kpiId, recipients.Count);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending KPI alert for {KpiId}", kpiId);
            return Result.Failure("Failed to send KPI alert. Please try again later.");
        }
    }
}
```

## Key Workflows

1. **KPI Creation Process**:

   ```
   Controller → Validation → KPI Service → Repository → Database
   ```

2. **Dashboard Generation**:

   ```
   Controller → Dashboard Service → Data Aggregation → View Model
   ```

3. **Alert System**:

```7:40:KPI.Services/Implementation/AlertService.cs
public class AlertService : IAlertService
{
    private readonly IRepository<Kpi> _kpiRepository;
    private readonly IRepository<KpiValue> _kpiValueRepository;
    private readonly IRepository<AlertRule> _alertRuleRepository;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        IRepository<Kpi> kpiRepository,
        IRepository<KpiValue> kpiValueRepository,
        IRepository<AlertRule> alertRuleRepository,
        INotificationService notificationService,
        IUserService userService,
        ILogger<AlertService> logger)
    {
        _kpiRepository = kpiRepository;
        _kpiValueRepository = kpiValueRepository;
        _alertRuleRepository = alertRuleRepository;
        _notificationService = notificationService;
        _userService = userService;
        _logger = logger;
    }

    public async Task ProcessNewKpiValueAsync(Guid kpiId, decimal value)
    {
        try
        {
            var kpi = await _kpiRepository.GetByIdAsync(kpiId);
            if (kpi == null)
            {
                _logger.LogWarning("Cannot process alert for non-existent KPI: {KpiId}", kpiId);
                return;
            }

            // Determine KPI status based on value
            var status = DetermineKpiStatus(kpi, value);

            // Only proceed if status is Warning or Critical
            if (status != KpiStatus.Warning && status != KpiStatus.Critical)
            {
                return;
            }

            // Find alert rules for this KPI
            var rules = await _alertRuleRepository.GetAllAsync(r => r.KpiId == kpiId && r.IsActive);

            foreach (var rule in rules)
            {
                // Check if rule should trigger for this status
                if ((rule.TriggerOnWarning && status == KpiStatus.Warning) ||
                    (rule.TriggerOnCritical && status == KpiStatus.Critical))
                {
                    // Get recipients
                    var recipients = await _userService.GetUserEmailsByRoleAsync(rule.NotifyRole);

                    // Send notification
                    await _notificationService.SendKpiAlertAsync(
                        kpiId, kpi.Name, value, status, recipients);

                    _logger.LogInformation(
                        "Triggered alert rule {RuleId} for KPI {KpiId} with status {Status}",
                        rule.Id, kpiId, status);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing alert for KPI {KpiId}", kpiId);
        }
    }

    private KpiStatus DetermineKpiStatus(Kpi kpi, decimal currentValue)
    {
        if (kpi.CriticalThreshold.HasValue && currentValue <= kpi.CriticalThreshold.Value)
        {
            return KpiStatus.Critical;
        }

        if (kpi.WarningThreshold.HasValue && currentValue <= kpi.WarningThreshold.Value)
        {
            return KpiStatus.Warning;
        }

        if (currentValue >= kpi.TargetValue)
        {
            return KpiStatus.Good;
        }

        return KpiStatus.Satisfactory;
    }
}
```

## Getting Started Tips

1. Start with Core Entity Models (KPI, CSF, Users)
2. Set up database context and migrations
3. Implement authentication and authorization
4. Create core KPI management features
5. Build dashboard and reporting components

## Important Dependencies

- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- AutoMapper
- EPPlus (for Excel export)
- ChartJs.Blazor (for charts)
- X.PagedList.Mvc.Core (for pagination)

This structure provides a maintainable foundation for a KPI management system using the MVC pattern with clear separation of concerns.
