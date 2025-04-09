using Microsoft.AspNetCore.Mvc.Rendering;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using KPISolution.Extensions;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing indicator measurements
    /// </summary>
    [Authorize]
    public class MeasurementController : BaseController
    {
        private readonly ILogger<MeasurementController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="unitOfWork">Unit of Work for data access</param>
        /// <param name="mapper">Mapper for object-object mapping</param>
        public MeasurementController(
            ILogger<MeasurementController> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Displays the list of all measurements with filtering and pagination
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        [HttpGet]
        public async Task<IActionResult> Index(MeasurementSearchViewModel? searchModel = null)
        {
            try
            {
                // Save current URL for navigation
                this.SaveCurrentUrlAsPrevious();

                this._logger.LogInformation("Getting measurements");

                searchModel ??= new MeasurementSearchViewModel();

                // Tạo MeasurementListViewModel thay vì MeasurementIndexViewModel
                var model = new MeasurementListViewModel
                {
                    // Ánh xạ từ MeasurementSearchViewModel sang IndicatorMeasurementFilterViewModel
                    Filter = new IndicatorMeasurementFilterViewModel
                    {
                        // Cần ánh xạ các thuộc tính phù hợp từ searchModel
                        SearchTerm = searchModel.IndicatorName,
                        DepartmentId = searchModel.DepartmentId
                        // Ánh xạ thêm các thuộc tính khác nếu cần
                    },
                    CurrentPage = searchModel.CurrentPage,
                    PageSize = searchModel.PageSize
                };

                // Populate dropdown lists
                model.Departments = await this.GetDepartmentsForDropdown();
                model.IndicatorTypes = this.GetIndicatorTypesForDropdown();
                model.MeasurementFrequencies = this.GetMeasurementFrequenciesForDropdown();

                // Get measurements with filters
                var query = this._unitOfWork.Measurements.GetAll();

                if (searchModel.IndicatorId.HasValue)
                {
                    // Look for measurements where PerformanceIndicatorId or ResultIndicatorId matches
                    query = query.Where(m =>
                        m.PerformanceIndicatorId == searchModel.IndicatorId ||
                        m.ResultIndicatorId == searchModel.IndicatorId);
                }

                if (!string.IsNullOrEmpty(searchModel.IndicatorName))
                {
                    // Fetch the indicators with matching names
                    var piIndicators = this._unitOfWork.PerformanceIndicators
                        .GetAll()
                        .Where(i => i.Name.Contains(searchModel.IndicatorName))
                        .Select(i => i.Id);

                    var riIndicators = this._unitOfWork.ResultIndicators
                        .GetAll()
                        .Where(i => i.Name.Contains(searchModel.IndicatorName))
                        .Select(i => i.Id);

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIndicators.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIndicators.Contains(m.ResultIndicatorId.Value)));
                }

                if (searchModel.DepartmentId.HasValue)
                {
                    // Find all indicators belonging to the department
                    var departmentPIs = this._unitOfWork.PerformanceIndicators
                        .GetAll()
                        .Where(i => i.DepartmentId == searchModel.DepartmentId)
                        .Select(i => i.Id);

                    var departmentRIs = this._unitOfWork.ResultIndicators
                        .GetAll()
                        .Where(i => i.DepartmentId == searchModel.DepartmentId)
                        .Select(i => i.Id);

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && departmentPIs.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && departmentRIs.Contains(m.ResultIndicatorId.Value)));
                }

                if (searchModel.FromDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate >= searchModel.FromDate.Value);
                }

                if (searchModel.ToDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate <= searchModel.ToDate.Value);
                }

                // Apply sorting
                query = query.OrderByDescending(m => m.MeasurementDate);

                // Get total count for pagination
                model.TotalCount = await query.CountAsync();

                // Apply pagination
                query = query.Skip((searchModel.CurrentPage - 1) * searchModel.PageSize)
                             .Take(searchModel.PageSize);

                // Execute query and load related data
                var measurements = await query.ToListAsync();

                // Map measurements to IndicatorValueViewModel instead of MeasurementViewModel
                model.Items = await this.MapToIndicatorValueViewModels(measurements);

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting measurements");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Maps measurement entities to IndicatorValueViewModel for display
        /// </summary>
        private async Task<List<IndicatorValueViewModel>> MapToIndicatorValueViewModels(List<Measurement> measurements)
        {
            var result = new List<IndicatorValueViewModel>();

            foreach (var measurement in measurements)
            {
                var item = new IndicatorValueViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    ActualValue = measurement.Value,
                    Status = this.GetStatusDisplayName(measurement.Status),
                    Notes = measurement.Notes,
                    CreatedBy = measurement.CreatedBy ?? "System"
                };

                // Set indicator information based on type
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (pi != null)
                    {
                        item.IndicatorId = pi.Id;
                        item.IndicatorCode = pi.Code;
                        item.IndicatorName = pi.Name;
                        item.TargetValue = pi.TargetValue;
                        item.AchievementPercentage = this.CalculateAchievementPercentage(measurement.Value, pi.TargetValue);
                        item.StatusCssClass = this.GetStatusCssClass(measurement.Status);
                        item.IndicatorType = pi.IsKey ? "KPI" : "PI";
                        item.Unit = pi.Unit;

                        // Log indicator type
                        this._logger.LogInformation("Set IndicatorType to {IndicatorType} for indicator {IndicatorCode}",
                            item.IndicatorType, item.IndicatorCode);
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (ri != null)
                    {
                        item.IndicatorId = ri.Id;
                        item.IndicatorCode = ri.Code;
                        item.IndicatorName = ri.Name;
                        item.TargetValue = ri.TargetValue;
                        item.AchievementPercentage = this.CalculateAchievementPercentage(measurement.Value, ri.TargetValue);
                        item.StatusCssClass = this.GetStatusCssClass(measurement.Status);
                        item.IndicatorType = ri.IsKey ? "KRI" : "RI";
                        item.Unit = ri.Unit;
                    }
                }

                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Calculate achievement percentage
        /// </summary>
        private decimal CalculateAchievementPercentage(decimal actual, decimal? target)
        {
            if (!target.HasValue || target.Value == 0)
                return 0;

            return Math.Round((actual / target.Value) * 100, 2);
        }

        /// <summary>
        /// Get CSS class for status display
        /// </summary>
        private string GetStatusCssClass(MeasurementStatus status)
        {
            return status switch
            {
                MeasurementStatus.Target => "badge bg-success",
                MeasurementStatus.Expected => "badge bg-warning",
                MeasurementStatus.Actual => "badge bg-primary",
                MeasurementStatus.NotSet => "badge bg-secondary",
                MeasurementStatus.Threshold => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }

        /// <summary>
        /// Get friendly display name for MeasurementStatus
        /// </summary>
        private string GetStatusDisplayName(MeasurementStatus status)
        {
            return status switch
            {
                MeasurementStatus.Target => "Mục tiêu",
                MeasurementStatus.Expected => "Dự kiến",
                MeasurementStatus.Actual => "Thực tế",
                MeasurementStatus.NotSet => "Chưa thiết lập",
                MeasurementStatus.Threshold => "Ngưỡng tối thiểu",
                _ => status.ToString()
            };
        }

        /// <summary>
        /// Get departments for dropdown
        /// </summary>
        private async Task<List<SelectListItem>> GetDepartmentsForDropdown()
        {
            var departments = await this._unitOfWork.Departments.GetAll().OrderBy(d => d.Name).ToListAsync();
            return departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Name
            }).ToList();
        }

        /// <summary>
        /// Get indicator types for dropdown
        /// </summary>
        private List<SelectListItem> GetIndicatorTypesForDropdown()
        {
            return
            [
                new SelectListItem { Value = ((int)IndicatorType.PI).ToString(), Text = "Chỉ số thực hiện (PI)" },
                new SelectListItem { Value = ((int)IndicatorType.KPI).ToString(), Text = "Chỉ số KPI" },
                new SelectListItem { Value = ((int)IndicatorType.RI).ToString(), Text = "Chỉ số kết quả (RI)" },
                new SelectListItem { Value = ((int)IndicatorType.KRI).ToString(), Text = "Chỉ số KRI" }
            ];
        }

        /// <summary>
        /// Get measurement frequencies for dropdown
        /// </summary>
        private List<SelectListItem> GetMeasurementFrequenciesForDropdown()
        {
            return
            [
                new SelectListItem { Value = "Daily", Text = "Hàng ngày" },
                new SelectListItem { Value = "Weekly", Text = "Hàng tuần" },
                new SelectListItem { Value = "Monthly", Text = "Hàng tháng" },
                new SelectListItem { Value = "Quarterly", Text = "Hàng quý" },
                new SelectListItem { Value = "Yearly", Text = "Hàng năm" }
            ];
        }

        /// <summary>
        /// Creates a measurement form for a specific indicator
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        [HttpGet]
        public async Task<IActionResult> Create(Guid indicatorId, IndicatorType type)
        {
            try
            {
                // Save return URL before displaying form
                this.SaveReturnUrl();

                // Validate indicator ID
                if (indicatorId == Guid.Empty)
                {
                    this._logger.LogWarning("Invalid indicator ID: Empty GUID provided");
                    return this.BadRequest("Invalid indicator ID provided.");
                }
                this._logger.LogInformation("Displaying create measurement form for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);

                // Default to current month
                var currentDate = DateTime.Now;
                var viewModel = new MeasurementCreateViewModel
                {
                    MeasurementDate = currentDate,
                    Type = this.GetIndicatorMeasurementType(type),
                    IndicatorId = indicatorId
                };

                // Set details based on indicator type
                switch (type)
                {
                    case IndicatorType.KPI:
                    case IndicatorType.PI:
                        var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(indicatorId);
                        if (pi != null)
                        {
                            viewModel.PerformanceIndicatorId = indicatorId;
                            viewModel.IndicatorName = pi.Name;
                            viewModel.IndicatorCode = pi.Code;
                            viewModel.Unit = pi.Unit;
                            viewModel.TargetValue = pi.TargetValue;
                            viewModel.Frequency = pi.MeasurementFrequency;
                            viewModel.MinAlertThreshold = pi.MinAlertThreshold;
                            viewModel.MaxAlertThreshold = pi.MaxAlertThreshold;
                        }
                        break;

                    case IndicatorType.RI:
                        var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(indicatorId);
                        if (ri != null)
                        {
                            viewModel.ResultIndicatorId = indicatorId;
                            viewModel.IndicatorName = ri.Name;
                            viewModel.IndicatorCode = ri.Code;
                            viewModel.Unit = ri.Unit;
                            viewModel.TargetValue = ri.TargetValue;
                            viewModel.Frequency = ri.MeasurementFrequency;
                        }
                        break;

                    case IndicatorType.SF:
                    case IndicatorType.CSF:
                        var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(indicatorId);
                        if (sf != null)
                        {
                            viewModel.SuccessFactorId = indicatorId;
                            viewModel.IndicatorName = sf.Name;
                            viewModel.IndicatorCode = sf.Code;
                            viewModel.Unit = sf.Unit;
                            viewModel.TargetValue = sf.TargetValue;
                            viewModel.Frequency = sf.MeasurementFrequency;
                        }
                        break;
                }

                if (string.IsNullOrEmpty(viewModel.IndicatorName))
                {
                    this._logger.LogWarning("Indicator with ID {Id} of type {Type} not found", indicatorId, type);
                    return this.NotFound($"Indicator with ID {indicatorId} of type {type} was not found.");
                }

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying measurement creation form for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while preparing the measurement form." });
            }
        }

        /// <summary>
        /// Processes the creation of a new measurement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        public async Task<IActionResult> Create(MeasurementCreateViewModel viewModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    this._logger.LogWarning("Invalid model state when creating measurement");
                    return this.View(viewModel);
                }

                this._logger.LogInformation("Creating new measurement for indicator: {Name}", viewModel.IndicatorName);

                // Tạo đối tượng Measurement từ ViewModel
                var measurement = new Measurement
                {
                    Value = viewModel.ActualValue,
                    MeasurementDate = viewModel.MeasurementDate,
                    Notes = viewModel.Notes,
                    Status = viewModel.Status
                };

                // Thiết lập ID chỉ số tương ứng dựa vào loại
                if (viewModel.PerformanceIndicatorId.HasValue)
                {
                    measurement.PerformanceIndicatorId = viewModel.PerformanceIndicatorId;
                    measurement.IndicatorType = IndicatorMeasurementType.PerformanceIndicator;
                }
                else if (viewModel.ResultIndicatorId.HasValue)
                {
                    measurement.ResultIndicatorId = viewModel.ResultIndicatorId;
                    measurement.IndicatorType = IndicatorMeasurementType.ResultIndicator;
                }
                else if (viewModel.SuccessFactorId.HasValue)
                {
                    measurement.SuccessFactorId = viewModel.SuccessFactorId;
                    measurement.IndicatorType = IndicatorMeasurementType.SuccessFactor;
                }
                else
                {
                    this._logger.LogError("No indicator ID set in the view model");
                    this.ModelState.AddModelError("", "No indicator ID set in the view model.");
                    return this.View(viewModel);
                }

                // Lưu đo lường mới vào cơ sở dữ liệu
                await this._unitOfWork.Measurements.AddAsync(measurement);
                await this._unitOfWork.SaveChangesAsync();

                // Cập nhật trạng thái của chỉ số tương ứng
                await this.UpdateIndicatorStatusAsync(measurement);

                this._logger.LogInformation("Successfully created measurement with ID: {Id}", measurement.Id);

                this.AddSuccessAlert("Đã thêm giá trị đo lường mới thành công.");

                return this.RedirectToPreviousPage();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while creating measurement");
                this.ModelState.AddModelError("", "An unexpected error occurred while saving the measurement.");
                return this.View(viewModel);
            }
        }

        /// <summary>
        /// Displays the measurement history for a specific indicator
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> History(Guid indicatorId, IndicatorType type)
        {
            try
            {
                this._logger.LogInformation("Viewing measurement history for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);

                // Chuẩn bị ViewModel để hiển thị lịch sử
                var viewModel = new MeasurementHistoryViewModel
                {
                    IndicatorId = indicatorId,
                    IndicatorType = type
                };

                IQueryable<Measurement> query = this._unitOfWork.Measurements.GetAll();

                // Lọc các phép đo phù hợp với loại chỉ số
                switch (type)
                {
                    case IndicatorType.KPI:
                    case IndicatorType.PI:
                        query = query.Where(m => m.PerformanceIndicatorId == indicatorId);
                        var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(indicatorId);
                        if (pi != null)
                        {
                            viewModel.IndicatorName = pi.Name;
                            viewModel.IndicatorCode = pi.Code;
                            viewModel.Unit = pi.Unit;
                            viewModel.TargetValue = pi.TargetValue;
                            if (pi.Department != null)
                            {
                                viewModel.Department = pi.Department.Name;
                            }
                        }
                        break;

                    case IndicatorType.RI:
                        query = query.Where(m => m.ResultIndicatorId == indicatorId);
                        var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(indicatorId);
                        if (ri != null)
                        {
                            viewModel.IndicatorName = ri.Name;
                            viewModel.IndicatorCode = ri.Code;
                            viewModel.Unit = ri.Unit;
                            viewModel.TargetValue = ri.TargetValue;
                            if (ri.Department != null)
                            {
                                viewModel.Department = ri.Department.Name;
                            }
                        }
                        break;

                    case IndicatorType.SF:
                    case IndicatorType.CSF:
                        query = query.Where(m => m.SuccessFactorId == indicatorId);
                        var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(indicatorId);
                        if (sf != null)
                        {
                            viewModel.IndicatorName = sf.Name;
                            viewModel.IndicatorCode = sf.Code;
                            viewModel.Unit = sf.Unit;
                            viewModel.TargetValue = sf.TargetValue;
                            if (sf.Department != null)
                            {
                                viewModel.Department = sf.Department.Name;
                            }
                        }
                        break;

                    default:
                        this._logger.LogWarning("Unsupported indicator type: {Type}", type);
                        return this.NotFound();
                }

                // Kiểm tra chỉ số tồn tại
                if (string.IsNullOrEmpty(viewModel.IndicatorName))
                {
                    this._logger.LogWarning("Indicator with ID {Id} of type {Type} not found", indicatorId, type);
                    return this.NotFound($"Indicator with ID {indicatorId} of type {type} was not found.");
                }

                // Lấy dữ liệu đo lường và sắp xếp theo thời gian
                var measurements = await query
                    .OrderByDescending(m => m.MeasurementDate)
                    .ToListAsync();

                // Chuyển đổi sang MeasurementItemViewModel
                var items = measurements.Select(m => new MeasurementItemViewModel
                {
                    Id = m.Id,
                    Value = m.Value,
                    MeasurementDate = m.MeasurementDate,
                    Status = m.Status,
                    Notes = m.Notes,
                    CreatedDate = m.CreatedAt,
                    Period = m.MeasurementDate.ToString("MMM yyyy")
                }).ToList();

                // Tính toán thêm thông tin chi tiết cho từng mục
                if (viewModel.TargetValue.HasValue)
                {
                    foreach (var item in items)
                    {
                        item.AchievementPercentage = Math.Round((item.Value / viewModel.TargetValue.Value) * 100, 2);
                        item.Variance = item.Value - viewModel.TargetValue.Value;
                    }
                }

                viewModel.Measurements = items;

                this._logger.LogInformation("Retrieved {0} measurements for indicator {1}", items.Count, viewModel.IndicatorName);

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving measurement history for indicator ID: {Id}, type: {Type}",
                    indicatorId, type);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving measurement history." });
            }
        }

        /// <summary>
        /// Builds a query with applied filters
        /// </summary>
        private IQueryable<Measurement> BuildFilteredQuery(IndicatorMeasurementFilterViewModel filter)
        {
            this._logger.LogInformation("Building filtered query for measurements");
            var query = this._unitOfWork.Measurements.GetAll();

            if (filter != null)
            {
                // Filter by search term (Indicator Code or Notes)
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.Trim().ToLower();
                    // Lấy ID của các Indicator có Code khớp
                    var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                        .Where(i => i.Code.ToLower().Contains(searchTerm))
                        .Select(i => i.Id)
                        .ToList();
                    var riIds = this._unitOfWork.ResultIndicators.GetAll()
                        .Where(i => i.Code.ToLower().Contains(searchTerm))
                        .Select(i => i.Id)
                        .ToList();
                    // Hoặc ghi chú chứa searchTerm
                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value)) ||
                        (m.Notes != null && m.Notes.ToLower().Contains(searchTerm)));
                }

                // Filter by department
                if (filter.DepartmentId.HasValue)
                {
                    // Requires joining with indicators to filter by department
                    var departmentId = filter.DepartmentId.Value;
                    var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                        .Where(i => i.DepartmentId == departmentId)
                        .Select(i => i.Id)
                        .ToList();

                    var riIds = this._unitOfWork.ResultIndicators.GetAll()
                        .Where(i => i.DepartmentId == departmentId)
                        .Select(i => i.Id)
                        .ToList();

                    query = query.Where(m =>
                        (m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value)) ||
                        (m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value)));
                }

                // Filter by indicator type (Sửa lại logic này)
                if (filter.IndicatorType.HasValue)
                {
                    var indicatorType = filter.IndicatorType.Value;
                    switch (indicatorType)
                    {
                        case IndicatorType.KPI:
                            // KPI = PerformanceIndicator where IsKey = true
                            var kpiIds = this._unitOfWork.PerformanceIndicators.GetAll()
                                .Where(pi => pi.IsKey)
                                .Select(pi => pi.Id)
                                .ToList();
                            query = query.Where(m => m.PerformanceIndicatorId.HasValue && kpiIds.Contains(m.PerformanceIndicatorId.Value));
                            break;
                        case IndicatorType.PI:
                            // PI = PerformanceIndicator where IsKey = false
                            var piIds = this._unitOfWork.PerformanceIndicators.GetAll()
                                .Where(pi => !pi.IsKey)
                                .Select(pi => pi.Id)
                                .ToList();
                            query = query.Where(m => m.PerformanceIndicatorId.HasValue && piIds.Contains(m.PerformanceIndicatorId.Value));
                            break;
                        case IndicatorType.KRI:
                            // KRI = ResultIndicator where IsKey = true
                            var kriIds = this._unitOfWork.ResultIndicators.GetAll()
                                .Where(ri => ri.IsKey)
                                .Select(ri => ri.Id)
                                .ToList();
                            query = query.Where(m => m.ResultIndicatorId.HasValue && kriIds.Contains(m.ResultIndicatorId.Value));
                            break;
                        case IndicatorType.RI:
                            // RI = ResultIndicator where IsKey = false
                            var riIds = this._unitOfWork.ResultIndicators.GetAll()
                                .Where(ri => !ri.IsKey)
                                .Select(ri => ri.Id)
                                .ToList();
                            query = query.Where(m => m.ResultIndicatorId.HasValue && riIds.Contains(m.ResultIndicatorId.Value));
                            break;
                            // Thêm trường hợp cho SuccessFactor/CSF nếu cần thiết
                            // case IndicatorType.SF:
                            // case IndicatorType.CSF:
                            //     var sfIds = _unitOfWork.SuccessFactors.GetAll().Select(sf => sf.Id).ToList();
                            //     query = query.Where(m => m.SuccessFactorId.HasValue && sfIds.Contains(m.SuccessFactorId.Value));
                            //     break;
                    }
                }

                // Filter by date range (support both old and new property names)
                if (filter.StartDate.HasValue || filter.FromDate.HasValue)
                {
                    var startDate = filter.StartDate ?? filter.FromDate;
                    if (startDate.HasValue)
                    {
                        query = query.Where(m => m.MeasurementDate >= startDate.Value);
                    }
                }

                if (filter.EndDate.HasValue || filter.ToDate.HasValue)
                {
                    var endDate = filter.EndDate ?? filter.ToDate;
                    if (endDate.HasValue)
                    {
                        query = query.Where(m => m.MeasurementDate <= endDate.Value);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// Updates the status of an indicator based on its measurements
        /// </summary>
        private async Task UpdateIndicatorStatusAsync(Measurement measurement)
        {
            try
            {
                this._logger.LogInformation("Updating indicator status based on new measurement");

                // Cập nhật trạng thái của chỉ số dựa trên phép đo mới nhất
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var pi = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (pi != null && pi.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / pi.TargetValue.Value) * 100;

                        // Cập nhật trạng thái dựa trên phần trăm đạt được
                        pi.CurrentValue = measurement.Value;
                        pi.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật IndicatorStatus khi xác định được enum phù hợp
                        /*
                        if (achievementPercentage >= 100)
                        {
                            pi.Status = IndicatorStatus.OnTarget;
                        }
                        else if (achievementPercentage >= 80)
                        {
                            pi.Status = IndicatorStatus.AtRisk;
                }
                else
                {
                            pi.Status = IndicatorStatus.BelowTarget;
                        }
                        */

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var ri = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (ri != null && ri.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / ri.TargetValue.Value) * 100;

                        // Cập nhật trạng thái
                        ri.CurrentValue = measurement.Value;
                        ri.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật IndicatorStatus khi xác định được enum phù hợp

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
                else if (measurement.SuccessFactorId.HasValue)
                {
                    var sf = await this._unitOfWork.SuccessFactors.GetByIdAsync(measurement.SuccessFactorId.Value);
                    if (sf != null && sf.TargetValue.HasValue)
                    {
                        var achievementPercentage = (measurement.Value / sf.TargetValue.Value) * 100;

                        // Cập nhật trạng thái
                        sf.CurrentValue = measurement.Value;
                        sf.LastUpdated = DateTime.UtcNow;

                        // TODO: Cập nhật SuccessFactorStatus khi xác định được enum phù hợp

                        await this._unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error updating indicator status");
                throw;
            }
        }

        /// <summary>
        /// Maps IndicatorType to IndicatorMeasurementType
        /// </summary>
        private IndicatorMeasurementType GetIndicatorMeasurementType(IndicatorType type)
        {
            return type switch
            {
                IndicatorType.KPI or IndicatorType.PI => IndicatorMeasurementType.PerformanceIndicator,
                IndicatorType.RI => IndicatorMeasurementType.ResultIndicator,
                IndicatorType.SF or IndicatorType.CSF => IndicatorMeasurementType.SuccessFactor,
                _ => throw new ArgumentException($"Unsupported indicator type: {type}")
            };
        }

        /// <summary>
        /// Gets cached department select list
        /// </summary>
        private async Task<List<SelectListItem>> GetDepartmentSelectListCached()
        {
            var departments = await this._unitOfWork.Departments.GetAllAsync();
            return departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                }).ToList();
        }

        /// <summary>
        /// Gets cached enum select list
        /// </summary>
        private List<SelectListItem> GetEnumSelectListCached<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = e.GetDisplayName()
                }).ToList();
        }

        private async Task<List<MeasurementViewModel>> EnrichMeasurements(List<Measurement> measurements)
        {
            var result = new List<MeasurementViewModel>();

            foreach (var measurement in measurements)
            {
                var viewModel = new MeasurementViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    Value = measurement.Value,
                    Status = measurement.Status,
                    Notes = measurement.Notes
                };

                // Check if status should be NotSet
                if (!measurement.Status.Equals(MeasurementStatus.NotSet) && !viewModel.TargetValue.HasValue)
                {
                    viewModel.Status = MeasurementStatus.NotSet;
                }

                // Determine indicator type and get details
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var performanceIndicator = await this._unitOfWork.PerformanceIndicators
                        .GetByIdAsync(measurement.PerformanceIndicatorId.Value);

                    if (performanceIndicator != null)
                    {
                        viewModel.IndicatorId = performanceIndicator.Id;
                        viewModel.IndicatorName = performanceIndicator.Name;
                        viewModel.IndicatorType = performanceIndicator.IsKey ? "KPI" : "PI";
                        viewModel.DepartmentName = performanceIndicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = performanceIndicator.Unit;
                        viewModel.TargetValue = performanceIndicator.TargetValue;

                        // Set status to NotSet if no target value is available
                        if (!viewModel.TargetValue.HasValue && viewModel.Status != MeasurementStatus.NotSet)
                        {
                            viewModel.Status = MeasurementStatus.NotSet;
                        }

                        result.Add(viewModel);
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var resultIndicator = await this._unitOfWork.ResultIndicators
                        .GetByIdAsync(measurement.ResultIndicatorId.Value);

                    if (resultIndicator != null)
                    {
                        viewModel.IndicatorId = resultIndicator.Id;
                        viewModel.IndicatorName = resultIndicator.Name;
                        viewModel.IndicatorType = resultIndicator.IsKey ? "KRI" : "RI";
                        viewModel.DepartmentName = resultIndicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = resultIndicator.Unit;
                        viewModel.TargetValue = resultIndicator.TargetValue;

                        // Set status to NotSet if no target value is available
                        if (!viewModel.TargetValue.HasValue && viewModel.Status != MeasurementStatus.NotSet)
                        {
                            viewModel.Status = MeasurementStatus.NotSet;
                        }

                        result.Add(viewModel);
                    }
                }
                else if (measurement.SuccessFactorId.HasValue)
                {
                    var successFactor = await this._unitOfWork.SuccessFactors
                        .GetByIdAsync(measurement.SuccessFactorId.Value);

                    if (successFactor != null)
                    {
                        viewModel.IndicatorId = successFactor.Id;
                        viewModel.IndicatorName = successFactor.Name;
                        viewModel.IndicatorType = "SF";
                        viewModel.DepartmentName = successFactor.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = successFactor.Unit ?? string.Empty;
                        viewModel.TargetValue = successFactor.TargetValue;
                        result.Add(viewModel);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Hiển thị biểu đồ theo dõi chỉ số theo thời gian
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> Chart(Guid indicatorId, IndicatorType type)
        {
            try
            {
                var measurements = await this._unitOfWork.Measurements
                    .GetAll()
                    .Where(m =>
                        (type == IndicatorType.PI && m.PerformanceIndicatorId == indicatorId) ||
                        (type == IndicatorType.RI && m.ResultIndicatorId == indicatorId))
                    .OrderBy(m => m.MeasurementDate)
                    .ToListAsync();

                var model = new MeasurementChartViewModel
                {
                    IndicatorId = indicatorId,
                    IndicatorType = type,
                    Labels = measurements.Select(m => m.MeasurementDate.ToString("dd/MM/yyyy")).ToList(),
                    ActualValues = measurements.Select(m => m.Value).ToList()
                };

                // Lấy thông tin chỉ tiêu
                if (type == IndicatorType.PI)
                {
                    var indicator = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(indicatorId);
                    if (indicator != null)
                    {
                        model.IndicatorName = indicator.Name;
                        model.TargetValue = indicator.TargetValue;
                        model.Unit = indicator.Unit;
                    }
                }
                else
                {
                    var indicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(indicatorId);
                    if (indicator != null)
                    {
                        model.IndicatorName = indicator.Name;
                        model.TargetValue = indicator.TargetValue;
                        model.Unit = indicator.Unit;
                    }
                }

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting measurement chart data");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Hiển thị trang đo lường chỉ số hiệu suất (PI/KPI)
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> PerformanceIndicators()
        {
            try
            {
                var model = new MeasurementListViewModel
                {
                    Filter = new IndicatorMeasurementFilterViewModel(),
                    CurrentPage = 1,
                    PageSize = 10
                };

                // Chỉ lấy các đo lường của PI/KPI
                var query = this._unitOfWork.Measurements
                    .GetAll()
                    .Where(m => m.PerformanceIndicatorId.HasValue);

                // Get total count for pagination
                model.TotalCount = await query.CountAsync();

                // Apply pagination
                query = query.Skip((model.CurrentPage - 1) * model.PageSize)
                             .Take(model.PageSize);

                // Execute query and load related data
                var measurements = await query.ToListAsync();

                // Map measurements to view models
                model.Items = await this.MapToIndicatorValueViewModels(measurements);

                // Populate dropdown lists
                model.Departments = await this.GetDepartmentsForDropdown();
                model.IndicatorTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "KPI", Text = "KPI" },
                    new SelectListItem { Value = "PI", Text = "PI" }
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting performance indicator measurements");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Hiển thị trang đo lường chỉ số kết quả (RI/KRI)
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> ResultIndicators()
        {
            try
            {
                var model = new MeasurementListViewModel
                {
                    Filter = new IndicatorMeasurementFilterViewModel(),
                    CurrentPage = 1,
                    PageSize = 10
                };

                // Chỉ lấy các đo lường của RI/KRI
                var query = this._unitOfWork.Measurements
                    .GetAll()
                    .Where(m => m.ResultIndicatorId.HasValue);

                // Get total count for pagination
                model.TotalCount = await query.CountAsync();

                // Apply pagination
                query = query.Skip((model.CurrentPage - 1) * model.PageSize)
                             .Take(model.PageSize);

                // Execute query and load related data
                var measurements = await query.ToListAsync();

                // Map measurements to view models
                model.Items = await this.MapToIndicatorValueViewModels(measurements);

                // Populate dropdown lists
                model.Departments = await this.GetDepartmentsForDropdown();
                model.IndicatorTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "KRI", Text = "KRI" },
                    new SelectListItem { Value = "RI", Text = "RI" }
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error getting result indicator measurements");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Xuất báo cáo đo lường theo kỳ
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> Report(DateTime? fromDate, DateTime? toDate, Guid? departmentId)
        {
            try
            {
                var query = this._unitOfWork.Measurements.GetAll();

                if (fromDate.HasValue)
                    query = query.Where(m => m.MeasurementDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(m => m.MeasurementDate <= toDate.Value);

                var measurements = await query.ToListAsync();

                var model = new MeasurementReportViewModel
                {
                    FromDate = fromDate ?? DateTime.Today.AddMonths(-1),
                    ToDate = toDate ?? DateTime.Today,
                    DepartmentId = departmentId,
                    Departments = await this.GetDepartmentsForDropdown(),
                    Items = await this.MapToIndicatorValueViewModels(measurements)
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error generating measurement report");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Xuất báo cáo ra Excel
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        public async Task<IActionResult> ExportToExcel(DateTime? fromDate, DateTime? toDate, Guid? departmentId)
        {
            try
            {
                var query = this._unitOfWork.Measurements.GetAll();

                if (fromDate.HasValue)
                    query = query.Where(m => m.MeasurementDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(m => m.MeasurementDate <= toDate.Value);

                var measurements = await query.ToListAsync();
                var items = await this.MapToIndicatorValueViewModels(measurements);

                // Tạo file Excel
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Measurements");

                // Thêm header
                worksheet.Cell(1, 1).Value = "Mã chỉ số";
                worksheet.Cell(1, 2).Value = "Tên chỉ số";
                worksheet.Cell(1, 3).Value = "Ngày đo";
                worksheet.Cell(1, 4).Value = "Giá trị thực tế";
                worksheet.Cell(1, 5).Value = "Giá trị mục tiêu";
                worksheet.Cell(1, 6).Value = "Tỷ lệ đạt";
                worksheet.Cell(1, 7).Value = "Trạng thái";
                worksheet.Cell(1, 8).Value = "Ghi chú";

                // Thêm dữ liệu
                var row = 2;
                foreach (var item in items)
                {
                    worksheet.Cell(row, 1).Value = item.IndicatorCode;
                    worksheet.Cell(row, 2).Value = item.IndicatorName;
                    worksheet.Cell(row, 3).Value = item.MeasurementDate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 4).Value = item.ActualValue;
                    worksheet.Cell(row, 5).Value = item.TargetValue;
                    worksheet.Cell(row, 6).Value = item.AchievementPercentage;
                    worksheet.Cell(row, 7).Value = item.Status;
                    worksheet.Cell(row, 8).Value = item.Notes;
                    row++;
                }

                // Format header
                var header = worksheet.Range(1, 1, 1, 8);
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Convert to byte array
                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                // Return file
                return this.File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Measurements_{DateTime.Now:yyyyMMdd}.xlsx"
                );
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error exporting measurements to Excel");
                return this.View("Error");
            }
        }

        /// <summary>
        /// Displays details of a specific measurement
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanViewIndicators)]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying details for measurement ID: {Id}", id);

                // Get the measurement
                var measurement = await this._unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null)
                {
                    this._logger.LogWarning("Measurement not found with ID: {Id}", id);
                    return this.NotFound();
                }

                // Create view model
                var viewModel = new MeasurementViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    Value = measurement.Value,
                    Status = measurement.Status,
                    Notes = measurement.Notes,
                    CreatedAt = measurement.CreatedAt,
                    CreatedBy = measurement.CreatedBy ?? "System",
                    UpdatedAt = measurement.UpdatedAt,
                    UpdatedBy = measurement.UpdatedBy
                };

                // Get indicator information
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var indicator = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (indicator != null)
                    {
                        viewModel.IndicatorId = indicator.Id;
                        viewModel.IndicatorName = indicator.Name;
                        viewModel.IndicatorType = indicator.IsKey ? "KPI" : "PI";
                        viewModel.DepartmentName = indicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = indicator.Unit.ToString();
                        viewModel.TargetValue = indicator.TargetValue;

                        // AchievementPercentage and Variance are calculated automatically
                        // from Value and TargetValue
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var indicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (indicator != null)
                    {
                        viewModel.IndicatorId = indicator.Id;
                        viewModel.IndicatorName = indicator.Name;
                        viewModel.IndicatorType = "RI";
                        viewModel.DepartmentName = indicator.Department?.Name ?? "Unknown";
                        viewModel.IndicatorUnit = indicator.Unit.ToString();
                        viewModel.TargetValue = indicator.TargetValue;

                        // AchievementPercentage and Variance are calculated automatically
                        // from Value and TargetValue
                    }
                }

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while retrieving measurement details for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while retrieving measurement details." });
            }
        }

        /// <summary>
        /// Displays form to edit an existing measurement
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                this._logger.LogInformation("Displaying edit form for measurement ID: {Id}", id);

                // Save return URL before displaying form
                this.SaveReturnUrl();

                // Get the measurement
                var measurement = await this._unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null)
                {
                    this._logger.LogWarning("Measurement not found with ID: {Id}", id);
                    return this.NotFound();
                }

                // Create view model
                var viewModel = new MeasurementEditViewModel
                {
                    Id = measurement.Id,
                    MeasurementDate = measurement.MeasurementDate,
                    Value = measurement.Value,
                    Status = measurement.Status,
                    Notes = measurement.Notes
                };

                // Get indicator information
                if (measurement.PerformanceIndicatorId.HasValue)
                {
                    var indicator = await this._unitOfWork.PerformanceIndicators.GetByIdAsync(measurement.PerformanceIndicatorId.Value);
                    if (indicator != null)
                    {
                        viewModel.PerformanceIndicatorId = indicator.Id;
                        viewModel.IndicatorId = indicator.Id;
                        viewModel.IndicatorName = indicator.Name;
                        viewModel.IndicatorCode = indicator.Code;
                        viewModel.IndicatorType = indicator.IsKey ? IndicatorType.KPI : IndicatorType.PI;
                        viewModel.Unit = Enum.TryParse<MeasurementUnit>(indicator.Unit, out var unit) ? unit : MeasurementUnit.Other;
                        viewModel.TargetValue = indicator.TargetValue;
                        viewModel.MinAlertThreshold = indicator.MinAlertThreshold;
                        viewModel.MaxAlertThreshold = indicator.MaxAlertThreshold;
                    }
                }
                else if (measurement.ResultIndicatorId.HasValue)
                {
                    var indicator = await this._unitOfWork.ResultIndicators.GetByIdAsync(measurement.ResultIndicatorId.Value);
                    if (indicator != null)
                    {
                        viewModel.ResultIndicatorId = indicator.Id;
                        viewModel.IndicatorId = indicator.Id;
                        viewModel.IndicatorName = indicator.Name;
                        viewModel.IndicatorCode = indicator.Code;
                        viewModel.IndicatorType = IndicatorType.RI;
                        viewModel.Unit = Enum.TryParse<MeasurementUnit>(indicator.Unit, out var unit) ? unit : MeasurementUnit.Other;
                        viewModel.TargetValue = indicator.TargetValue;
                    }
                }
                else if (measurement.SuccessFactorId.HasValue)
                {
                    var indicator = await this._unitOfWork.SuccessFactors.GetByIdAsync(measurement.SuccessFactorId.Value);
                    if (indicator != null)
                    {
                        viewModel.SuccessFactorId = indicator.Id;
                        viewModel.IndicatorId = indicator.Id;
                        viewModel.IndicatorName = indicator.Name;
                        viewModel.IndicatorCode = indicator.Code;
                        viewModel.IndicatorType = IndicatorType.CSF;
                        viewModel.Unit = Enum.TryParse<MeasurementUnit>(indicator.Unit, out var unit) ? unit : MeasurementUnit.Other;
                        viewModel.TargetValue = indicator.TargetValue;
                    }
                }

                // Prepare status list
                viewModel.StatusList = this.GetStatusSelectList();

                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while preparing measurement edit form for ID: {Id}", id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while preparing the edit form." });
            }
        }

        /// <summary>
        /// Processes the update of an existing measurement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        public async Task<IActionResult> Edit(MeasurementEditViewModel viewModel)
        {
            try
            {
                // Validate model
                if (!this.ModelState.IsValid)
                {
                    this._logger.LogWarning("Invalid model state when editing measurement: {Errors}",
                        string.Join("; ", this.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

                    // Prepare status list again
                    viewModel.StatusList = this.GetStatusSelectList();

                    return this.View(viewModel);
                }

                // Get existing measurement
                var measurement = await this._unitOfWork.Measurements.GetByIdAsync(viewModel.Id);
                if (measurement == null)
                {
                    this._logger.LogWarning("Measurement not found with ID: {Id}", viewModel.Id);
                    return this.NotFound();
                }

                // Update measurement properties
                measurement.MeasurementDate = viewModel.MeasurementDate;
                measurement.Value = viewModel.Value;
                measurement.Status = viewModel.Status;
                measurement.Notes = viewModel.Notes;

                // Save changes
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Measurement with ID {Id} updated successfully", viewModel.Id);

                // Redirect to return URL or index
                var returnUrl = this.GetReturnUrl();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return this.Redirect(returnUrl);
                }

                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while updating measurement with ID: {Id}", viewModel.Id);
                return this.View("Error", new ErrorViewModel { Message = "An error occurred while updating the measurement." });
            }
        }

        /// <summary>
        /// Get select list for measurement status
        /// </summary>
        private IEnumerable<SelectListItem> GetStatusSelectList()
        {
            var statusValues = Enum.GetValues(typeof(MeasurementStatus)).Cast<MeasurementStatus>();
            return statusValues.Select(status => new SelectListItem
            {
                Value = status.ToString(),
                Text = status.ToString()
            });
        }

        /// <summary>
        /// Get the return URL from TempData
        /// </summary>
        private string? GetReturnUrl()
        {
            const string ReturnUrlKey = "ReturnUrl";
            return this.TempData[ReturnUrlKey]?.ToString();
        }

        /// <summary>
        /// Deletes a measurement and returns JSON result
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                this._logger.LogInformation("Deleting measurement with ID: {Id}", id);

                var measurement = await this._unitOfWork.Measurements.GetByIdAsync(id);
                if (measurement == null)
                {
                    this._logger.LogWarning("Measurement not found with ID: {Id}", id);
                    return this.Json(new { success = false, message = "Không tìm thấy đo lường này." });
                }

                await this._unitOfWork.Measurements.DeleteAsync(measurement);
                await this._unitOfWork.SaveChangesAsync();

                this._logger.LogInformation("Measurement with ID {Id} deleted successfully", id);

                return this.Json(new { success = true, message = "Đã xóa đo lường thành công." });
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while deleting measurement with ID: {Id}", id);
                return this.Json(new { success = false, message = "Có lỗi xảy ra khi xóa đo lường." });
            }
        }

        /// <summary>
        /// Hiển thị trang chọn chỉ số trước khi tạo đo lường mới
        /// </summary>
        [Authorize(Policy = IndicatorAuthorizationPolicies.PolicyNames.CanManageIndicators)]
        [HttpGet]
        public async Task<IActionResult> SelectIndicator()
        {
            try
            {
                this._logger.LogInformation("Displaying indicator selection page for measurement creation");

                var model = new SelectIndicatorViewModel
                {
                    PerformanceIndicators = await this._unitOfWork.PerformanceIndicators.GetAll()
                        .Include(pi => pi.Department)
                        .OrderBy(pi => pi.Name)
                        .Select(pi => new IndicatorSelectionItemViewModel
                        {
                            Id = pi.Id,
                            Name = pi.Name,
                            Code = pi.Code,
                            IsKey = pi.IsKey,
                            Type = pi.IsKey ? IndicatorType.KPI : IndicatorType.PI,
                            Department = pi.Department != null ? pi.Department.Name : "N/A",
                            LastMeasurementDate = this._unitOfWork.Measurements.GetAll()
                                .Where(m => m.PerformanceIndicatorId == pi.Id)
                                .OrderByDescending(m => m.MeasurementDate)
                                .Select(m => m.MeasurementDate)
                                .FirstOrDefault()
                        })
                        .ToListAsync(),

                    ResultIndicators = await this._unitOfWork.ResultIndicators.GetAll()
                        .Include(ri => ri.Department)
                        .OrderBy(ri => ri.Name)
                        .Select(ri => new IndicatorSelectionItemViewModel
                        {
                            Id = ri.Id,
                            Name = ri.Name,
                            Code = ri.Code,
                            IsKey = ri.IsKey,
                            Type = ri.IsKey ? IndicatorType.KRI : IndicatorType.RI,
                            Department = ri.Department != null ? ri.Department.Name : "N/A",
                            LastMeasurementDate = this._unitOfWork.Measurements.GetAll()
                                .Where(m => m.ResultIndicatorId == ri.Id)
                                .OrderByDescending(m => m.MeasurementDate)
                                .Select(m => m.MeasurementDate)
                                .FirstOrDefault()
                        })
                        .ToListAsync()
                };

                return this.View(model);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error occurred while displaying indicator selection page");
                return this.View("Error", new ErrorViewModel { Message = "Đã xảy ra lỗi khi tải trang chọn chỉ số." });
            }
        }
    }
}
