using System.Security.Claims;
namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller responsible for managing different types of dashboards including executive,
    /// department-specific, and custom user dashboards.
    /// </summary>
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the DashboardController.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for data access</param>
        /// <param name="logger">Logger for the DashboardController</param>
        /// <param name="mapper">Mapper for object mapping</param>
        public DashboardController(
            IUnitOfWork unitOfWork,
            ILogger<DashboardController> logger,
            IMapper mapper)
        {
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Displays the executive dashboard
        /// </summary>
        /// <returns>Executive dashboard view</returns>
        [HttpGet]
        [Authorize(Roles = "Executive,Administrator")]
        public async Task<IActionResult> Executive(int departmentPage = 1, int kriPage = 1)
        {
            try
            {
                // Store the pagination parameters for use in the view
                this.ViewBag.DepartmentPage = departmentPage;
                this.ViewBag.KriPage = kriPage;

                // Create view model
                var viewModel = new ExecutiveDashboardViewModel
                {
                    Title = "Bảng điều khiển điều hành",
                    LastUpdated = DateTime.Now,
                    DepartmentPage = departmentPage,
                    KriPage = kriPage
                };

                this._logger.LogInformation("Loading executive dashboard");

                // Get KRIs (ResultIndicators with IsKey = true) and populate KRI summaries
                var kris = await this._unitOfWork.ResultIndicators.GetAllAsync(r => r.IsKey); // Use ResultIndicators and filter by IsKey
                viewModel.KriSummaries = kris.Select(k => new ExecutiveIndicatorSummaryViewModel
                {
                    Id = k.Id,
                    Name = k.Name ?? "Unnamed KRI",
                    Code = k.Code ?? string.Empty,
                    TargetValue = k.TargetValue,
                    CurrentValue = k.CurrentValue,
                    MeasurementUnit = k.Unit.ToString(),
                    Status = k.Status,
                    Department = k.Department?.Name ?? "Unassigned",
                    StatusCssClass = this.GetStatusCssClass(k.Status),
                    StatusDisplay = this.GetStatusDisplay(k.Status)
                }).ToList();

                // Get CSFs (SuccessFactors with IsCritical = true) and populate CSF summaries
                var csfs = await this._unitOfWork.SuccessFactors.GetAllAsync(sf => sf.IsCritical);
                viewModel.SuccessFactorSummaries = csfs.Select(c => new ExecutiveSuccessFactorSummaryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    ProgressPercentage = c.ProgressPercentage
                }).ToList();

                // Lấy tất cả các phòng ban
                var departments = await this._unitOfWork.Departments.GetAllAsync();
                viewModel.PerformanceByDepartment = [];

                // Duyệt qua từng phòng ban và lấy thông tin hiệu suất
                foreach (var department in departments)
                {
                    // Lấy tất cả chỉ số của phòng ban
                    var departmentPIs = await this._unitOfWork.PerformanceIndicators.GetAllAsync(pi => pi.DepartmentId == department.Id);
                    var departmentRIs = await this._unitOfWork.ResultIndicators.GetAllAsync(ri => ri.DepartmentId == department.Id);

                    // Kết hợp danh sách chỉ số
                    var allIndicators = new List<BaseEntity>();
                    allIndicators.AddRange(departmentPIs);
                    allIndicators.AddRange(departmentRIs);

                    // Đếm số lượng chỉ số đang có vấn đề (ở trạng thái AtRisk hoặc BelowTarget)
                    int atRiskCount = allIndicators.Count(i =>
                        (i is PerformanceIndicator pi && (pi.Status == IndicatorStatus.AtRisk || pi.Status == IndicatorStatus.BelowTarget)) ||
                        (i is ResultIndicator ri && (ri.Status == IndicatorStatus.AtRisk || ri.Status == IndicatorStatus.BelowTarget))
                    );

                    // Đếm số lượng chỉ số đạt mục tiêu
                    int onTargetCount = allIndicators.Count(i =>
                        (i is PerformanceIndicator pi && pi.Status == IndicatorStatus.OnTarget) ||
                        (i is ResultIndicator ri && ri.Status == IndicatorStatus.OnTarget)
                    );

                    // Tính phần trăm hiệu suất dựa trên số lượng chỉ số đạt mục tiêu
                    int performancePercentage = allIndicators.Count > 0
                        ? (int)Math.Round((double)onTargetCount / allIndicators.Count * 100)
                        : 0;

                    // Thêm thông tin hiệu suất phòng ban vào danh sách
                    viewModel.PerformanceByDepartment.Add(new DepartmentPerformanceViewModel
                    {
                        DepartmentId = department.Id,
                        DepartmentName = department.Name,
                        PerformanceScore = performancePercentage,
                        TargetAchievementRate = allIndicators.Count > 0
                            ? (decimal)onTargetCount / allIndicators.Count * 100
                            : 0,
                        CompletionRate = performancePercentage,
                        TotalIndicators = allIndicators.Count,
                        IndicatorsOnTarget = onTargetCount,
                        IndicatorsBelowTarget = atRiskCount,
                        LastUpdated = DateTime.Now
                    });
                }

                // Lấy tất cả các Objectives từ database
                var objectives = await this._unitOfWork.Objectives.GetAllAsync();
                viewModel.ObjectiveCount = objectives.Count();

                // Lấy chi tiết từng Objective và điền vào ObjectiveSummaries
                viewModel.ObjectiveSummaries = [];
                foreach (var objective in objectives)
                {
                    var objectiveSuccessFactors = await this._unitOfWork.SuccessFactors.GetAllAsync(sf => sf.ObjectiveId == objective.Id);
                    var objectiveCriticalSuccessFactors = objectiveSuccessFactors.Where(sf => sf.IsCritical).ToList();

                    // Đếm số lượng KPI liên quan đến Objective này
                    var allKpis = new List<BaseEntity>();
                    foreach (var sf in objectiveSuccessFactors)
                    {
                        var indicators = await this._unitOfWork.ResultIndicators.GetAllAsync(ri => ri.SuccessFactorId == sf.Id);
                        allKpis.AddRange(indicators);
                    }

                    // Tính toán tiến độ dựa trên trung bình của tiến độ các success factor
                    int progress = objectiveSuccessFactors.Any()
                        ? (int)Math.Round(objectiveSuccessFactors.Average(sf => sf.ProgressPercentage))
                        : 0;

                    // Xác định CSS class dựa trên tiến độ
                    string progressCssClass = progress switch
                    {
                        >= 80 => "bg-success",
                        >= 60 => "bg-warning",
                        >= 40 => "bg-info",
                        _ => "bg-danger"
                    };

                    // Lấy tên phòng ban
                    string departmentName = "Chưa phân bổ";
                    if (objective.DepartmentId.HasValue)
                    {
                        var department = await this._unitOfWork.Departments.GetByIdAsync(objective.DepartmentId.Value);
                        if (department != null)
                        {
                            departmentName = department.Name;
                        }
                    }

                    viewModel.ObjectiveSummaries.Add(new ObjectiveSummaryViewModel
                    {
                        Id = objective.Id,
                        Name = objective.Name,
                        DepartmentName = departmentName,
                        ProgressPercentage = progress,
                        ProgressCssClass = progressCssClass,
                        SuccessFactorCount = objectiveSuccessFactors.Count(),
                        CriticalSuccessFactorCount = objectiveCriticalSuccessFactors.Count(),
                        IndicatorCount = allKpis.Count()
                    });
                }

                // Lấy tất cả các Success Factors từ database
                var successFactors = await this._unitOfWork.SuccessFactors.GetAllAsync();
                viewModel.SuccessFactorCount = successFactors.Count();

                // Lấy tất cả Critical Success Factors (Success Factors có IsCritical = true)
                var criticalSuccessFactors = await this._unitOfWork.SuccessFactors.GetAllAsync(sf => sf.IsCritical);
                viewModel.SuccessFactorCount = criticalSuccessFactors.Count();

                // Lấy tất cả Performance Indicators và Result Indicators
                var pis = await this._unitOfWork.PerformanceIndicators.GetAllAsync();
                var ris = await this._unitOfWork.ResultIndicators.GetAllAsync();
                viewModel.IndicatorCount = pis.Count() + ris.Count();

                // Lấy tất cả Key Result Indicators (Result Indicators có IsKey = true)
                var keyResultIndicators = await this._unitOfWork.ResultIndicators.GetAllAsync(r => r.IsKey);
                viewModel.KeyIndicatorCount = keyResultIndicators.Count() + pis.Count(p => p.IsKey);

                // Tính tổng số KPIs/KRIs
                viewModel.TotalIndicatorCount = viewModel.KeyIndicatorCount;

                // Đếm số lượng KPIs theo trạng thái
                foreach (IndicatorStatus status in Enum.GetValues(typeof(IndicatorStatus)))
                {
                    int statusCount =
                        pis.Count(pi => pi.IsKey && pi.Status == status) +
                        keyResultIndicators.Count(ri => ri.Status == status);

                    viewModel.IndicatorsByStatus[status] = statusCount;
                }

                // Tính phần trăm chỉ số đạt mục tiêu
                viewModel.OnTargetPercentage = viewModel.TotalIndicatorCount > 0 && viewModel.IndicatorsByStatus.ContainsKey(IndicatorStatus.OnTarget)
                    ? (decimal)viewModel.IndicatorsByStatus[IndicatorStatus.OnTarget] / viewModel.TotalIndicatorCount * 100
                    : 0;

                // Các repository Alerts và MeasurementUpdates chưa được định nghĩa trong IUnitOfWork
                // Tạm thời sử dụng dữ liệu mẫu cho phần alerts và updates
                viewModel.AlertsRequiringAttention =
                [
                    new IndicatorAlertViewModel
                    {
                        Id = Guid.NewGuid(),
                        IndicatorId = Guid.NewGuid(),
                        IndicatorName = "KPI-001: Tỷ lệ đơn hàng đúng hạn",
                        Message = "Tỷ lệ đơn hàng đúng hạn giảm xuống dưới 85%, cần kiểm tra ngay",
                        Severity = AlertSeverity.Critical,
                        SeverityDisplay = "Nghiêm trọng",
                        SeverityCssClass = "text-danger",
                        CreatedOn = DateTime.Now.AddDays(-2)
                    }
                ];

                viewModel.RecentUpdates =
                [
                    new IndicatorUpdateViewModel
                    {
                        IndicatorId = Guid.NewGuid(),
                        IndicatorName = "KPI-005: Chỉ số hài lòng khách hàng",
                        PreviousValue = 85,
                        NewValue = 92,
                        PercentageChange = 7,
                        ChangeCssClass = "text-success",
                        UpdatedAt = DateTime.Now.AddDays(-7),
                        UpdatedBy = "system"
                    }
                ];

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading executive dashboard");
                return View("Error");
            }
        }

        /// <summary>
        /// Displays a department-specific dashboard showing KPIs for a particular department.
        /// </summary>
        /// <param name="id">The unique identifier of the department.</param>
        /// <returns>The department dashboard view with department-specific KPI data.</returns>
        [HttpGet]
        public async Task<IActionResult> Department(Guid id)
        {
            try
            {
                this._logger.LogInformation("Loading department dashboard for department {DepartmentId}", id);

                // Get the department
                var department = await this._unitOfWork.Departments.GetByIdAsync(id);

                if (department == null)
                {
                    this._logger.LogWarning("Department not found with ID {DepartmentId}.", id);
                    return this.NotFound();
                }

                // Create the view model
                var viewModel = new DepartmentDashboardViewModel
                {
                    DepartmentId = department.Id,
                    DepartmentName = department.Name ?? string.Empty,
                    LastUpdated = DateTime.UtcNow
                };

                // Fetch PIs belonging to this department
                var pisForDepartment = await this._unitOfWork.PerformanceIndicators
                    .GetAllAsync(p => p.DepartmentId == id);

                // Fetch RIs using Include/ThenInclude on the IQueryable returned by GetAll()
                var risForDepartment = await this._unitOfWork.ResultIndicators.GetAll() // Assuming GetAll() returns IQueryable<ResultIndicator>
                    .Include(r => r.SuccessFactor)
                        .ThenInclude(sf => sf != null ? sf.Objective : null) // Safe navigation for ThenInclude
                            .ThenInclude(o => o != null ? o.Department : null) // Safe navigation for ThenInclude
                    .Where(r => r.SuccessFactor != null && r.SuccessFactor.Objective != null && r.SuccessFactor.Objective.DepartmentId == id)
                    .ToListAsync();

                // Combine the lists
                var allKpisForDepartment = new List<BaseEntity>();
                allKpisForDepartment.AddRange(pisForDepartment);
                allKpisForDepartment.AddRange(risForDepartment);

                // Populate KPI summaries - Handle Department based on type
                viewModel.KpiSummaries = allKpisForDepartment.Select(k => new IndicatorSummaryViewModel
                {
                    Id = k.Id,
                    Name = k is PerformanceIndicator pi ? pi.Name : (k is ResultIndicator ri ? ri.Name : "Unknown Indicator"),
                    Code = k is PerformanceIndicator pi_c ? pi_c.Code : (k is ResultIndicator ri_c ? ri_c.Code : string.Empty),
                    TargetValue = k is PerformanceIndicator pi_t ? pi_t.TargetValue : (k is ResultIndicator ri_t ? ri_t.TargetValue : null),
                    // Handle CurrentValue similarly if needed based on type
                    CurrentValue = k is PerformanceIndicator pi_cv ? pi_cv.CurrentValue : (k is ResultIndicator ri_cv ? ri_cv.CurrentValue : null), // Example: assuming CurrentValue exists on both
                    MeasurementUnit = (k is PerformanceIndicator pi_u ? pi_u.Unit : (k is ResultIndicator ri_u ? ri_u.Unit : null))?.ToString() ?? string.Empty, // Safely call ToString()
                    Status = k is PerformanceIndicator pi_s ? pi_s.Status : (k is ResultIndicator ri_s ? ri_s.Status : IndicatorStatus.Draft), // Example default
                    // Get Department Name based on type
                    Department = (k is PerformanceIndicator pi_d ? pi_d.Department?.Name : (k is ResultIndicator ri_d ? ri_d.SuccessFactor?.Objective?.Department?.Name : string.Empty)) ?? string.Empty, // Ensure non-null
                    StatusCssClass = this.GetStatusCssClass(k is PerformanceIndicator pi_sc ? pi_sc.Status : (k is ResultIndicator ri_sc ? ri_sc.Status : IndicatorStatus.Draft)),
                    StatusDisplay = this.GetStatusDisplay(k is PerformanceIndicator pi_sd ? pi_sd.Status : (k is ResultIndicator ri_sd ? ri_sd.Status : IndicatorStatus.Draft))
                }).ToList();

                // Get CSFs linked to this department
                var csfsByDepartment = await this._unitOfWork.SuccessFactors
                     .GetAllAsync(c => c.IsCritical && c.DepartmentId == id);

                viewModel.LinkedSuccessFactors = csfsByDepartment.Select(c => new SuccessFactorSummaryViewModel
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Code = c.Code ?? string.Empty,
                    ProgressPercentage = c.ProgressPercentage,
                    ProgressCssClass = c.ProgressPercentage > 80 ? "bg-success" : (c.ProgressPercentage > 60 ? "bg-warning" : "bg-danger"),
                    StatusCssClass = c.ProgressPercentage > 80 ? "bg-success" : (c.ProgressPercentage > 60 ? "bg-warning" : "bg-danger")
                }).ToList();

                // Calculate statistics
                viewModel.TotalIndicatorCount = viewModel.KpiSummaries.Count;
                viewModel.AtRiskIndicatorCount = viewModel.KpiSummaries.Count(k => k.Status == IndicatorStatus.AtRisk || k.Status == IndicatorStatus.BelowTarget);
                viewModel.OnTargetPercentage = viewModel.TotalIndicatorCount > 0
                    ? (decimal)viewModel.KpiSummaries.Count(k => k.Status == IndicatorStatus.OnTarget) / viewModel.TotalIndicatorCount * 100
                    : 0;
                viewModel.OverallPerformance = viewModel.OnTargetPercentage;
                viewModel.PerformanceCssClass = viewModel.OverallPerformance > 80 ? "bg-success" :
                                               (viewModel.OverallPerformance > 60 ? "bg-warning" : "bg-danger");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading department dashboard for department {DepartmentId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Helper method to get CSS class for status display
        /// </summary>
        private string GetStatusCssClass(IndicatorStatus status)
        {
            return status switch
            {
                IndicatorStatus.OnTarget => "bg-success",
                IndicatorStatus.AtRisk => "bg-warning",
                IndicatorStatus.BelowTarget => "bg-danger",
                IndicatorStatus.Active => "bg-primary",
                IndicatorStatus.Draft => "bg-secondary",
                IndicatorStatus.UnderReview => "bg-info",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Helper method to get display text for status
        /// </summary>
        private string GetStatusDisplay(IndicatorStatus status)
        {
            return status switch
            {
                IndicatorStatus.OnTarget => "Đạt mục tiêu",
                IndicatorStatus.AtRisk => "Cần chú ý",
                IndicatorStatus.BelowTarget => "Không đạt",
                IndicatorStatus.Active => "Hoạt động",
                IndicatorStatus.Draft => "Bản nháp",
                IndicatorStatus.UnderReview => "Đang xem xét",
                _ => "Không xác định"
            };
        }

        /// <summary>
        /// Displays a custom dashboard for the current user.
        /// </summary>
        /// <param name="id">The unique identifier of the custom dashboard. If null, displays the user's default dashboard.</param>
        /// <returns>The custom dashboard view with user-specific KPI data.</returns>
        [HttpGet]
        public async Task<IActionResult> Custom(Guid? id)
        {
            try
            {
                var userName = this.User.Identity?.Name ?? "unknown";
                this._logger.LogInformation("Loading custom dashboard {DashboardId} for user {UserId}",
                    id, userName);

                CustomDashboardViewModel viewModel;

                if (!id.HasValue)
                {
                    // Get user's default dashboard
                    var defaultDashboard = await this._unitOfWork.CustomDashboards.FirstOrDefaultAsync(
                        d => d.UserId == userName && d.IsDefault);

                    if (defaultDashboard == null)
                    {
                        // Create a new default dashboard if none exists
                        defaultDashboard = new CustomDashboard
                        {
                            Title = "My Dashboard",
                            UserId = userName,
                            UserName = userName,
                            IsDefault = true
                        };
                        await this._unitOfWork.CustomDashboards.AddAsync(defaultDashboard);
                        await this._unitOfWork.SaveChangesAsync();
                    }

                    viewModel = this.MapToViewModel(defaultDashboard);
                }
                else
                {
                    // Get specific dashboard
                    var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id.Value);
                    if (dashboard == null)
                    {
                        this._logger.LogWarning("Custom dashboard not found {DashboardId}", id);
                        return this.NotFound();
                    }

                    if (!dashboard.IsShared && dashboard.UserId != userName)
                    {
                        this._logger.LogWarning("User {UserId} attempted to access unauthorized dashboard {DashboardId}",
                            userName, id);
                        return this.Forbid();
                    }

                    viewModel = this.MapToViewModel(dashboard);
                }

                // Get all KPIs to populate available KPIs
                var allKpis = new List<BaseEntity>();
                var kris = await this._unitOfWork.ResultIndicators.GetAllAsync(r => r.IsKey); // Use ResultIndicators with filter
                var pis = await this._unitOfWork.PerformanceIndicators.GetAllAsync();
                var ris = await this._unitOfWork.ResultIndicators.GetAllAsync(); // Get all RIs

                allKpis.AddRange(kris); // Add KRIs (which are RIs with IsKey=true)
                allKpis.AddRange(pis);
                // Add RIs that are *not* KRIs to avoid duplicates
                allKpis.AddRange(ris.Where(r => !r.IsKey));

                // Populate available KPIs
                viewModel.AvailableIndicators = allKpis.Select(k => new IndicatorSummaryViewModel
                {
                    Id = k.Id,
                    Name = k is PerformanceIndicator pi ? pi.Name : (k is ResultIndicator ri ? ri.Name : "Unknown Indicator"),
                    Code = k is PerformanceIndicator pi_c ? pi_c.Code : (k is ResultIndicator ri_c ? ri_c.Code : string.Empty),
                    Department = (k is PerformanceIndicator pi_d ? pi_d.Department?.Name : (k is ResultIndicator ri_d ? ri_d.SuccessFactor?.Objective?.Department?.Name : string.Empty)) ?? string.Empty // Ensure non-null
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error loading custom dashboard {DashboardId}", id);
                return View("Error");
            }
        }

        /// <summary>
        /// Displays the form for creating a new custom dashboard.
        /// </summary>
        /// <returns>The create dashboard view.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            var userName = this.User.Identity?.Name ?? "unknown";
            this._logger.LogInformation("Displaying create dashboard form for user {UserId}", userName);
            var viewModel = new CustomDashboardViewModel
            {
                Title = "My Dashboard",
                UserId = userName,
                UserName = userName
            };
            return View(viewModel);
        }

        /// <summary>
        /// Creates a new custom dashboard.
        /// </summary>
        /// <param name="viewModel">The dashboard configuration data.</param>
        /// <returns>Redirects to the newly created dashboard if successful; otherwise returns to the create form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomDashboardViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var userName = this.User.Identity?.Name ?? "unknown";
                this._logger.LogInformation("Creating new dashboard for user {UserId}", userName);

                var dashboard = new CustomDashboard
                {
                    Title = viewModel.Title,
                    UserId = userName,
                    UserName = userName,
                    IsDefault = viewModel.IsDefault,
                    IsShared = viewModel.IsShared,
                    RefreshInterval = viewModel.RefreshInterval,
                    LayoutConfiguration = viewModel.LayoutConfiguration ?? string.Empty
                };

                await this._unitOfWork.CustomDashboards.AddAsync(dashboard);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully created dashboard {DashboardId} for user {UserId}",
                    dashboard.Id, userName);

                return this.RedirectToAction(nameof(this.Custom), new { id = dashboard.Id });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error creating custom dashboard for user {UserId}", this.User.Identity?.Name ?? "unknown");
                this.ModelState.AddModelError("", "Error creating dashboard. Please try again.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to edit.</param>
        /// <returns>The edit dashboard view if authorized; otherwise returns a forbidden or not found result.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            this._logger.LogInformation("Loading edit form for dashboard {DashboardId}", id);

            var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
            if (dashboard == null)
            {
                this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                return this.NotFound();
            }

            var userName = this.User.Identity?.Name ?? "unknown";
            if (dashboard.UserId != userName)
            {
                this._logger.LogWarning("User {UserId} attempted to edit unauthorized dashboard {DashboardId}",
                    userName, id);
                return this.Forbid();
            }

            var viewModel = this.MapToViewModel(dashboard);

            // Get all KPIs to populate available KPIs
            var allKpis = new List<BaseEntity>();
            var kris = await this._unitOfWork.ResultIndicators.GetAllAsync(r => r.IsKey); // Use ResultIndicators with filter
            var pis = await this._unitOfWork.PerformanceIndicators.GetAllAsync();
            var ris = await this._unitOfWork.ResultIndicators.GetAllAsync(); // Get all RIs

            allKpis.AddRange(kris); // Add KRIs
            allKpis.AddRange(pis);
            allKpis.AddRange(ris.Where(r => !r.IsKey)); // Add only non-key RIs

            // Populate available KPIs
            viewModel.AvailableIndicators = allKpis.Select(k => new IndicatorSummaryViewModel
            {
                Id = k.Id,
                Name = k is PerformanceIndicator pi ? pi.Name : (k is ResultIndicator ri ? ri.Name : "Unknown Indicator"),
                Code = k is PerformanceIndicator pi_c ? pi_c.Code : (k is ResultIndicator ri_c ? ri_c.Code : string.Empty),
                Department = (k is PerformanceIndicator pi_d ? pi_d.Department?.Name : (k is ResultIndicator ri_d ? ri_d.SuccessFactor?.Objective?.Department?.Name : string.Empty)) ?? string.Empty // Ensure non-null
            }).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Updates an existing custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to update.</param>
        /// <param name="viewModel">The updated dashboard configuration data.</param>
        /// <returns>Redirects to the updated dashboard if successful; otherwise returns to the edit form.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CustomDashboardViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return this.NotFound();
                }

                var userName = this.User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    this._logger.LogWarning("User {UserId} attempted to update unauthorized dashboard {DashboardId}",
                        userName, id);
                    return this.Forbid();
                }

                this._logger.LogInformation("Updating dashboard {DashboardId}", id);

                // Update dashboard properties
                dashboard.Title = viewModel.Title;
                dashboard.IsDefault = viewModel.IsDefault;
                dashboard.IsShared = viewModel.IsShared;
                dashboard.RefreshInterval = viewModel.RefreshInterval;
                dashboard.LayoutConfiguration = viewModel.LayoutConfiguration ?? string.Empty;
                dashboard.LastModified = DateTime.UtcNow;

                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully updated dashboard {DashboardId}", id);
                return this.RedirectToAction(nameof(this.Custom), new { id });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating dashboard {DashboardId}", id);
                this.ModelState.AddModelError("", "Error updating dashboard. Please try again.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Deletes a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard to delete.</param>
        /// <returns>Redirects to the custom dashboard list if successful.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                this._logger.LogInformation("Attempting to delete dashboard {DashboardId}", id);

                var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return this.NotFound();
                }

                var userName = this.User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    this._logger.LogWarning("User {UserId} attempted to delete unauthorized dashboard {DashboardId}",
                        userName, id);
                    return this.Forbid();
                }

                await this._unitOfWork.CustomDashboards.DeleteAsync(dashboard);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully deleted dashboard {DashboardId}", id);
                return this.RedirectToAction(nameof(this.Custom));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error deleting dashboard {DashboardId}", id);
                return this.RedirectToAction(nameof(this.Custom), new { id });
            }
        }

        /// <summary>
        /// Saves the layout configuration for a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="layout">The layout configuration data.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveLayout(Guid id, [FromBody] DashboardLayoutViewModel layout)
        {
            try
            {
                this._logger.LogInformation("Saving layout for dashboard {DashboardId}", id);

                var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return this.NotFound();
                }

                var userName = this.User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    this._logger.LogWarning("User {UserId} attempted to save layout for unauthorized dashboard {DashboardId}",
                        userName, id);
                    return this.Forbid();
                }

                dashboard.LayoutConfiguration = layout.Configuration;
                dashboard.LastModified = DateTime.UtcNow;

                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully saved layout for dashboard {DashboardId}", id);
                return this.Json(new { success = true });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error saving layout for dashboard {DashboardId}", id);
                return this.Json(new { success = false, message = "Error saving layout" });
            }
        }

        /// <summary>
        /// Adds a new item to a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="item">The dashboard item to add.</param>
        /// <returns>JSON result containing the new item ID if successful.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(Guid id, [FromBody] DashboardItemViewModel item)
        {
            try
            {
                this._logger.LogInformation("Adding item to dashboard {DashboardId}", id);

                var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return this.NotFound();
                }

                var userName = this.User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    this._logger.LogWarning("User {UserId} attempted to add item to unauthorized dashboard {DashboardId}",
                        userName, id);
                    return this.Forbid();
                }

                var dashboardItem = new DashboardItem
                {
                    DashboardId = id,
                    IndicatorId = item.IndicatorId,
                    SuccessFactorId = item.SuccessFactorId,
                    ChartType = (ChartType)(int)item.ChartType,
                    Title = item.Title,
                    Width = item.Width,
                    Height = item.Height,
                    X = item.X,
                    Y = item.Y,
                    DataConfiguration = item.DataConfiguration,
                    Order = item.Order,
                    ItemType = (DashboardItemType)(int)item.ItemType,
                    ShowLegend = item.ShowLegend,
                    TimePeriod = (DisplayTimePeriod)(int)item.TimePeriod
                };

                await this._unitOfWork.DashboardItems.AddAsync(dashboardItem);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully added item {ItemId} to dashboard {DashboardId}",
                    dashboardItem.Id, id);
                return this.Json(new { success = true, itemId = dashboardItem.Id });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error adding item to dashboard {DashboardId}", id);
                return this.Json(new { success = false, message = "Error adding item" });
            }
        }

        /// <summary>
        /// Removes an item from a custom dashboard.
        /// </summary>
        /// <param name="id">The unique identifier of the dashboard.</param>
        /// <param name="itemId">The unique identifier of the item to remove.</param>
        /// <returns>JSON result indicating success or failure.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
        {
            try
            {
                this._logger.LogInformation("Removing item {ItemId} from dashboard {DashboardId}", itemId, id);

                var dashboard = await this._unitOfWork.CustomDashboards.GetByIdAsync(id);
                if (dashboard == null)
                {
                    this._logger.LogWarning("Dashboard not found {DashboardId}", id);
                    return this.NotFound();
                }

                var userName = this.User.Identity?.Name ?? "unknown";
                if (dashboard.UserId != userName)
                {
                    this._logger.LogWarning("User {UserId} attempted to remove item from unauthorized dashboard {DashboardId}",
                        userName, id);
                    return this.Forbid();
                }

                var item = await this._unitOfWork.DashboardItems.GetByIdAsync(itemId);
                if (item == null || item.DashboardId != id)
                {
                    this._logger.LogWarning("Dashboard item not found {ItemId}", itemId);
                    return this.NotFound();
                }

                await this._unitOfWork.DashboardItems.DeleteAsync(item);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Successfully removed item {ItemId} from dashboard {DashboardId}", itemId, id);
                return this.Json(new { success = true });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error removing item {ItemId} from dashboard {DashboardId}", itemId, id);
                return this.Json(new { success = false, message = "Error removing item" });
            }
        }

        /// <summary>
        /// Helper method to check if user has access to a department
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="departmentId">Department ID</param>
        /// <returns>True if user has access, false otherwise</returns>
        private async Task<bool> UserHasAccessToDepartment(ClaimsPrincipal user, Guid departmentId)
        {
            // In a real implementation, this would check the user's roles and assigned departments
            // For now, allow access to administrators and executives, or if user is in the department
            if (user.IsInRole("Administrator") || user.IsInRole("Executive"))
            {
                return true;
            }

            // Implement actual check for user's department assignment
            var department = await this._unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department != null)
            {
                // Add logic to check if user belongs to department
                // For example:
                // var userDepartments = await _userService.GetUserDepartmentsAsync(user.Identity.Name);
                // return userDepartments.Any(d => d.Id == departmentId);
            }

            return true; // Temporarily allowing all access
        }

        /// <summary>
        /// Maps a CustomDashboard entity to a CustomDashboardViewModel
        /// </summary>
        private CustomDashboardViewModel MapToViewModel(CustomDashboard dashboard)
        {
            return new CustomDashboardViewModel
            {
                Id = dashboard.Id,
                Title = dashboard.Title ?? string.Empty,
                UserId = dashboard.UserId ?? string.Empty,
                UserName = dashboard.UserName ?? string.Empty,
                LastUpdated = dashboard.LastUpdated,
                LastModified = dashboard.LastModified,
                LayoutConfiguration = dashboard.LayoutConfiguration,
                IsDefault = dashboard.IsDefault,
                IsShared = dashboard.IsShared,
                RefreshInterval = dashboard.RefreshInterval,
                DashboardItems = dashboard.DashboardItems?.Select(item => new DashboardItemViewModel
                {
                    Id = item.Id,
                    IndicatorId = item.IndicatorId,
                    SuccessFactorId = item.SuccessFactorId,
                    ChartType = (KPISolution.Models.Enums.Visualization.ChartType)(int)item.ChartType,
                    Title = item.Title ?? string.Empty,
                    Width = item.Width,
                    Height = item.Height,
                    X = item.X,
                    Y = item.Y,
                    DataConfiguration = item.DataConfiguration,
                    Order = item.Order,
                    ItemType = (DashboardItemType)(int)item.ItemType,
                    ShowLegend = item.ShowLegend,
                    TimePeriod = (TimePeriod)(int)item.TimePeriod
                }).ToList() ?? []
            };
        }
    }
}
