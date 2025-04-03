using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KPISolution.Models.ViewModels.KPI;
using KPISolution.Authorization;
using KPISolution.Models.Enums;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Base;
using KPISolution.Data;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Data.Repositories.Extensions;
using AutoMapper;
using KPISolution.Extensions;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing KPI measurements
    /// </summary>
    [Authorize]
    public class MeasurementController : Controller
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
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Displays the list of all measurements with filtering and pagination
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Index(MeasurementFilterViewModel filter, int page = 1)
        {
            try
            {
                var pageSize = 10;

                // Tạo truy vấn cơ bản với bộ lọc
                var query = BuildFilteredQuery(filter);

                // Đếm tổng số bản ghi (truy vấn COUNT hiệu quả hơn)
                var totalCount = await query.CountAsync();

                // Áp dụng sắp xếp và phân trang ở cấp cơ sở dữ liệu
                var pagedData = await query
                    .OrderByDescending(m => m.MeasurementDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(m => m.Kpi) // Đảm bảo load dữ liệu KPI
                    .ToListAsync();

                // Chuyển đổi thành viewmodels
                var items = pagedData.Select(value =>
                {
                    var viewModel = _mapper.Map<KpiValueViewModel>(value);

                    // Bổ sung thông tin từ KPI
                    if (value.Kpi != null)
                    {
                        viewModel.TargetValue = value.Kpi.TargetValue;
                        viewModel.KpiType = value.Kpi.GetType().Name;
                        viewModel.KpiCode = value.Kpi.Code;
                        viewModel.KpiName = value.Kpi.Name;

                        // Tính toán phần trăm đạt được
                        if (value.Kpi.TargetValue.HasValue && value.Kpi.TargetValue > 0)
                        {
                            viewModel.AchievementPercentage = Math.Round((value.ActualValue / value.Kpi.TargetValue.Value) * 100, 2);
                            viewModel.Variance = value.ActualValue - value.Kpi.TargetValue.Value;

                            // Thiết lập trạng thái dựa trên phần trăm đạt được
                            if (viewModel.AchievementPercentage >= 100)
                            {
                                viewModel.Status = "Đạt mục tiêu";
                                viewModel.StatusCssClass = "badge bg-success";
                            }
                            else if (viewModel.AchievementPercentage >= 80)
                            {
                                viewModel.Status = "Có rủi ro";
                                viewModel.StatusCssClass = "badge bg-warning text-dark";
                            }
                            else
                            {
                                viewModel.Status = "Không đạt";
                                viewModel.StatusCssClass = "badge bg-danger";
                            }
                        }

                        // Thiết lập kỳ dựa trên ngày đo lường
                        viewModel.Period = value.MeasurementDate.ToString("MMM yyyy");
                    }

                    return viewModel;
                }).ToList();

                // Tạo viewmodel với dữ liệu đã phân trang
                var viewModel = new MeasurementListViewModel
                {
                    Filter = filter ?? new MeasurementFilterViewModel(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Items = items,
                    Departments = new SelectList(await GetDepartmentSelectListCached(), "Value", "Text"),
                    KpiTypes = new SelectList(GetEnumSelectListCached<KpiType>(), "Value", "Text"),
                    MeasurementFrequencies = new SelectList(GetEnumSelectListCached<MeasurementFrequency>(), "Value", "Text")
                };

                _logger.LogInformation($"Số lượng KPI trong ViewModel: {viewModel.Items.Count}");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi truy xuất danh sách đo lường");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Tạo truy vấn lọc cơ bản cho KpiValues
        /// </summary>
        private IQueryable<KpiValue> BuildFilteredQuery(MeasurementFilterViewModel? filter)
        {
            // Bắt đầu với một IQueryable đơn giản và không theo dõi cho hiệu suất tối ưu
            var dbContext = GetDbContextFromUnitOfWork();
            IQueryable<KpiValue> query = dbContext.KpiValues.AsNoTracking();

            // Áp dụng các bộ lọc
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(m =>
                        (m.Notes != null && m.Notes.Contains(filter.SearchTerm)) ||
                        (m.Kpi != null && (m.Kpi.Name.Contains(filter.SearchTerm) || m.Kpi.Code.Contains(filter.SearchTerm)))
                    );
                }

                if (filter.StartDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate <= filter.EndDate.Value);
                }

                if (filter.KpiType.HasValue)
                {
                    string typeName = filter.KpiType switch
                    {
                        KpiType.KeyResultIndicator => nameof(KRI),
                        KpiType.ResultIndicator => nameof(RI),
                        KpiType.PerformanceIndicator => nameof(PI),
                        KpiType.StandaloneKPI => nameof(Models.Entities.KPI.KPI),
                        _ => ""
                    };

                    if (!string.IsNullOrEmpty(typeName))
                    {
                        // Sử dụng tên loại để lọc
                        query = query.Where(m => m.Kpi != null && EF.Property<string>(m.Kpi, "Discriminator") == typeName);
                    }
                }

                if (filter.DepartmentId.HasValue)
                {
                    // Truy vấn phụ lấy tên phòng ban theo ID
                    var department = dbContext.Departments
                        .Where(d => d.Id == filter.DepartmentId.Value)
                        .Select(d => d.Name)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(department))
                    {
                        query = query.Where(m => m.Kpi != null && m.Kpi.Department == department);
                    }
                }

                if (filter.Frequency.HasValue)
                {
                    query = query.Where(m => m.Kpi != null && m.Kpi.Frequency == filter.Frequency.Value);
                }
            }

            return query;
        }

        /// <summary>
        /// Helper để truy cập ApplicationDbContext từ UnitOfWork
        /// </summary>
        private ApplicationDbContext GetDbContextFromUnitOfWork()
        {
            // Truy cập DbContext từ UnitOfWork bằng reflection
            var dbContext = _unitOfWork.GetType()
                .GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(_unitOfWork) as ApplicationDbContext;

            if (dbContext == null)
            {
                throw new InvalidOperationException("Không thể truy cập DbContext từ UnitOfWork");
            }

            return dbContext;
        }

        /// <summary>
        /// Displays the form for adding a new measurement
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Add(Guid kpiId)
        {
            try
            {
                var kpi = await FindKpiByIdAsync(kpiId);
                if (kpi == null)
                    return NotFound();

                var viewModel = new AddMeasurementViewModel
                {
                    KpiId = kpiId,
                    KpiCode = kpi.Code,
                    KpiName = kpi.Name,
                    MeasurementUnit = kpi.Unit,
                    MeasurementDate = DateTime.Now,
                    TargetValue = kpi.TargetValue
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing add measurement form for KPI: {KpiId}", kpiId);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for adding a new measurement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Add(AddMeasurementViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var kpi = await FindKpiByIdAsync(viewModel.KpiId);
                if (kpi == null)
                {
                    ModelState.AddModelError("", "KPI not found");
                    return View(viewModel);
                }

                var kpiValue = new KpiValue
                {
                    KpiId = viewModel.KpiId,
                    ActualValue = viewModel.ActualValue,
                    MeasurementDate = viewModel.MeasurementDate,
                    Notes = viewModel.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.GetUserId()
                };

                await _unitOfWork.KpiValues.AddAsync(kpiValue);
                await _unitOfWork.SaveChangesAsync();

                // Update KPI's current value and status
                await UpdateKpiStatusAsync(viewModel.KpiId);

                return RedirectToAction("Details", "Kpi", new { id = viewModel.KpiId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving measurement for KPI: {KpiId}", viewModel.KpiId);
                ModelState.AddModelError("", "An error occurred while saving the measurement. Please try again.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Displays historical measurements for a specific KPI
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> History(Guid? kpiId, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            try
            {
                var pageSize = 10;
                var dbContext = GetDbContextFromUnitOfWork();

                // Xây dựng truy vấn cơ bản
                var query = dbContext.KpiValues.AsNoTracking();

                // Thêm Include để load Kpi
                query = query.Include(m => m.Kpi);

                // Lọc theo KPI cụ thể nếu có
                if (kpiId.HasValue)
                {
                    query = query.Where(m => m.KpiId == kpiId.Value);
                }

                // Lọc theo khoảng thời gian
                if (startDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate <= endDate.Value);
                }

                // Đếm tổng số bản ghi
                var totalItems = await query.CountAsync();

                // Áp dụng phân trang
                var measurements = await query
                    .OrderByDescending(m => m.MeasurementDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Chuyển đổi sang view model
                var viewModels = measurements.Select(m =>
                {
                    var vm = _mapper.Map<KpiValueViewModel>(m);

                    if (m.Kpi != null)
                    {
                        vm.KpiName = m.Kpi.Name;
                        vm.KpiCode = m.Kpi.Code;
                        vm.TargetValue = m.Kpi.TargetValue.HasValue ? m.Kpi.TargetValue.Value : null;
                        vm.KpiType = m.Kpi.GetType().Name;
                        vm.Period = m.MeasurementDate.ToString("MMM yyyy");

                        // Tính phần trăm đạt được
                        if (m.Kpi.TargetValue.HasValue && m.Kpi.TargetValue.Value > 0)
                        {
                            var actualValueDecimal = m.ActualValue;
                            var targetValueDecimal = m.Kpi.TargetValue.Value;
                            vm.AchievementPercentage = Math.Round((actualValueDecimal / targetValueDecimal) * 100, 2);

                            // Thiết lập trạng thái và màu sắc
                            if (vm.AchievementPercentage >= 100)
                            {
                                vm.Status = "Đạt mục tiêu";
                                vm.StatusCssClass = "bg-success";
                            }
                            else if (vm.AchievementPercentage >= 80)
                            {
                                vm.Status = "Có rủi ro";
                                vm.StatusCssClass = "bg-warning text-dark";
                            }
                            else
                            {
                                vm.Status = "Không đạt";
                                vm.StatusCssClass = "bg-danger";
                            }
                        }
                    }

                    return vm;
                }).ToList();

                // Tạo danh sách KPI cho dropdown
                var kpiList = await dbContext.Set<KpiBase>()
                    .AsNoTracking()
                    .OrderBy(k => k.Code)
                    .Select(k => new SelectListItem
                    {
                        Value = k.Id.ToString(),
                        Text = $"{k.Code} - {k.Name}"
                    })
                    .ToListAsync();

                // Tạo view model cho lịch sử đo lường
                var model = new MeasurementHistoryViewModel
                {
                    Measurements = viewModels,
                    KpiList = new SelectList(kpiList, "Value", "Text"),
                    SelectedKpiId = kpiId,
                    StartDate = startDate,
                    EndDate = endDate,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalItems
                };

                // Thiết lập thông tin cho trang
                ViewData["Title"] = "Lịch sử đo lường KPI";
                ViewData["Icon"] = "bi-clock-history";
                ViewData["Subtitle"] = "Xem lịch sử các đo lường KPI theo thời gian";

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị lịch sử đo lường: {Message}", ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Exports measurement data to Excel
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Export(MeasurementFilterViewModel filter)
        {
            try
            {
                var measurements = await GetMeasurementsByFilterAsync(filter);
                // TODO: Implement Excel export logic
                return File(new byte[] { }, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "measurements.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exporting measurements");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Xem KPI theo loại KPI: KRI, PI, RI, KPI
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> SeparatedView()
        {
            try
            {
                var viewModel = new KpiSeparatedViewModel
                {
                    KeyPerformanceIndicators = new List<KpiInfoViewModel>(),
                    PerformanceIndicators = new List<KpiInfoViewModel>(),
                    KeyResultIndicators = new List<KpiInfoViewModel>(),
                    ResultIndicators = new List<KpiInfoViewModel>()
                };

                var dbContext = GetDbContextFromUnitOfWork();

                // Tải tất cả KPI từ cơ sở dữ liệu
                var kpiList = await dbContext.Set<KpiBase>().AsNoTracking().ToListAsync();

                // Lọc từng loại KPI và thêm vào các danh sách tương ứng
                var pis = await dbContext.Set<PI>().AsNoTracking().ToListAsync();
                var kris = await dbContext.Set<KRI>().AsNoTracking().ToListAsync();
                var ris = await dbContext.Set<RI>().AsNoTracking().ToListAsync();
                var kpis = await dbContext.Set<Models.Entities.KPI.KPI>().AsNoTracking().ToListAsync();

                _logger.LogInformation($"Tổng số KPI: {kpiList.Count}");
                _logger.LogInformation($"Số lượng PI: {pis.Count}");
                _logger.LogInformation($"Số lượng KRI: {kris.Count}");
                _logger.LogInformation($"Số lượng RI: {ris.Count}");
                _logger.LogInformation($"Số lượng KPI: {kpis.Count}");

                // Xử lý Performance Indicators (PI)
                foreach (var pi in pis)
                {
                    viewModel.PerformanceIndicators.Add(ConvertToKpiInfoViewModel(pi));

                    // Nếu là Key PI, thêm vào danh sách KPI
                    if (pi.IsKey)
                    {
                        viewModel.KeyPerformanceIndicators.Add(ConvertToKpiInfoViewModel(pi));
                    }
                }

                // Xử lý các Result Indicators (RI)
                foreach (var ri in ris)
                {
                    viewModel.ResultIndicators.Add(ConvertToKpiInfoViewModel(ri));
                }

                // Xử lý các Key Result Indicators (KRI)
                foreach (var kri in kris)
                {
                    viewModel.KeyResultIndicators.Add(ConvertToKpiInfoViewModel(kri));
                }

                // Xử lý các KPI độc lập (không thuộc loại nào ở trên)
                foreach (var kpi in kpis)
                {
                    viewModel.KeyPerformanceIndicators.Add(ConvertToKpiInfoViewModel(kpi));
                }

                // Ghi log số lượng KPI trong từng danh sách
                _logger.LogInformation($"KeyPerformanceIndicators: {viewModel.KeyPerformanceIndicators.Count}");
                _logger.LogInformation($"PerformanceIndicators: {viewModel.PerformanceIndicators.Count}");
                _logger.LogInformation($"KeyResultIndicators: {viewModel.KeyResultIndicators.Count}");
                _logger.LogInformation($"ResultIndicators: {viewModel.ResultIndicators.Count}");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi tải dữ liệu KPI theo loại");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Key Performance Indicators page - redirects to KPI controller with KPI filter
        /// Shows Performance Indicators that have IsKey = true
        /// </summary>
        /// <param name="filter">KPI filter for search and filtering</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>Redirects to KPI controller's Measurement action with KPI filter</returns>
        public IActionResult KeyPerformanceIndicators(KpiFilterViewModel filter, int page = 1)
        {
            _logger.LogInformation("Redirecting to Key Performance Indicators measurement page");

            if (filter == null)
                filter = new KpiFilterViewModel();

            filter.KpiType = KpiType.PerformanceIndicator;
            filter.IsKey = true;

            // Set view data for title
            TempData["MeasurementTitle"] = "Chỉ số hiệu suất chính (KPI)";
            TempData["MeasurementIcon"] = "bi-graph-up-arrow";
            TempData["MeasurementSubtitle"] = "Đo lường và theo dõi các chỉ số hiệu suất chính của tổ chức";

            return RedirectToAction("Measurement", "Kpi", new { filter, page });
        }

        /// <summary>
        /// Performance Indicators page - redirects to KPI controller with PI filter
        /// </summary>
        /// <param name="filter">KPI filter for search and filtering</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>Redirects to KPI controller's Measurement action with PI filter</returns>
        public IActionResult PerformanceIndicators(KpiFilterViewModel filter, int page = 1)
        {
            _logger.LogInformation("Redirecting to Performance Indicators measurement page");

            if (filter == null)
                filter = new KpiFilterViewModel();

            filter.KpiType = KpiType.PerformanceIndicator;
            filter.IsKey = false;

            // Set view data for title
            TempData["MeasurementTitle"] = "Chỉ số hiệu suất (PI)";
            TempData["MeasurementIcon"] = "bi-bar-chart";
            TempData["MeasurementSubtitle"] = "Đo lường và theo dõi các chỉ số hiệu suất của tổ chức";

            return RedirectToAction("Measurement", "Kpi", new { filter, page });
        }

        /// <summary>
        /// Result Indicators page - redirects to KPI controller with RI filter
        /// </summary>
        /// <param name="filter">KPI filter for search and filtering</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>Redirects to KPI controller's Measurement action with RI filter</returns>
        public IActionResult ResultIndicators(KpiFilterViewModel filter, int page = 1)
        {
            _logger.LogInformation("Redirecting to Result Indicators measurement page");

            if (filter == null)
                filter = new KpiFilterViewModel();

            filter.KpiType = KpiType.ResultIndicator;
            filter.IsKey = false;

            // Set view data for title
            TempData["MeasurementTitle"] = "Chỉ số kết quả (RI)";
            TempData["MeasurementIcon"] = "bi-pie-chart";
            TempData["MeasurementSubtitle"] = "Đo lường và theo dõi các chỉ số kết quả của tổ chức";

            return RedirectToAction("Measurement", "Kpi", new { filter, page });
        }

        /// <summary>
        /// Key Result Indicators page - redirects to KPI controller with KRI filter
        /// </summary>
        /// <param name="filter">KPI filter for search and filtering</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>Redirects to KPI controller's Measurement action with KRI filter</returns>
        public IActionResult KeyResultIndicators(KpiFilterViewModel filter, int page = 1)
        {
            _logger.LogInformation("Redirecting to Key Result Indicators measurement page");

            if (filter == null)
                filter = new KpiFilterViewModel();

            filter.KpiType = KpiType.KeyResultIndicator;

            // Set view data for title
            TempData["MeasurementTitle"] = "Chỉ số kết quả then chốt (KRI)";
            TempData["MeasurementIcon"] = "bi-graph-up";
            TempData["MeasurementSubtitle"] = "Đo lường và theo dõi các chỉ số kết quả then chốt của tổ chức";

            return RedirectToAction("Measurement", "Kpi", new { filter, page });
        }

        /// <summary>
        /// Add measurement for a KPI
        /// </summary>
        /// <param name="kpiId">ID of the KPI to add measurement for</param>
        /// <returns>View for adding measurement</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public IActionResult AddMeasurement(Guid kpiId)
        {
            // For now, just redirect to the KPI controller's Add Measurement page
            return RedirectToAction("AddMeasurement", "Kpi", new { kpiId });
        }

        /// <summary>
        /// View measurements for a specific KPI
        /// </summary>
        /// <param name="id">ID of the KPI to view measurements for</param>
        /// <returns>View with KPI measurements</returns>
        public IActionResult KpiMeasurement(Guid id)
        {
            // Redirect to the KPI details page which includes measurements
            return RedirectToAction("Details", "Kpi", new { id });
        }

        /// <summary>
        /// Displays form for creating a new measurement
        /// </summary>
        [HttpGet]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(Guid kpiId)
        {
            try
            {
                var kpi = await FindKpiByIdAsync(kpiId);

                if (kpi == null)
                {
                    return NotFound();
                }

                var viewModel = new KpiValueCreateViewModel
                {
                    KpiId = kpiId,
                    KpiName = kpi.Name,
                    KpiCode = kpi.Code,
                    TargetValue = kpi.TargetValue.HasValue ? (double)kpi.TargetValue.Value : null,
                    Unit = kpi.Unit,
                    MeasurementDate = DateTime.Today,
                    Frequency = kpi.Frequency
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi xảy ra khi chuẩn bị tạo đo lường mới cho KPI {kpiId}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Handles form submission for creating a new measurement
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(KpiValueCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var kpi = await FindKpiByIdAsync(viewModel.KpiId);

                if (kpi != null)
                {
                    viewModel.KpiName = kpi.Name;
                    viewModel.KpiCode = kpi.Code;
                    viewModel.TargetValue = kpi.TargetValue.HasValue ? (double)kpi.TargetValue.Value : null;
                    viewModel.Unit = kpi.Unit;
                }

                return View(viewModel);
            }

            try
            {
                var kpiValue = new KpiValue
                {
                    KpiId = viewModel.KpiId,
                    ActualValue = (decimal)viewModel.ActualValue,
                    MeasurementDate = viewModel.MeasurementDate,
                    Notes = viewModel.Notes
                };

                await _unitOfWork.KpiValues.AddAsync(kpiValue);
                await _unitOfWork.SaveChangesAsync();

                // Update KPI's current value and status
                await UpdateKpiStatusAsync(viewModel.KpiId);

                return RedirectToAction("Details", "Kpi", new { id = viewModel.KpiId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi xảy ra khi tạo đo lường mới cho KPI {viewModel.KpiId}: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu đo lường. Vui lòng thử lại sau.");
                return View(viewModel);
            }
        }

        /// <summary>
        /// Extracts the EntityKpi from the Entity
        /// </summary>
        public async Task<IActionResult> ExtractEntityKpiAsync<TEntity>(Guid id) where TEntity : BaseEntity
        {
            try
            {
                var dbContext = GetDbContextFromUnitOfWork();

                // Sử dụng FindAsync với primary key
                var entity = await dbContext.Set<TEntity>().FindAsync(id);

                if (entity == null)
                {
                    return NotFound();
                }

                // BaseEntity đã có thuộc tính Id, truy cập trực tiếp
                var entityId = entity.Id;

                // Log the entity type and ID
                _logger.LogInformation($"Extracting KPI from entity: {typeof(TEntity).Name} with ID: {entityId}");

                return RedirectToAction("Create", "Kpi", new { entityType = typeof(TEntity).Name, entityId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extracting KPI from entity: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Returns a view with details about a specific measurement
        /// </summary>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var dbContext = GetDbContextFromUnitOfWork();
                // Retrieve the KPI value with its KPI
                var kpiValue = await dbContext.KpiValues
                    .AsNoTracking()
                    .Include(v => v.Kpi)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (kpiValue == null)
                {
                    return NotFound();
                }

                var viewModel = _mapper.Map<KpiValueViewModel>(kpiValue);

                // Add additional information
                if (kpiValue.Kpi != null)
                {
                    viewModel.KpiType = kpiValue.Kpi.GetType().Name;
                    viewModel.Period = kpiValue.MeasurementDate.ToString("MMM yyyy");
                }

                _logger.LogInformation($"Hiển thị thông tin chi tiết cho đo lường ID: {id}");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi xảy ra khi hiển thị chi tiết đo lường {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        #region Helper Methods

        // Bộ nhớ cache cho các KPI đã truy vấn
        private static readonly Dictionary<Guid, KpiBase> _kpiCache = new Dictionary<Guid, KpiBase>();

        /// <summary>
        /// Tìm KPI theo ID từ cơ sở dữ liệu
        /// </summary>
        private async Task<KpiBase?> FindKpiByIdAsync(Guid id)
        {
            var dbContext = GetDbContextFromUnitOfWork();
            return await dbContext.Set<KpiBase>()
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        private async Task<List<KpiValueViewModel>> GetMeasurementsByFilterAsync(MeasurementFilterViewModel? filter)
        {
            var dbContext = GetDbContextFromUnitOfWork();

            // Bắt đầu với một IQueryable đơn giản
            IQueryable<KpiValue> query = dbContext.KpiValues.AsNoTracking();

            // Thêm Include để eager load liên kết KPI
            query = query.Include(m => m.Kpi);

            // Áp dụng các bộ lọc
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(m =>
                        (m.Notes != null && m.Notes.Contains(filter.SearchTerm)) ||
                        (m.Kpi != null && (m.Kpi.Name.Contains(filter.SearchTerm) || m.Kpi.Code.Contains(filter.SearchTerm)))
                    );
                }

                if (filter.StartDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    query = query.Where(m => m.MeasurementDate <= filter.EndDate.Value);
                }

                if (filter.KpiType.HasValue)
                {
                    // Bộ lọc theo loại KPI dựa trên các giá trị discriminator
                    switch (filter.KpiType)
                    {
                        case KpiType.KeyResultIndicator:
                            query = query.Where(m => m.Kpi != null && m.Kpi.GetType().Name == nameof(KRI));
                            break;
                        case KpiType.ResultIndicator:
                            query = query.Where(m => m.Kpi != null && m.Kpi.GetType().Name == nameof(RI));
                            break;
                        case KpiType.PerformanceIndicator:
                            query = query.Where(m => m.Kpi != null && m.Kpi.GetType().Name == nameof(PI));
                            break;
                        case KpiType.StandaloneKPI:
                            query = query.Where(m => m.Kpi != null && m.Kpi.GetType().Name == nameof(Models.Entities.KPI.KPI));
                            break;
                    }
                }

                if (filter.DepartmentId.HasValue)
                {
                    // Lấy thông tin phòng ban theo ID
                    var department = await dbContext.Departments
                        .Where(d => d.Id == filter.DepartmentId.Value)
                        .Select(d => d.Name)
                        .FirstOrDefaultAsync();

                    if (!string.IsNullOrEmpty(department))
                    {
                        query = query.Where(m => m.Kpi != null && m.Kpi.Department == department);
                    }
                }

                if (filter.Frequency.HasValue)
                {
                    query = query.Where(m => m.Kpi != null && m.Kpi.Frequency == filter.Frequency.Value);
                }
            }

            // Sắp xếp kết quả
            query = query.OrderByDescending(m => m.MeasurementDate);

            // Thực thi truy vấn chính chỉ một lần để cải thiện hiệu suất
            var kpiValues = await query.ToListAsync();

            // Ánh xạ sang view models
            var viewModels = new List<KpiValueViewModel>();

            foreach (var value in kpiValues)
            {
                var viewModel = _mapper.Map<KpiValueViewModel>(value);

                // Sử dụng entity KPI đã được load để thiết lập các thuộc tính liên quan
                if (value.Kpi != null)
                {
                    viewModel.TargetValue = value.Kpi.TargetValue;
                    viewModel.KpiType = value.Kpi.GetType().Name;
                    viewModel.KpiCode = value.Kpi.Code;
                    viewModel.KpiName = value.Kpi.Name;

                    // Tính toán phần trăm đạt được
                    if (value.Kpi.TargetValue.HasValue && value.Kpi.TargetValue > 0)
                    {
                        viewModel.AchievementPercentage = Math.Round((value.ActualValue / value.Kpi.TargetValue.Value) * 100, 2);
                        viewModel.Variance = value.ActualValue - value.Kpi.TargetValue.Value;

                        // Thiết lập trạng thái dựa trên phần trăm đạt được
                        if (viewModel.AchievementPercentage >= 100)
                        {
                            viewModel.Status = "Đạt mục tiêu";
                            viewModel.StatusCssClass = "badge bg-success";
                        }
                        else if (viewModel.AchievementPercentage >= 80)
                        {
                            viewModel.Status = "Có rủi ro";
                            viewModel.StatusCssClass = "badge bg-warning text-dark";
                        }
                        else
                        {
                            viewModel.Status = "Không đạt";
                            viewModel.StatusCssClass = "badge bg-danger";
                        }
                    }

                    // Thiết lập kỳ dựa trên ngày đo lường
                    viewModel.Period = value.MeasurementDate.ToString("MMM yyyy");
                }

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        // Dictionary to cache SelectLists
        private static readonly Dictionary<string, SelectList> _selectListCache = new Dictionary<string, SelectList>();

        private async Task<IEnumerable<SelectListItem>> GetDepartmentSelectListCached()
        {
            // Lấy danh sách phòng ban từ cơ sở dữ liệu
            var departments = await _unitOfWork.Departments.GetAllAsync();

            // Chuyển đổi thành SelectListItem cho dropdown
            return departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                });
        }

        /// <summary>
        /// Lấy danh sách enum cho dropdown (sử dụng bộ nhớ đệm)
        /// </summary>
        private IEnumerable<SelectListItem> GetEnumSelectListCached<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)(object)e).ToString(),
                    Text = e.GetDisplayName()
                });
        }

        /// <summary>
        /// Cập nhật trạng thái KPI
        /// </summary>
        public async Task<bool> UpdateKpiStatusAsync(Guid kpiId)
        {
            try
            {
                var dbContext = GetDbContextFromUnitOfWork();

                // Tìm KPI cần cập nhật
                var kpi = await dbContext.Set<KpiBase>().FirstOrDefaultAsync(k => k.Id == kpiId);

                if (kpi == null)
                {
                    _logger.LogWarning($"Không tìm thấy KPI với ID: {kpiId}");
                    return false;
                }

                // Tìm giá trị đo lường mới nhất cho KPI này
                var latestMeasurement = await dbContext.KpiValues
                    .Where(v => v.KpiId == kpiId)
                    .OrderByDescending(v => v.MeasurementDate)
                    .FirstOrDefaultAsync();

                // Cập nhật CurrentValue từ giá trị mới nhất nếu có
                if (latestMeasurement != null)
                {
                    kpi.CurrentValue = latestMeasurement.ActualValue;
                    kpi.UpdatedAt = DateTime.UtcNow;
                    _logger.LogInformation($"Đã cập nhật CurrentValue của KPI {kpi.Code} thành {latestMeasurement.ActualValue}");
                }
                else
                {
                    _logger.LogWarning($"Không tìm thấy giá trị đo lường nào cho KPI: {kpiId}");
                    // Nếu không có giá trị đo lường, đặt CurrentValue là null
                    kpi.CurrentValue = null;
                    return false;
                }

                // Tính toán trạng thái dựa trên giá trị hiện tại và mục tiêu
                if (kpi.TargetValue.HasValue && kpi.CurrentValue.HasValue)
                {
                    double achievementPercent = (double)(kpi.CurrentValue.Value / kpi.TargetValue.Value) * 100;

                    if (kpi.MeasurementDirection == MeasurementDirection.HigherIsBetter)
                    {
                        if (achievementPercent >= 100)
                        {
                            kpi.Status = KpiStatus.OnTarget; // KPI đạt mục tiêu
                        }
                        else if (achievementPercent >= 80)
                        {
                            kpi.Status = KpiStatus.AtRisk;  // KPI có rủi ro
                        }
                        else
                        {
                            kpi.Status = KpiStatus.BelowTarget; // KPI không đạt
                        }
                    }
                    else if (kpi.MeasurementDirection == MeasurementDirection.LowerIsBetter)
                    {
                        if (kpi.CurrentValue <= kpi.TargetValue)
                        {
                            kpi.Status = KpiStatus.OnTarget; // KPI đạt mục tiêu
                        }
                        else if (kpi.CurrentValue <= kpi.TargetValue * 1.2m)
                        {
                            kpi.Status = KpiStatus.AtRisk;  // KPI có rủi ro
                        }
                        else
                        {
                            kpi.Status = KpiStatus.BelowTarget; // KPI không đạt
                        }
                    }
                }

                // Cập nhật KPI và lưu thay đổi
                dbContext.Update(kpi);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Đã cập nhật trạng thái của KPI {kpi.Code} thành {kpi.Status}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật trạng thái KPI {kpiId}: {ex.Message}");
                return false;
            }
        }

        private KpiType GetIndicatorType(KpiBase indicator)
        {
            return indicator switch
            {
                KRI => KpiType.KeyResultIndicator,
                RI => KpiType.ResultIndicator,
                PI => KpiType.PerformanceIndicator,
                _ => KpiType.StandaloneKPI
            };
        }

        /// <summary>
        /// Chuyển đổi từ đối tượng KpiBase thành KpiInfoViewModel
        /// </summary>
        private KpiInfoViewModel ConvertToKpiInfoViewModel(KpiBase kpi)
        {
            return new KpiInfoViewModel
            {
                Id = kpi.Id,
                Name = kpi.Name,
                Code = kpi.Code,
                Department = kpi.Department,
                Frequency = kpi.Frequency,
                Description = kpi.Description,
                ActualValue = kpi.CurrentValue.HasValue ? (double)kpi.CurrentValue.Value : null,
                TargetValue = kpi.TargetValue.HasValue ? (double)kpi.TargetValue.Value : null,
                Unit = kpi.Unit,
                Type = kpi.GetType().Name,
                StatusCssClass = GetStatusCssClass(kpi)
            };
        }

        /// <summary>
        /// Lấy CSS class cho trạng thái KPI
        /// </summary>
        private string GetStatusCssClass(KpiBase kpi)
        {
            if (kpi.Status == KpiStatus.Active)
                return "badge bg-success";
            else if (kpi.Status == KpiStatus.Draft)
                return "badge bg-warning text-dark";
            else
                return "badge bg-danger";
        }

        #endregion
    }

    /// <summary>
    /// View model for displaying separate sections of indicator types
    /// </summary>
    public class SeparatedIndicatorsViewModel
    {
        /// <summary>
        /// List of Key Performance Indicators (KPIs)
        /// </summary>
        public List<KpiListItemViewModel> KeyPerformanceIndicators { get; set; } = new List<KpiListItemViewModel>();

        /// <summary>
        /// List of Performance Indicators (PIs)
        /// </summary>
        public List<KpiListItemViewModel> PerformanceIndicators { get; set; } = new List<KpiListItemViewModel>();

        /// <summary>
        /// List of Key Result Indicators (KRIs)
        /// </summary>
        public List<KpiListItemViewModel> KeyResultIndicators { get; set; } = new List<KpiListItemViewModel>();

        /// <summary>
        /// List of Result Indicators (RIs)
        /// </summary>
        public List<KpiListItemViewModel> ResultIndicators { get; set; } = new List<KpiListItemViewModel>();
    }
}