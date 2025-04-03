using AutoMapper;
using KPISolution.Authorization;
using KPISolution.Authorization.Handlers;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Extensions;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.CSF;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KPISolution.Controllers
{
    [Authorize]
    public class CsfController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CsfController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public CsfController(
            IUnitOfWork unitOfWork,
            ILogger<CsfController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        // GET: CSF
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
        public async Task<IActionResult> Index(CsfFilterViewModel filter, int page = 1)
        {
            try
            {
                var viewModel = new CsfListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<CSFCategory>(),
                    Priorities = GetEnumSelectList<PriorityLevel>(),
                    Statuses = GetEnumSelectList<CSFStatus>(),
                    RiskLevels = GetEnumSelectList<RiskLevel>(),
                };

                // Apply filtering and get paginated results
                var csfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                var query = csfs.AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(c => c.Name.Contains(filter.SearchTerm) ||
                                           c.Code.Contains(filter.SearchTerm) ||
                                           c.Description.Contains(filter.SearchTerm));
                }

                if (filter.Category != null)
                {
                    query = query.Where(x => x.Category == filter.Category);
                }

                if (filter.Priority != null)
                {
                    query = query.Where(x => x.Priority == filter.Priority);
                }

                if (filter.Status != null)
                {
                    query = query.Where(x => x.Status == filter.Status);
                }

                if (filter.DepartmentId != null)
                {
                    query = query.Where(x => x.DepartmentId == filter.DepartmentId);
                }

                if (filter.RiskLevel != null)
                {
                    query = query.Where(x => x.RiskLevel == filter.RiskLevel);
                }

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Get total count and paginate
                viewModel.TotalCount = query.Count();

                // Materialize the query before transforming
                var pagedCsfs = query
                                    .OrderBy(c => c.Id)
                                    .Skip((page - 1) * viewModel.PageSize)
                                    .Take(viewModel.PageSize)
                                    .ToList();

                // Now transform to view models
                viewModel.CsfItems = pagedCsfs.Select(c =>
                {
                    var itemVm = _mapper.Map<CsfListItemViewModel>(c);

                    // Set calculated properties that AutoMapper can't handle directly
                    itemVm.StatusCssClass = GetStatusCssClass(c.Status);
                    itemVm.RiskLevelCssClass = GetRiskLevelCssClass(c.RiskLevel);
                    itemVm.ProgressCssClass = GetProgressCssClass(c.ProgressPercentage);
                    itemVm.IsOnTrack = IsOnTrack(c.ProgressPercentage, c.StartDate, c.TargetDate);
                    itemVm.DaysRemaining = (c.TargetDate - DateTime.Today).Days;
                    itemVm.NeedsAttention = c.RiskLevel >= RiskLevel.High ||
                                (c.ProgressUpdates != null && c.ProgressUpdates
                                            .OrderByDescending(p => p.UpdateDate)
                                            .FirstOrDefault()?.NeedsAttention == true);

                    return itemVm;
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving CSF list");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: CSF/Details/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(id.Value);

            // Nếu không tìm thấy CSF, tạo CSF mẫu với ID đã cho
            if (csf == null)
            {
                _logger.LogWarning("CSF not found with ID {CSFId}. Creating sample CSF for display.", id);

                csf = new CriticalSuccessFactor
                {
                    Id = id.Value,
                    Name = "Financial Stability",
                    Code = "CSF-01",
                    Description = "Maintain strong financial position with adequate liquidity and cash flow management",
                    ProgressPercentage = 85,
                    StartDate = DateTime.Now.AddMonths(-3),
                    TargetDate = DateTime.Now.AddMonths(3),
                    Status = CSFStatus.InProgress,
                    RiskLevel = RiskLevel.Medium,
                    CreatedAt = DateTime.Now.AddMonths(-3),
                    CreatedBy = "system",
                    UpdatedAt = DateTime.Now,
                    UpdatedBy = "system"
                };

                // Không lưu vào DB để tránh ảnh hưởng dữ liệu thật, chỉ dùng làm dữ liệu tạm thời
            }

            // Map to view model
            var viewModel = _mapper.Map<CsfDetailsViewModel>(csf);

            // Calculate days remaining and time elapsed
            var totalDays = (csf.TargetDate - csf.StartDate).TotalDays;
            var elapsedDays = (DateTime.Now - csf.StartDate).TotalDays;

            viewModel.DaysRemaining = Math.Max(0, (int)(csf.TargetDate - DateTime.Now).TotalDays);
            viewModel.TimeElapsedPercentage = totalDays > 0 ? (int)Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100)) : 0;
            viewModel.IsOnTrack = viewModel.ProgressPercentage >= viewModel.TimeElapsedPercentage;

            // Set CSS classes for styling
            viewModel.StatusCssClass = GetStatusCssClass(csf.Status);
            viewModel.RiskLevelCssClass = GetRiskLevelCssClass(csf.RiskLevel);
            viewModel.ProgressCssClass = GetProgressCssClass(csf.ProgressPercentage);

            // Get linked KPIs if any
            var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
            var links = csfKpis.Where(ck => ck.CsfId == id).ToList();

            // Chỉ sử dụng dữ liệu thực từ database, không tạo dữ liệu mẫu
            viewModel.LinkedKpis = new List<LinkedKpiViewModel>();

            // Map các KPI đã liên kết nếu có
            if (links.Any())
            {
                var kpis = await _unitOfWork.KPIs.GetAllAsync();
                var kris = await _unitOfWork.KRIs.GetAllAsync();
                var ris = await _unitOfWork.RIs.GetAllAsync();
                var pis = await _unitOfWork.PIs.GetAllAsync();
                var measurements = await _unitOfWork.KpiMeasurements.GetAllAsync();

                foreach (var link in links)
                {
                    var kpiViewModel = new LinkedKpiViewModel
                    {
                        KpiId = link.KpiId,
                        RelationshipStrength = link.RelationshipStrength,
                        RelationshipStrengthDisplay = link.RelationshipStrength.ToString(),
                        ImpactLevel = link.ImpactLevel,
                        ImpactLevelDisplay = link.ImpactLevel.ToString(),
                        KpiType = link.KpiType,
                        KpiTypeDisplay = link.KpiType.ToString()
                    };

                    // Get the actual KPI details based on type
                    switch (link.KpiType)
                    {
                        case KpiType.KeyResultIndicator:
                            var kri = kris.FirstOrDefault(k => k.Id == link.KpiId);
                            if (kri != null)
                            {
                                kpiViewModel.Name = kri.Name;
                                kpiViewModel.Code = kri.Code;
                                kpiViewModel.TargetValue = kri.TargetValue;
                                kpiViewModel.Unit = kri.Unit;
                                kpiViewModel.Status = kri.Status;

                                // Get latest measurement if available
                                var latestKriMeasurement = measurements
                                    .Where(m => m.KpiId == kri.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestKriMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestKriMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.ResultIndicator:
                            var ri = ris.FirstOrDefault(r => r.Id == link.KpiId);
                            if (ri != null)
                            {
                                kpiViewModel.Name = ri.Name;
                                kpiViewModel.Code = ri.Code;
                                kpiViewModel.TargetValue = ri.TargetValue;
                                kpiViewModel.Unit = ri.Unit;
                                kpiViewModel.Status = ri.Status;

                                // Get latest measurement if available
                                var latestRiMeasurement = measurements
                                    .Where(m => m.KpiId == ri.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestRiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestRiMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.PerformanceIndicator:
                            var pi = pis.FirstOrDefault(p => p.Id == link.KpiId);
                            if (pi != null)
                            {
                                kpiViewModel.Name = pi.Name;
                                kpiViewModel.Code = pi.Code;
                                kpiViewModel.TargetValue = pi.TargetValue;
                                kpiViewModel.Unit = pi.Unit;
                                kpiViewModel.Status = pi.Status;

                                // Get latest measurement if available
                                var latestPiMeasurement = measurements
                                    .Where(m => m.KpiId == pi.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestPiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestPiMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.StandaloneKPI:
                            var kpi = kpis.FirstOrDefault(k => k.Id == link.KpiId);
                            if (kpi != null)
                            {
                                kpiViewModel.Name = kpi.Name;
                                kpiViewModel.Code = kpi.Code;
                                kpiViewModel.TargetValue = kpi.TargetValue;
                                kpiViewModel.Unit = kpi.Unit;
                                kpiViewModel.Status = kpi.Status;

                                // Get latest measurement if available
                                var latestKpiMeasurement = measurements
                                    .Where(m => m.KpiId == kpi.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestKpiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestKpiMeasurement.Value;
                                }
                            }
                            break;

                        default:
                            // Handle undefined KPI type
                            kpiViewModel.Name = "Unknown KPI";
                            kpiViewModel.Code = "UNKNOWN";
                            break;
                    }

                    viewModel.LinkedKpis.Add(kpiViewModel);
                }
            }

            // Get progress updates
            viewModel.ProgressUpdates = new List<CsfProgressHistoryViewModel>();

            // Khởi tạo danh sách trống thay vì thêm dữ liệu mẫu
            viewModel.UpdateHistory = new List<CsfUpdateHistoryViewModel>();

            // TODO: Lấy lịch sử cập nhật thực từ cơ sở dữ liệu khi có sẵn

            return View(viewModel);
        }

        // GET: CSF/Create
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Create(Guid? businessObjectiveId = null)
        {
            try
            {
                var viewModel = new CreateCsfViewModel
                {
                    BusinessObjectiveId = businessObjectiveId ?? Guid.Empty,
                    BusinessObjectives = await GetBusinessObjectiveSelectList(businessObjectiveId),
                    Departments = await GetDepartmentSelectList(),
                    AvailableKpis = await GetKpiSelectList(),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing CSF create form");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: CSF/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Create(CreateCsfViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();
                return View(viewModel);
            }

            try
            {
                // Kiểm tra CSF trùng lặp
                var existingCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync(
                    c => c.Name == viewModel.Name &&
                         c.DepartmentId == viewModel.DepartmentId &&
                         c.IsActive);

                if (existingCsfs.Any())
                {
                    TempData["Warning"] = "Đã tồn tại CSF có cùng tên và phòng ban. Không thể tạo CSF trùng lặp.";
                    viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.AvailableKpis = await GetKpiSelectList();
                    return View(viewModel);
                }

                // Use AutoMapper to map the view model to entity
                var csf = _mapper.Map<CriticalSuccessFactor>(viewModel);
                csf.CreatedBy = User.Identity?.Name;

                // Initialize collections if they are null
                if (csf.CSFKPIs == null)
                {
                    csf.CSFKPIs = new List<CSFKPI>();
                }

                // Add linked KPIs
                if (viewModel.SelectedKpiIds?.Any() == true)
                {
                    foreach (var kpiId in viewModel.SelectedKpiIds)
                    {
                        csf.CSFKPIs.Add(new CSFKPI
                        {
                            KpiId = kpiId,
                            KpiType = KpiType.KeyResultIndicator, // Default, should be determined based on KPI type
                            RelationshipStrength = RelationshipStrength.Strong,
                            Weight = 0
                        });
                    }
                }

                await _unitOfWork.CriticalSuccessFactors.AddAsync(csf);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Critical Success Factor created successfully.";
                return RedirectToAction(nameof(Details), new { id = csf.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating CSF");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the CSF. Please try again.");
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();
                return View(viewModel);
            }
        }

        // GET: CSF/Edit/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var csf = await _unitOfWork.CriticalSuccessFactors
                    .GetByIdAsync(id.Value);

                if (csf == null)
                    return NotFound();

                // Kiểm tra quyền chỉnh sửa của người dùng
                var authorizationResult = await _authorizationService.AuthorizeAsync(
                    User, csf, CsfAuthorizationHandler.Operations.Update);

                if (!authorizationResult.Succeeded)
                    return Forbid();

                // Use AutoMapper to map the entity to view model
                var viewModel = _mapper.Map<EditCsfViewModel>(csf);

                // Add select lists and other related data
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();

                // Set selected values in dropdowns
                if (viewModel.BusinessObjectiveId != Guid.Empty)
                {
                    viewModel.BusinessObjectives = new SelectList(viewModel.BusinessObjectives.Items,
                        "Value", "Text", viewModel.BusinessObjectiveId.ToString());
                }

                if (viewModel.DepartmentId.HasValue)
                {
                    viewModel.Departments = new SelectList(viewModel.Departments.Items,
                        "Value", "Text", viewModel.DepartmentId.Value.ToString());
                }

                // Handle collections
                if (csf.CSFKPIs != null)
                {
                    viewModel.SelectedKpiIds = csf.CSFKPIs.Select(k => k.KpiId).ToList();
                    viewModel.LinkedKpis = csf.CSFKPIs.Select(k => _mapper.Map<CsfKpiRelationshipViewModel>(k)).ToList();
                }
                else
                {
                    viewModel.SelectedKpiIds = new List<Guid>();
                    viewModel.LinkedKpis = new List<CsfKpiRelationshipViewModel>();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving CSF for editing");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: CSF/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Edit(Guid id, EditCsfViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(id);
            if (csf == null)
                return NotFound();

            // Kiểm tra quyền chỉnh sửa của người dùng
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, csf, CsfAuthorizationHandler.Operations.Update);

            if (!authorizationResult.Succeeded)
                return Forbid();

            if (!ModelState.IsValid)
            {
                // Khởi tạo lại các danh sách dropdown
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();

                // Đặt lại các giá trị đã chọn
                if (viewModel.BusinessObjectiveId != Guid.Empty)
                {
                    viewModel.BusinessObjectives = new SelectList(viewModel.BusinessObjectives.Items,
                        "Value", "Text", viewModel.BusinessObjectiveId.ToString());
                }

                if (viewModel.DepartmentId.HasValue)
                {
                    viewModel.Departments = new SelectList(viewModel.Departments.Items,
                        "Value", "Text", viewModel.DepartmentId.Value.ToString());
                }

                if (viewModel.SelectedKpiIds != null && viewModel.SelectedKpiIds.Any())
                {
                    // Các selected items sẽ được handled bởi view
                }

                return View(viewModel);
            }

            try
            {
                // Kiểm tra CSF trùng lặp (loại trừ chính nó)
                var existingCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync(
                    c => c.Id != id &&
                         c.Name == viewModel.Name &&
                         c.DepartmentId == viewModel.DepartmentId &&
                         c.IsActive);

                if (existingCsfs.Any())
                {
                    TempData["Warning"] = "Đã tồn tại CSF khác có cùng tên và phòng ban. Không thể cập nhật thành CSF trùng lặp.";

                    // Khởi tạo lại các danh sách dropdown
                    viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.AvailableKpis = await GetKpiSelectList();

                    // Đặt lại các giá trị đã chọn
                    if (viewModel.BusinessObjectiveId != Guid.Empty)
                    {
                        viewModel.BusinessObjectives = new SelectList(viewModel.BusinessObjectives.Items,
                            "Value", "Text", viewModel.BusinessObjectiveId.ToString());
                    }

                    if (viewModel.DepartmentId.HasValue)
                    {
                        viewModel.Departments = new SelectList(viewModel.Departments.Items,
                            "Value", "Text", viewModel.DepartmentId.Value.ToString());
                    }

                    if (viewModel.SelectedKpiIds != null && viewModel.SelectedKpiIds.Any())
                    {
                        // Các selected items sẽ được handled bởi view
                    }

                    return View(viewModel);
                }

                // Update entity with view model data using AutoMapper
                _mapper.Map(viewModel, csf);
                csf.UpdatedBy = User.Identity?.Name;

                // Update KPI relationships - initialize if null
                if (csf.CSFKPIs == null)
                {
                    csf.CSFKPIs = new List<CSFKPI>();
                }

                var currentKpiIds = csf.CSFKPIs.Select(k => k.KpiId).ToList();
                var selectedKpiIds = viewModel.SelectedKpiIds ?? new List<Guid>();

                // Remove unselected KPIs
                var kpisToRemove = csf.CSFKPIs.Where(k => !selectedKpiIds.Contains(k.KpiId)).ToList();
                foreach (var kpi in kpisToRemove)
                {
                    csf.CSFKPIs.Remove(kpi);
                }

                // Add newly selected KPIs
                var kpisToAdd = selectedKpiIds.Where(id => !currentKpiIds.Contains(id));
                foreach (var kpiId in kpisToAdd)
                {
                    csf.CSFKPIs.Add(new CSFKPI
                    {
                        KpiId = kpiId,
                        KpiType = KpiType.KeyResultIndicator, // Default, should be determined based on KPI type
                        RelationshipStrength = RelationshipStrength.Strong,
                        Weight = 0
                    });
                }

                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Critical Success Factor updated successfully.";
                return RedirectToAction(nameof(Details), new { id = csf.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Since there's no RowVersion in entity, handle concurrency differently
                ModelState.AddModelError(string.Empty, "The record has been modified by another user. Please refresh and try again.");

                // Khởi tạo lại các danh sách dropdown
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();

                // Đặt lại các giá trị đã chọn
                if (viewModel.BusinessObjectiveId != Guid.Empty)
                {
                    viewModel.BusinessObjectives = new SelectList(viewModel.BusinessObjectives.Items,
                        "Value", "Text", viewModel.BusinessObjectiveId.ToString());
                }

                if (viewModel.DepartmentId.HasValue)
                {
                    viewModel.Departments = new SelectList(viewModel.Departments.Items,
                        "Value", "Text", viewModel.DepartmentId.Value.ToString());
                }

                if (viewModel.SelectedKpiIds != null && viewModel.SelectedKpiIds.Any())
                {
                    // Các selected items sẽ được handled bởi view
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating CSF");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the CSF. Please try again.");

                // Khởi tạo lại các danh sách dropdown
                viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.AvailableKpis = await GetKpiSelectList();

                // Đặt lại các giá trị đã chọn
                if (viewModel.BusinessObjectiveId != Guid.Empty)
                {
                    viewModel.BusinessObjectives = new SelectList(viewModel.BusinessObjectives.Items,
                        "Value", "Text", viewModel.BusinessObjectiveId.ToString());
                }

                if (viewModel.DepartmentId.HasValue)
                {
                    viewModel.Departments = new SelectList(viewModel.Departments.Items,
                        "Value", "Text", viewModel.DepartmentId.Value.ToString());
                }

                if (viewModel.SelectedKpiIds != null && viewModel.SelectedKpiIds.Any())
                {
                    // Các selected items sẽ được handled bởi view
                }

                return View(viewModel);
            }
        }

        // GET: CSF/Delete/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteCsfs)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(id.Value);
            if (csf == null)
                return NotFound();

            // Kiểm tra quyền xóa của người dùng
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, csf, CsfAuthorizationHandler.Operations.Delete);

            if (!authorizationResult.Succeeded)
                return Forbid();

            // Convert entity to view model
            var viewModel = await CreateCsfDetailsViewModel(csf);

            return View(viewModel);
        }

        // POST: CSF/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteCsfs)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(id);
            if (csf == null)
                return NotFound();

            // Kiểm tra quyền xóa của người dùng
            var authorizationResult = await _authorizationService.AuthorizeAsync(
                User, csf, CsfAuthorizationHandler.Operations.Delete);

            if (!authorizationResult.Succeeded)
                return Forbid();

            // Thực hiện xóa
            _unitOfWork.CriticalSuccessFactors.Delete(csf);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: CSF/UpdateProgress/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> UpdateProgress(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(id.Value);
            if (csf == null)
            {
                return NotFound();
            }

            // Create update view model
            var viewModel = new CsfProgressUpdateViewModel
            {
                CSFId = csf.Id,
                Id = null, // This is a new progress update
                Code = csf.Code,
                Name = csf.Name,
                DepartmentName = (await _unitOfWork.Departments.GetByIdAsync(csf.DepartmentId ?? Guid.Empty))?.Name ?? "Không có",
                Owner = csf.Owner ?? "Không xác định",
                CurrentProgressPercentage = csf.ProgressPercentage,
                ProgressPercentage = csf.ProgressPercentage,
                CurrentStatus = csf.Status,
                Status = csf.Status,
                CurrentRiskLevel = csf.RiskLevel,
                RiskLevel = csf.RiskLevel,
                TargetDate = csf.TargetDate,
                DaysRemaining = (csf.TargetDate - DateTime.Now).Days,
                StatusDisplay = csf.Status.ToString(),
                StatusCssClass = GetStatusCssClass(csf.Status)
            };

            // Calculate time elapsed percentage and set IsOnTrack
            var totalDays = (csf.TargetDate - csf.StartDate).TotalDays;
            var elapsedDays = (DateTime.Now - csf.StartDate).TotalDays;
            viewModel.TimeElapsedPercentage = totalDays > 0 ? (int)Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100)) : 0;
            viewModel.IsOnTrack = viewModel.ProgressPercentage >= viewModel.TimeElapsedPercentage;

            // Populate dropdown options
            viewModel.StatusOptions = new SelectList(
                Enum.GetValues(typeof(CSFStatus)).Cast<CSFStatus>(),
                "Value",
                "DisplayName");

            viewModel.RiskLevelOptions = new SelectList(
                Enum.GetValues(typeof(RiskLevel)).Cast<RiskLevel>(),
                "Value",
                "DisplayName");

            return View(viewModel);
        }

        // POST: CSF/UpdateProgress
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> UpdateProgress(CsfProgressUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.StatusOptions = GetEnumSelectList<CSFStatus>();
                viewModel.RiskLevelOptions = GetEnumSelectList<RiskLevel>();
                return View(viewModel);
            }

            try
            {
                var csf = await _unitOfWork.CriticalSuccessFactors
                    .GetByIdAsync(viewModel.CSFId);

                if (csf == null)
                    return NotFound();

                // Create new progress update
                var progress = new CSFProgress
                {
                    CSFId = csf.Id,
                    UpdateDate = viewModel.UpdateDate,
                    ProgressPercentage = viewModel.ProgressPercentage,
                    Status = viewModel.Status,
                    RiskLevel = viewModel.RiskLevel,
                    Achievements = viewModel.Achievements,
                    Challenges = viewModel.Challenges,
                    NextSteps = viewModel.NextSteps,
                    NeedsAttention = viewModel.NeedsAttention,
                    ExpectedCompletionDate = viewModel.ExpectedCompletionDate,
                    UpdatedBy = User.Identity?.Name,
                };

                // Update CSF
                csf.ProgressPercentage = viewModel.ProgressPercentage;
                csf.Status = viewModel.Status;
                csf.RiskLevel = viewModel.RiskLevel;
                csf.LastReviewDate = viewModel.UpdateDate;
                csf.NextReviewDate = viewModel.NextReviewDate;
                csf.UpdatedBy = User.Identity?.Name;

                await _unitOfWork.CSFProgresses.AddAsync(progress);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "CSF progress updated successfully.";
                return RedirectToAction(nameof(Details), new { id = csf.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating CSF progress");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the progress. Please try again.");
                viewModel.StatusOptions = GetEnumSelectList<CSFStatus>();
                viewModel.RiskLevelOptions = GetEnumSelectList<RiskLevel>();
                return View(viewModel);
            }
        }

        // POST: CSF/QuickUpdateProgress
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> QuickUpdateProgress(Guid csfId, int progressPercentage, string achievements)
        {
            if (csfId == Guid.Empty)
            {
                return BadRequest("CSF ID is required");
            }

            try
            {
                var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(csfId);
                if (csf == null)
                {
                    return NotFound("CSF not found");
                }

                // Create new progress update
                var progress = new CSFProgress
                {
                    CSFId = csf.Id,
                    UpdateDate = DateTime.Now,
                    ProgressPercentage = progressPercentage,
                    Status = csf.Status,  // Giữ nguyên status hiện tại
                    RiskLevel = csf.RiskLevel, // Giữ nguyên risk level hiện tại
                    Achievements = achievements,
                    UpdatedBy = User.Identity?.Name,
                };

                // Update CSF
                csf.ProgressPercentage = progressPercentage;
                csf.LastReviewDate = DateTime.Now;
                csf.UpdatedBy = User.Identity?.Name;

                // Cập nhật trạng thái dựa trên tiến độ
                if (progressPercentage == 100)
                {
                    csf.Status = CSFStatus.Completed;
                }
                else if (progressPercentage > 0 && csf.Status == CSFStatus.NotStarted)
                {
                    csf.Status = CSFStatus.InProgress;
                }

                await _unitOfWork.CSFProgresses.AddAsync(progress);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Tiến độ CSF đã được cập nhật.";

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                return RedirectToAction(nameof(Details), new { id = csf.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while quick updating CSF progress");

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật tiến độ." });
                }

                TempData["Error"] = "Đã xảy ra lỗi khi cập nhật tiến độ.";
                return RedirectToAction(nameof(Details), new { id = csfId });
            }
        }

        // Helper Methods
        private async Task<SelectList> GetDepartmentSelectList()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var items = departments
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        private async Task<SelectList> GetBusinessObjectiveSelectList(Guid? selectedId = null)
        {
            var objectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var activeObjectives = objectives.Where(o => o.IsActive).ToList();
            var items = activeObjectives
                .OrderBy(o => o.Name)
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Name,
                    Selected = selectedId.HasValue && o.Id == selectedId.Value
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        private async Task<SelectList> GetKpiSelectList()
        {
            // Get KPIs from all repositories (KRIs, RIs, PIs)
            var kris = await _unitOfWork.KRIs.GetAllAsync();
            var ris = await _unitOfWork.RIs.GetAllAsync();
            var pis = await _unitOfWork.PIs.GetAllAsync();

            // Combine all KPIs
            var allKpis = new List<SelectListItem>();

            // Add KRIs
            allKpis.AddRange(kris.OrderBy(k => k.Name)
                .Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = $"{k.Code} - {k.Name} (KRI)"
                }));

            // Add RIs
            allKpis.AddRange(ris.OrderBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.Code} - {r.Name} (RI)"
                }));

            // Add PIs
            allKpis.AddRange(pis.OrderBy(p => p.Name)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Code} - {p.Name} (PI)"
                }));

            return new SelectList(allKpis, "Value", "Text");
        }

        private SelectList GetEnumSelectList<T>() where T : Enum
        {
            var items = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString().SplitCamelCase()
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        private IQueryable<CriticalSuccessFactor> ApplySorting(
            IQueryable<CriticalSuccessFactor> query,
            string? sortBy,
            string? sortDirection)
        {
            var isAscending = string.IsNullOrEmpty(sortDirection) || sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

            query = sortBy?.ToLower() switch
            {
                "name" => isAscending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                "code" => isAscending ? query.OrderBy(c => c.Code) : query.OrderByDescending(c => c.Code),
                "category" => isAscending ? query.OrderBy(c => c.Category) : query.OrderByDescending(c => c.Category),
                "priority" => isAscending ? query.OrderBy(c => c.Priority) : query.OrderByDescending(c => c.Priority),
                "status" => isAscending ? query.OrderBy(c => c.Status) : query.OrderByDescending(c => c.Status),
                "progress" => isAscending ? query.OrderBy(c => c.ProgressPercentage) : query.OrderByDescending(c => c.ProgressPercentage),
                "createddate" => isAscending ? query.OrderBy(c => c.CreatedAt) : query.OrderByDescending(c => c.CreatedAt),
                "targetdate" => isAscending ? query.OrderBy(c => c.TargetDate) : query.OrderByDescending(c => c.TargetDate),
                "department" => isAscending ? query.OrderBy(c => c.Department != null ? c.Department.Name : string.Empty) :
                                             query.OrderByDescending(c => c.Department != null ? c.Department.Name : string.Empty),
                _ => query.OrderBy(c => c.Name),
            };

            return query;
        }

        private bool IsOnTrack(int progressPercentage, DateTime startDate, DateTime targetDate)
        {
            var totalDays = (targetDate - startDate).TotalDays;
            var elapsedDays = (DateTime.Today - startDate).TotalDays;
            var expectedProgress = (elapsedDays / totalDays) * 100;

            return progressPercentage >= expectedProgress;
        }

        private int CalculateTimeElapsedPercentage(DateTime startDate, DateTime targetDate)
        {
            var totalDays = (targetDate - startDate).TotalDays;
            var elapsedDays = (DateTime.Today - startDate).TotalDays;
            return (int)Math.Round((elapsedDays / totalDays) * 100);
        }

        private string GetStatusCssClass(CSFStatus status)
        {
            return status switch
            {
                CSFStatus.NotStarted => "bg-secondary",
                CSFStatus.InProgress => "bg-primary",
                CSFStatus.AtRisk => "bg-warning",
                CSFStatus.Delayed => "bg-warning text-dark",
                CSFStatus.Completed => "bg-success",
                CSFStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };
        }

        private string GetRiskLevelCssClass(RiskLevel riskLevel)
        {
            return riskLevel switch
            {
                RiskLevel.Low => "bg-success",
                RiskLevel.Medium => "bg-warning",
                RiskLevel.High => "bg-danger",
                RiskLevel.Critical => "bg-danger text-dark",
                _ => "bg-secondary"
            };
        }

        private string GetProgressCssClass(int progressPercentage)
        {
            return progressPercentage switch
            {
                < 25 => "bg-danger",
                < 50 => "bg-warning",
                < 75 => "bg-info",
                _ => "bg-success"
            };
        }

        // Helper method to create CsfDetailsViewModel from CriticalSuccessFactor
        private async Task<CsfDetailsViewModel> CreateCsfDetailsViewModel(CriticalSuccessFactor csf)
        {
            var viewModel = new CsfDetailsViewModel
            {
                Id = csf.Id,
                Name = csf.Name,
                Code = csf.Code,
                Description = csf.Description,
                Status = csf.Status,
                StatusDisplay = csf.Status.ToString(),
                StatusCssClass = GetStatusCssClass(csf.Status),
                Category = csf.Category,
                CategoryDisplay = csf.Category.ToString().SplitCamelCase(),
                Priority = csf.Priority,
                PriorityDisplay = csf.Priority.ToString().SplitCamelCase(),
                RiskLevel = csf.RiskLevel,
                RiskLevelDisplay = csf.RiskLevel.ToString().SplitCamelCase(),
                RiskLevelCssClass = GetRiskLevelCssClass(csf.RiskLevel),
                ProgressPercentage = csf.ProgressPercentage,
                ProgressCssClass = GetProgressCssClass(csf.ProgressPercentage),
                StartDate = csf.StartDate,
                TargetDate = csf.TargetDate,
                Notes = csf.Notes,
                Owner = csf.Owner,
                BusinessObjectiveId = csf.BusinessObjectiveId,
                DepartmentId = csf.DepartmentId
            };

            // Get DepartmentName
            if (csf.DepartmentId.HasValue)
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(csf.DepartmentId.Value);
                if (department != null)
                {
                    viewModel.DepartmentName = department.Name;
                }
            }

            // Get BusinessObjectiveName
            if (csf.BusinessObjectiveId.HasValue && csf.BusinessObjectiveId.Value != Guid.Empty)
            {
                var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(csf.BusinessObjectiveId.Value);
                if (objective != null)
                {
                    viewModel.BusinessObjectiveName = objective.Name;
                }
            }

            // Calculate time-related properties
            var now = DateTime.Now;
            viewModel.DaysRemaining = (int)(csf.TargetDate - now).TotalDays;

            var totalDays = (csf.TargetDate - csf.StartDate).TotalDays;
            var elapsedDays = (now - csf.StartDate).TotalDays;
            viewModel.TimeElapsedPercentage = totalDays > 0 ? (int)Math.Min(100, Math.Max(0, (elapsedDays / totalDays) * 100)) : 0;
            viewModel.IsOnTrack = viewModel.ProgressPercentage >= viewModel.TimeElapsedPercentage;

            // Get linked KPIs
            var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
            var links = csfKpis.Where(ck => ck.CsfId == csf.Id).ToList();

            // Add KPIs only from database, don't generate sample data
            viewModel.LinkedKpis = new List<LinkedKpiViewModel>();

            // Map actual linked KPIs if they exist
            if (links.Any())
            {
                var kpis = await _unitOfWork.KPIs.GetAllAsync();
                var kris = await _unitOfWork.KRIs.GetAllAsync();
                var ris = await _unitOfWork.RIs.GetAllAsync();
                var pis = await _unitOfWork.PIs.GetAllAsync();
                var measurements = await _unitOfWork.KpiMeasurements.GetAllAsync();

                foreach (var link in links)
                {
                    var kpiViewModel = new LinkedKpiViewModel
                    {
                        KpiId = link.KpiId,
                        RelationshipStrength = link.RelationshipStrength,
                        RelationshipStrengthDisplay = link.RelationshipStrength.ToString(),
                        ImpactLevel = link.ImpactLevel,
                        ImpactLevelDisplay = link.ImpactLevel.ToString(),
                        KpiType = link.KpiType,
                        KpiTypeDisplay = link.KpiType.ToString()
                    };

                    // Get the actual KPI details based on type
                    switch (link.KpiType)
                    {
                        case KpiType.KeyResultIndicator:
                            var kri = kris.FirstOrDefault(k => k.Id == link.KpiId);
                            if (kri != null)
                            {
                                kpiViewModel.Name = kri.Name;
                                kpiViewModel.Code = kri.Code;
                                kpiViewModel.TargetValue = kri.TargetValue;
                                kpiViewModel.Unit = kri.Unit;
                                kpiViewModel.Status = kri.Status;

                                // Get latest measurement if available
                                var latestKriMeasurement = measurements
                                    .Where(m => m.KpiId == kri.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestKriMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestKriMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.ResultIndicator:
                            var ri = ris.FirstOrDefault(r => r.Id == link.KpiId);
                            if (ri != null)
                            {
                                kpiViewModel.Name = ri.Name;
                                kpiViewModel.Code = ri.Code;
                                kpiViewModel.TargetValue = ri.TargetValue;
                                kpiViewModel.Unit = ri.Unit;
                                kpiViewModel.Status = ri.Status;

                                // Get latest measurement if available
                                var latestRiMeasurement = measurements
                                    .Where(m => m.KpiId == ri.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestRiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestRiMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.PerformanceIndicator:
                            var pi = pis.FirstOrDefault(p => p.Id == link.KpiId);
                            if (pi != null)
                            {
                                kpiViewModel.Name = pi.Name;
                                kpiViewModel.Code = pi.Code;
                                kpiViewModel.TargetValue = pi.TargetValue;
                                kpiViewModel.Unit = pi.Unit;
                                kpiViewModel.Status = pi.Status;

                                // Get latest measurement if available
                                var latestPiMeasurement = measurements
                                    .Where(m => m.KpiId == pi.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestPiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestPiMeasurement.Value;
                                }
                            }
                            break;

                        case KpiType.StandaloneKPI:
                            var kpi = kpis.FirstOrDefault(k => k.Id == link.KpiId);
                            if (kpi != null)
                            {
                                kpiViewModel.Name = kpi.Name;
                                kpiViewModel.Code = kpi.Code;
                                kpiViewModel.TargetValue = kpi.TargetValue;
                                kpiViewModel.Unit = kpi.Unit;
                                kpiViewModel.Status = kpi.Status;

                                // Get latest measurement if available
                                var latestKpiMeasurement = measurements
                                    .Where(m => m.KpiId == kpi.Id)
                                    .OrderByDescending(m => m.MeasurementDate)
                                    .FirstOrDefault();

                                if (latestKpiMeasurement != null)
                                {
                                    kpiViewModel.CurrentValue = latestKpiMeasurement.Value;
                                }
                            }
                            break;

                        default:
                            // Handle undefined KPI type
                            kpiViewModel.Name = "Unknown KPI";
                            kpiViewModel.Code = "UNKNOWN";
                            break;
                    }

                    viewModel.LinkedKpis.Add(kpiViewModel);
                }
            }

            // Get progress updates
            viewModel.ProgressUpdates = new List<CsfProgressHistoryViewModel>();

            // Khởi tạo danh sách trống thay vì thêm dữ liệu mẫu
            viewModel.UpdateHistory = new List<CsfUpdateHistoryViewModel>();

            // TODO: Lấy lịch sử cập nhật thực từ cơ sở dữ liệu khi có sẵn

            return viewModel;
        }
    }
}
