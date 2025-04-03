using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KPISolution.Authorization;
using KPISolution.Authorization.Handlers;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Extensions;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.KPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing KPIs
    /// </summary>
    [Authorize]
    public class KpiController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<KpiController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="KpiController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations</param>
        /// <param name="logger">The logger for logging information and errors</param>
        /// <param name="mapper">The mapper for mapping between entities and view models</param>
        /// <param name="authorizationService">The authorization service for checking permissions</param>
        public KpiController(
            IUnitOfWork unitOfWork,
            ILogger<KpiController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Displays the list of KPIs with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria for KPIs</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>View with list of KPIs</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Index(KpiFilterViewModel filter, int page = 1)
        {
            try
            {
                var viewModel = new KpiListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<KpiCategory>(),
                    Frequencies = GetEnumSelectList<MeasurementFrequency>(),
                    Directions = GetEnumSelectList<MeasurementDirection>(),
                    PageSize = 10
                };

                // Get KPIs by filter
                var kpis = await GetKpisByFilterAsync(filter);

                // Get the total count before pagination
                viewModel.TotalCount = kpis.Count();

                // Apply pagination
                kpis = kpis
                    .OrderBy(k => k.Id)
                    .Skip((page - 1) * viewModel.PageSize)
                    .Take(viewModel.PageSize)
                    .ToList();

                // Map to view models
                viewModel.KpiItems = kpis.Select(k => _mapper.Map<KpiListItemViewModel>(k)).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving KPI list");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the details of a specific KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to display</param>
        /// <returns>View with KPI details</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Find the KPI in any of the repositories
                KpiBase? kpi = await FindKpiByIdAsync(id.Value);

                // Nếu không tìm thấy KPI, trả về NotFound
                if (kpi == null)
                {
                    _logger.LogWarning("KPI not found with ID {KpiId}", id);
                    return NotFound();
                }

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kpi, KpiAuthorizationHandler.Operations.Read);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model
                var viewModel = _mapper.Map<KpiDetailsViewModel>(kpi);

                // Get historical values if available
                var kpiValues = await _unitOfWork.KpiValues.GetAllAsync();
                viewModel.HistoricalValues = kpiValues
                    .Where(v => v.KpiId == id)
                    .OrderByDescending(v => v.MeasurementDate)
                    .Select(_mapper.Map<KpiValueViewModel>)
                    .ToList();

                // Get linked CSFs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                var linkedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                if (linkedCsfIds.Any())
                {
                    var csfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                    viewModel.LinkedCsfs = csfs
                        .Where(c => linkedCsfIds.Contains(c.Id))
                        .Select(_mapper.Map<LinkedCsfViewModel>)
                        .ToList();
                }
                else
                {
                    // Nếu không có CSF liên kết, tạo danh sách rỗng
                    viewModel.LinkedCsfs = new List<LinkedCsfViewModel>();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving KPI details for ID: {KpiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form for creating a new KPI
        /// </summary>
        /// <returns>Create KPI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(Guid? csfId = null)
        {
            try
            {
                // Get departments and CSFs for dropdowns and pass them in ViewBag
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CriticalSuccessFactors = await GetCsfSelectList();

                // Create a new view model
                var viewModel = new KpiViewModel
                {
                    // Basic properties - Luôn sử dụng KPI độc lập
                    Type = KpiType.StandaloneKPI,
                    KpiType = KpiType.StandaloneKPI,
                    Name = string.Empty,
                    Code = string.Empty,
                    Description = string.Empty,
                    DepartmentId = Guid.Empty,
                    DepartmentName = string.Empty,
                    Owner = string.Empty,
                    Status = "Draft",
                    MeasurementDirection = MeasurementDirection.HigherIsBetter,
                    Unit = string.Empty,
                    Frequency = MeasurementFrequency.Monthly,
                    EffectiveDate = DateTime.Now,
                };

                // Nếu có csfId, thêm vào danh sách SelectedCsfIds
                if (csfId.HasValue && csfId != Guid.Empty)
                {
                    // Kiểm tra sự tồn tại của CSF
                    var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(csfId.Value);
                    if (csf != null)
                    {
                        viewModel.SelectedCsfIds = new List<Guid> { csfId.Value };

                        // Có thể lấy thêm thông tin từ CSF để điền trước cho KPI
                        if (!string.IsNullOrEmpty(csf.Department?.Name))
                        {
                            viewModel.DepartmentName = csf.Department.Name;
                            viewModel.DepartmentId = csf.DepartmentId ?? Guid.Empty;
                        }

                        // Sinh mã tự động dựa trên mã CSF cho KPI độc lập
                        viewModel.Code = $"KPI-{csf.Code}-{DateTime.Now.ToString("yyyyMMdd")}";
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KPI creation form");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for creating a new KPI
        /// </summary>
        /// <param name="viewModel">The view model containing KPI data</param>
        /// <param name="SelectedCsfIds">The selected CSF IDs</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(KpiViewModel viewModel, [FromForm] List<Guid> SelectedCsfIds)
        {
            // Convert KpiViewModel to CreateKpiViewModel for processing
            var createViewModel = new CreateKpiViewModel
            {
                KpiType = KpiType.StandaloneKPI, // Luôn sử dụng KPI độc lập
                Name = viewModel.Name,
                Description = viewModel.Description,
                Code = viewModel.Code,
                Unit = viewModel.Unit,
                TargetValue = viewModel.TargetValue ?? 0,
                DepartmentId = viewModel.DepartmentId,
                Owner = viewModel.Owner,
                MeasurementDirection = viewModel.MeasurementDirection,
                EffectiveDate = viewModel.EffectiveDate ?? DateTime.Now,
                SelectedCsfIds = SelectedCsfIds
                // Other properties can be mapped as needed
            };

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CriticalSuccessFactors = await GetCsfSelectList();
                return View(viewModel);
            }

            try
            {
                // Chỉ tạo KPI độc lập
                var kpi = _mapper.Map<Models.Entities.KPI.KPI>(createViewModel);
                await _unitOfWork.KPIs.AddAsync(kpi);

                // Set audit fields
                kpi.CreatedAt = DateTime.UtcNow;
                kpi.CreatedBy = User.GetUserId();

                // Link to CSFs if any are selected
                if (createViewModel.SelectedCsfIds != null && createViewModel.SelectedCsfIds.Any())
                {
                    foreach (var csfId in createViewModel.SelectedCsfIds)
                    {
                        await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                        {
                            CsfId = csfId,
                            KpiId = kpi.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.GetUserId()
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating KPI");
                ModelState.AddModelError("", "An error occurred while saving the KPI. Please try again.");
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CriticalSuccessFactors = await GetCsfSelectList();
                return View(viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to edit</param>
        /// <returns>Edit KPI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Find the KPI in any of the repositories
                KpiBase? kpi = await FindKpiByIdAsync(id.Value);

                if (kpi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kpi, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to edit view model
                var viewModel = _mapper.Map<EditKpiViewModel>(kpi);

                // Get selected CSF IDs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                viewModel.SelectedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                // Populate dropdown items
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KPI edit form for ID: {KpiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for editing a KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to edit</param>
        /// <param name="viewModel">The view model containing updated KPI data</param>
        /// <param name="SelectedCsfIds">The selected CSF IDs</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid id, KpiViewModel viewModel, [FromForm] List<Guid> SelectedCsfIds)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            // Ensure KPI exists and user is authorized
            var kpi = await FindKpiByIdAsync(id) as Models.Entities.KPI.KPI;
            if (kpi == null)
            {
                _logger.LogWarning("KPI not found: {KpiId}", id);
                return NotFound();
            }

            // Check authorization
            var authResult = await _authorizationService.AuthorizeAsync(
                User, kpi, KpiAuthorizationHandler.Operations.Update);
            if (!authResult.Succeeded)
            {
                _logger.LogWarning("User {UserId} not authorized to edit KPI {KpiId}", User.GetUserId(), id);
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CriticalSuccessFactors = await GetCsfSelectList();
                await LoadLinkedCsfsForEditAsync(viewModel);
                return View(viewModel);
            }

            try
            {
                // Update the KPI (only StandaloneKPI)
                _mapper.Map(viewModel, kpi);
                kpi.UpdatedAt = DateTime.UtcNow;
                kpi.UpdatedBy = User.GetUserId();
                _unitOfWork.KPIs.Update(kpi);

                // Update CSF links
                // First, remove all existing links
                var existingLinks = await _unitOfWork.CSFKPIs.GetAllAsync();
                var linksToRemove = existingLinks.Where(l => l.KpiId == id).ToList();
                foreach (var link in linksToRemove)
                {
                    _unitOfWork.CSFKPIs.Delete(link);
                }

                // Then add new links based on selected CSF IDs
                if (SelectedCsfIds != null && SelectedCsfIds.Any())
                {
                    foreach (var csfId in SelectedCsfIds)
                    {
                        await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                        {
                            CsfId = csfId,
                            KpiId = kpi.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.GetUserId()
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await KpiExistsAsync(id))
                {
                    _logger.LogWarning(ex, "KPI not found during edit: {KpiId}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error while editing KPI: {KpiId}", id);
                    ModelState.AddModelError("", "Someone else may have modified this KPI while you were editing it. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing KPI: {KpiId}", id);
                ModelState.AddModelError("", "An error occurred while saving the KPI. Please try again.");
            }

            ViewBag.Departments = await GetDepartmentSelectList();
            ViewBag.CriticalSuccessFactors = await GetCsfSelectList();
            await LoadLinkedCsfsForEditAsync(viewModel);
            return View(viewModel);
        }

        /// <summary>
        /// Displays the confirmation form for deleting a KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to delete</param>
        /// <returns>Delete confirmation view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Find the KPI in any of the repositories
                KpiBase? kpi = await FindKpiByIdAsync(id.Value);

                if (kpi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kpi, KpiAuthorizationHandler.Operations.Delete);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model for display
                var viewModel = _mapper.Map<KpiDetailsViewModel>(kpi);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KPI deletion for ID: {KpiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for deleting a KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to delete</param>
        /// <returns>Redirect to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Find the KPI in any of the repositories
                KpiBase? kpi = await FindKpiByIdAsync(id);

                if (kpi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kpi, KpiAuthorizationHandler.Operations.Delete);

                if (!authResult.Succeeded)
                    return Forbid();

                // Begin transaction for safe deletion
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // First delete any CSF-KPI links
                    var csfKpis = (await _unitOfWork.CSFKPIs.GetAllAsync())
                        .Where(ck => ck.KpiId == id)
                        .ToList();

                    foreach (var link in csfKpis)
                    {
                        await _unitOfWork.CSFKPIs.DeleteAsync(link);
                    }

                    // Delete any KPI values
                    var kpiValues = (await _unitOfWork.KpiValues.GetAllAsync())
                        .Where(v => v.KpiId == id)
                        .ToList();

                    foreach (var value in kpiValues)
                    {
                        await _unitOfWork.KpiValues.DeleteAsync(value);
                    }

                    // Delete the KPI based on its type
                    var kpiType = kpi.GetType();
                    if (kpiType == typeof(KRI))
                    {
                        var kri = (KRI)kpi;
                        await _unitOfWork.KRIs.DeleteAsync(kri);
                    }
                    else if (kpiType == typeof(RI))
                    {
                        var ri = (RI)kpi;
                        await _unitOfWork.RIs.DeleteAsync(ri);
                    }
                    else if (kpiType == typeof(PI))
                    {
                        var pi = (PI)kpi;
                        await _unitOfWork.PIs.DeleteAsync(pi);
                    }
                    else if (kpiType == typeof(Models.Entities.KPI.KPI))
                    {
                        var standaloneKpi = (Models.Entities.KPI.KPI)kpi;
                        await _unitOfWork.KPIs.DeleteAsync(standaloneKpi);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "Error occurred during transaction while deleting KPI with ID: {KpiId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting KPI with ID: {KpiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays KPIs that require attention (At Risk or Below Target)
        /// </summary>
        /// <returns>View with list of KPIs requiring attention</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Alerts()
        {
            try
            {
                // Get all active KPIs
                var allKpis = await GetKpisByFilterAsync(new KpiFilterViewModel
                {
                    Status = KpiStatus.Active
                });

                // Filter to only include KPIs that are At Risk or Below Target
                var alertKpis = allKpis.Where(k =>
                    k.Status == KpiStatus.AtRisk ||
                    k.Status == KpiStatus.BelowTarget).ToList();

                // Create view model with the filtered KPIs
                var viewModel = new KpiListViewModel
                {
                    KpiItems = alertKpis.Select(k => _mapper.Map<KpiListItemViewModel>(k)).ToList(),
                    TotalCount = alertKpis.Count,
                    CurrentPage = 1,
                    PageSize = alertKpis.Count // Show all alerts on one page
                };

                // Set flag to show only at-risk KPIs in the view
                viewModel.ShowAtRiskOnly = true;

                // Set view data title
                ViewData["Title"] = "KPIs cần chú ý";

                // Return the Index view with the filtered list
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving KPI alerts");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays a list of KPI measurements across the system
        /// </summary>
        /// <param name="filter">Filter criteria for measurements</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>View with list of measurements</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Measurement(KpiFilterViewModel filter, int page = 1)
        {
            try
            {
                var viewModel = new KpiListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<KpiCategory>(),
                    Frequencies = GetEnumSelectList<MeasurementFrequency>(),
                    Directions = GetEnumSelectList<MeasurementDirection>(),
                    PageSize = 10
                };

                // Get measurements for KPIs
                var kpis = await GetKpisByFilterAsync(filter);

                // Get the total count before pagination
                viewModel.TotalCount = kpis.Count();

                // Apply pagination
                kpis = kpis
                    .OrderBy(k => k.Id)
                    .Skip((page - 1) * viewModel.PageSize)
                    .Take(viewModel.PageSize)
                    .ToList();

                // Map to view models
                viewModel.KpiItems = kpis.Select(k => _mapper.Map<KpiListItemViewModel>(k)).ToList();

                // Set view title based on TempData if available
                ViewData["Title"] = TempData["MeasurementTitle"] ?? "Đo lường";

                if (TempData["MeasurementIcon"] != null)
                    ViewData["Icon"] = TempData["MeasurementIcon"];

                if (TempData["MeasurementSubtitle"] != null)
                    ViewData["Subtitle"] = TempData["MeasurementSubtitle"];

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving measurements");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays form for adding a new measurement for a KPI
        /// </summary>
        /// <param name="kpiId">The ID of the KPI to add measurement for</param>
        /// <returns>View for adding measurement</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> AddMeasurement(Guid kpiId)
        {
            try
            {
                var kpi = await FindKpiByIdAsync(kpiId);
                if (kpi == null)
                {
                    return NotFound();
                }

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kpi, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Redirect to Measurement controller's Create action
                return RedirectToAction("Create", "Measurement", new { kpiId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing to add measurement for KPI {KpiId}", kpiId);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays a hierarchical tree view of KPIs, RIs, PIs, and KRIs
        /// </summary>
        /// <returns>TreeView with hierarchical representation of indicators</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> TreeView()
        {
            try
            {
                _logger.LogInformation("Generating KPI Tree View");

                // Create the view model to hold all indicator data
                var viewModel = new KpiTreeViewModel
                {
                    KeyResultIndicators = new List<KpiTreeNodeViewModel>(),
                    ResultIndicators = new List<KpiTreeNodeViewModel>(),
                    PerformanceIndicators = new List<KpiTreeNodeViewModel>()
                };

                // Get all KPI entities
                var kris = await _unitOfWork.KRIs.GetAllAsync();
                var ris = await _unitOfWork.RIs.GetAllAsync();
                var pis = await _unitOfWork.PIs.GetAllAsync();

                // Get all departments for display
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var departmentLookup = departments.ToDictionary(d => d.Name, d => d);

                // Process KRIs (top level)
                foreach (var kri in kris)
                {
                    var kriNode = new KpiTreeNodeViewModel
                    {
                        Id = kri.Id,
                        Name = kri.Name,
                        Code = kri.Code,
                        Type = KpiType.KeyResultIndicator,
                        Department = kri.Department,
                        Description = kri.Description,
                        Status = kri.Status.ToString(),
                        Children = new List<KpiTreeNodeViewModel>()
                    };

                    // Find RIs that have this KRI as parent
                    var childRIs = ris.Where(r => r.ParentKriId == kri.Id).ToList();
                    foreach (var ri in childRIs)
                    {
                        var riNode = new KpiTreeNodeViewModel
                        {
                            Id = ri.Id,
                            Name = ri.Name,
                            Code = ri.Code,
                            Type = KpiType.ResultIndicator,
                            Department = ri.Department,
                            Description = ri.Description,
                            Status = ri.Status.ToString(),
                            ParentId = kri.Id,
                            Children = new List<KpiTreeNodeViewModel>()
                        };

                        // Find PIs that have this RI as parent
                        var childPIs = pis.Where(p => p.RIId == ri.Id).ToList();
                        foreach (var pi in childPIs)
                        {
                            var piNode = new KpiTreeNodeViewModel
                            {
                                Id = pi.Id,
                                Name = pi.Name,
                                Code = pi.Code,
                                Type = KpiType.PerformanceIndicator,
                                Department = pi.Department,
                                Description = pi.Description,
                                Status = pi.Status.ToString(),
                                ParentId = ri.Id,
                                Children = null // PIs are leaf nodes
                            };

                            riNode.Children.Add(piNode);
                        }

                        kriNode.Children.Add(riNode);
                    }

                    viewModel.KeyResultIndicators.Add(kriNode);
                }

                // Process standalone RIs (those without a parent KRI)
                var standaloneRIs = ris.Where(r => r.ParentKriId == null || !kris.Any(k => k.Id == r.ParentKriId)).ToList();
                foreach (var ri in standaloneRIs)
                {
                    var riNode = new KpiTreeNodeViewModel
                    {
                        Id = ri.Id,
                        Name = ri.Name,
                        Code = ri.Code,
                        Type = KpiType.ResultIndicator,
                        Department = ri.Department,
                        Description = ri.Description,
                        Status = ri.Status.ToString(),
                        Children = new List<KpiTreeNodeViewModel>()
                    };

                    // Find PIs that have this RI as parent
                    var childPIs = pis.Where(p => p.RIId == ri.Id).ToList();
                    foreach (var pi in childPIs)
                    {
                        var piNode = new KpiTreeNodeViewModel
                        {
                            Id = pi.Id,
                            Name = pi.Name,
                            Code = pi.Code,
                            Type = KpiType.PerformanceIndicator,
                            Department = pi.Department,
                            Description = pi.Description,
                            Status = pi.Status.ToString(),
                            ParentId = ri.Id,
                            Children = null // PIs are leaf nodes
                        };

                        riNode.Children.Add(piNode);
                    }

                    viewModel.ResultIndicators.Add(riNode);
                }

                // Process standalone PIs (those without a parent RI)
                var standalonePIs = pis.Where(p => p.RIId == null || !ris.Any(r => r.Id == p.RIId)).ToList();
                foreach (var pi in standalonePIs)
                {
                    var piNode = new KpiTreeNodeViewModel
                    {
                        Id = pi.Id,
                        Name = pi.Name,
                        Code = pi.Code,
                        Type = KpiType.PerformanceIndicator,
                        Department = pi.Department,
                        Description = pi.Description,
                        Status = pi.Status.ToString(),
                        Children = null // PIs are leaf nodes
                    };

                    viewModel.PerformanceIndicators.Add(piNode);
                }

                this.WithPageTemplate("Cây chỉ số", "Hiển thị cây chỉ số KPI", "bi-diagram-3");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating KPI Tree View");
                return RedirectToAction("Error", "Home");
            }
        }

        // Helper methods

        /// <summary>
        /// Finds a KPI by its ID across all KPI repositories
        /// </summary>
        /// <param name="id">The ID of the KPI to find</param>
        /// <returns>The KPI if found, null otherwise</returns>
        private async Task<KpiBase?> FindKpiByIdAsync(Guid id)
        {
            // Đầu tiên, tìm kiếm KPI độc lập
            var kpi = await _unitOfWork.KPIs.GetByIdAsync(id);

            // Nếu không tìm thấy, kiểm tra trong PI có IsKey = true
            if (kpi == null)
            {
                var pi = await _unitOfWork.PIs.FirstOrDefaultAsync(p => p.Id == id && p.IsKey);
                if (pi != null)
                {
                    return pi;
                }
            }

            return kpi;
        }

        /// <summary>
        /// Checks if a KPI exists by its ID
        /// </summary>
        /// <param name="id">The ID of the KPI to check</param>
        /// <returns>True if the KPI exists, false otherwise</returns>
        private async Task<bool> KpiExistsAsync(Guid id)
        {
            // Chỉ kiểm tra sự tồn tại của KPI độc lập
            return await _unitOfWork.KPIs.ExistsAsync(e => e.Id == id);
        }

        /// <summary>
        /// Gets a select list of departments for dropdowns
        /// </summary>
        /// <returns>SelectList for departments</returns>
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

        /// <summary>
        /// Gets a select list of CSFs for dropdowns
        /// </summary>
        /// <returns>SelectList for CSFs</returns>
        private async Task<SelectList> GetCsfSelectList()
        {
            var csfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
            var items = csfs
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Code} - {c.Name}"
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// Creates a select list from an enum type
        /// </summary>
        /// <typeparam name="TEnum">The enum type</typeparam>
        /// <returns>SelectList for the enum values</returns>
        private SelectList GetEnumSelectList<TEnum>() where TEnum : struct, Enum
        {
            var items = Enum.GetValues<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString().SplitCamelCase()
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// Gets KPIs from the repositories based on the filter
        /// </summary>
        /// <param name="filter">The filter criteria</param>
        /// <returns>List of KPIs matching the filter</returns>
        private async Task<List<KpiBase>> GetKpisByFilterAsync(KpiFilterViewModel filter)
        {
            List<KpiBase> kpis = new List<KpiBase>();

            // Lấy KPI độc lập 
            var standalone_kpis = await _unitOfWork.KPIs.GetAllAsync();
            kpis.AddRange(standalone_kpis.Cast<KpiBase>());

            // Lấy thêm PI đã được nâng cấp thành KPI (IsKey = true)
            var upgradedPIs = await _unitOfWork.PIs.GetAllAsync(p => p.IsKey);
            kpis.AddRange(upgradedPIs.Cast<KpiBase>());

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                kpis = kpis.Where(k =>
                    k.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    k.Code.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (k.Description != null && k.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Department))
            {
                kpis = kpis.Where(k => k.Department == filter.Department).ToList();
            }

            if (filter.Status.HasValue)
            {
                kpis = kpis.Where(k => k.Status == filter.Status.Value).ToList();
            }

            // Apply sorting
            kpis = ApplySorting(kpis, filter.SortBy, filter.SortDirection).ToList();

            return kpis;
        }

        /// <summary>
        /// Applies sorting to a list of KPIs
        /// </summary>
        /// <param name="kpis">List of KPIs to sort</param>
        /// <param name="sortBy">Property to sort by</param>
        /// <param name="sortDirection">Sort direction (asc/desc)</param>
        /// <returns>Sorted list of KPIs</returns>
        private IEnumerable<KpiBase> ApplySorting(IEnumerable<KpiBase> kpis, string? sortBy, string? sortDirection)
        {
            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? kpis.OrderByDescending(k => k.Name) : kpis.OrderBy(k => k.Name),
                "code" => isDescending ? kpis.OrderByDescending(k => k.Code) : kpis.OrderBy(k => k.Code),
                "department" => isDescending ? kpis.OrderByDescending(k => k.Department) : kpis.OrderBy(k => k.Department),
                "effectivedate" => isDescending ? kpis.OrderByDescending(k => k.EffectiveDate) : kpis.OrderBy(k => k.EffectiveDate),
                "status" => isDescending ? kpis.OrderByDescending(k => k.Status) : kpis.OrderBy(k => k.Status),
                _ => kpis.OrderBy(k => k.Name) // Default sort
            };
        }

        private async Task LoadLinkedCsfsForEditAsync(KpiViewModel viewModel)
        {
            try
            {
                // Get linked CSFs for this KPI
                var allLinks = await _unitOfWork.CSFKPIs.GetAllAsync();
                var linkedCsfs = allLinks.Where(l => l.KpiId == viewModel.Id).ToList();
                viewModel.SelectedCsfIds = linkedCsfs.Select(l => l.CsfId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading linked CSFs for KPI {KpiId}", viewModel.Id);
                // Don't throw - let the edit form load anyway
            }
        }
    }
}