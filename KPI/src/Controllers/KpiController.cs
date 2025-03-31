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

                // Get total count before pagination
                viewModel.TotalCount = kpis.Count;

                // Apply pagination
                kpis = kpis.Skip((page - 1) * viewModel.PageSize)
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

                if (kpi == null)
                    return NotFound();

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
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get departments and CSFs for dropdowns and pass them in ViewBag
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CriticalSuccessFactors = await GetCsfSelectList();

                // Create a new view model
                var viewModel = new KpiViewModel
                {
                    // Basic properties
                    Type = KpiType.KeyResultIndicator,
                    KpiType = KpiType.KeyResultIndicator,
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
                KpiType = viewModel.KpiType,
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
                // Create the appropriate KPI type based on the view model
                switch (createViewModel.KpiType)
                {
                    case KpiType.KeyResultIndicator:
                        {
                            var kri = _mapper.Map<KRI>(createViewModel);
                            await _unitOfWork.KRIs.AddAsync(kri);

                            // Set audit fields
                            kri.CreatedAt = DateTime.UtcNow;
                            kri.CreatedBy = User.GetUserId();

                            // Link to CSFs if any are selected
                            if (createViewModel.SelectedCsfIds != null && createViewModel.SelectedCsfIds.Any())
                            {
                                foreach (var csfId in createViewModel.SelectedCsfIds)
                                {
                                    await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                                    {
                                        CsfId = csfId,
                                        KpiId = kri.Id,
                                        CreatedAt = DateTime.UtcNow,
                                        CreatedBy = User.GetUserId()
                                    });
                                }
                            }
                        }
                        break;
                    case KpiType.ResultIndicator:
                        {
                            var ri = _mapper.Map<RI>(createViewModel);
                            await _unitOfWork.RIs.AddAsync(ri);

                            // Set audit fields
                            ri.CreatedAt = DateTime.UtcNow;
                            ri.CreatedBy = User.GetUserId();

                            // Link to CSFs if any are selected
                            if (createViewModel.SelectedCsfIds != null && createViewModel.SelectedCsfIds.Any())
                            {
                                foreach (var csfId in createViewModel.SelectedCsfIds)
                                {
                                    await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                                    {
                                        CsfId = csfId,
                                        KpiId = ri.Id,
                                        CreatedAt = DateTime.UtcNow,
                                        CreatedBy = User.GetUserId()
                                    });
                                }
                            }
                        }
                        break;
                    case KpiType.PerformanceIndicator:
                        {
                            var pi = _mapper.Map<PI>(createViewModel);
                            await _unitOfWork.PIs.AddAsync(pi);

                            // Set audit fields
                            pi.CreatedAt = DateTime.UtcNow;
                            pi.CreatedBy = User.GetUserId();

                            // Link to CSFs if any are selected
                            if (createViewModel.SelectedCsfIds != null && createViewModel.SelectedCsfIds.Any())
                            {
                                foreach (var csfId in createViewModel.SelectedCsfIds)
                                {
                                    await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                                    {
                                        CsfId = csfId,
                                        KpiId = pi.Id,
                                        CreatedAt = DateTime.UtcNow,
                                        CreatedBy = User.GetUserId()
                                    });
                                }
                            }
                        }
                        break;
                    default:
                        ModelState.AddModelError("", "Invalid KPI type specified");
                        createViewModel.Departments = await GetDepartmentSelectList();
                        createViewModel.CriticalSuccessFactors = await GetCsfSelectList();
                        return View(createViewModel);
                }

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating KPI");
                ModelState.AddModelError("", "An error occurred while saving the KPI. Please try again.");
                createViewModel.Departments = await GetDepartmentSelectList();
                createViewModel.CriticalSuccessFactors = await GetCsfSelectList();
                return View(createViewModel);
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
        /// Processes the form submission for editing an existing KPI
        /// </summary>
        /// <param name="id">The ID of the KPI to edit</param>
        /// <param name="viewModel">The view model containing updated KPI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid id, EditKpiViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                return View(viewModel);
            }

            try
            {
                // Find the existing KPI
                KpiBase? existingKpi = await FindKpiByIdAsync(id);

                if (existingKpi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, existingKpi, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Update the KPI properties based on its type
                if (existingKpi is KRI kri)
                {
                    _mapper.Map(viewModel, kri);
                    await _unitOfWork.KRIs.UpdateAsync(kri);
                }
                else if (existingKpi is RI ri)
                {
                    _mapper.Map(viewModel, ri);
                    await _unitOfWork.RIs.UpdateAsync(ri);
                }
                else if (existingKpi is PI pi)
                {
                    _mapper.Map(viewModel, pi);
                    await _unitOfWork.PIs.UpdateAsync(pi);
                }

                // Set audit fields
                existingKpi.ModifiedAt = DateTime.UtcNow;
                existingKpi.ModifiedBy = User.GetUserId();

                // Update CSF links
                // First remove existing links
                var existingLinks = (await _unitOfWork.CSFKPIs.GetAllAsync())
                    .Where(ck => ck.KpiId == id)
                    .ToList();

                foreach (var link in existingLinks)
                {
                    await _unitOfWork.CSFKPIs.DeleteAsync(link);
                }

                // Then add new links
                if (viewModel.SelectedCsfIds != null && viewModel.SelectedCsfIds.Any())
                {
                    foreach (var csfId in viewModel.SelectedCsfIds)
                    {
                        await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                        {
                            CsfId = csfId,
                            KpiId = id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.GetUserId()
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await KpiExistsAsync(id)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating KPI with ID: {KpiId}", id);
                ModelState.AddModelError("", "An error occurred while saving the KPI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                return View(viewModel);
            }
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

                // Get total count
                viewModel.TotalCount = kpis.Count;

                // Apply pagination - important to avoid duplicate entries
                kpis = kpis.Skip((page - 1) * viewModel.PageSize)
                          .Take(viewModel.PageSize)
                          .ToList();

                // Map to view models
                viewModel.KpiItems = kpis.Select(k => _mapper.Map<KpiListItemViewModel>(k)).ToList();

                // Set view title
                ViewData["Title"] = "Đo lường";

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving measurements");
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
            // Try to find the KPI in each repository
            var kri = await _unitOfWork.KRIs.GetByIdAsync(id);
            if (kri != null)
                return kri;

            var ri = await _unitOfWork.RIs.GetByIdAsync(id);
            if (ri != null)
                return ri;

            var pi = await _unitOfWork.PIs.GetByIdAsync(id);
            return pi;
        }

        /// <summary>
        /// Checks if a KPI exists by its ID
        /// </summary>
        /// <param name="id">The ID of the KPI to check</param>
        /// <returns>True if the KPI exists, false otherwise</returns>
        private async Task<bool> KpiExistsAsync(Guid id)
        {
            return await _unitOfWork.KRIs.ExistsAsync(e => e.Id == id) ||
                   await _unitOfWork.RIs.ExistsAsync(e => e.Id == id) ||
                   await _unitOfWork.PIs.ExistsAsync(e => e.Id == id);
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

            // Get all KPIs from all repositories
            var kris = await _unitOfWork.KRIs.GetAllAsync();
            var ris = await _unitOfWork.RIs.GetAllAsync();
            var pis = await _unitOfWork.PIs.GetAllAsync();

            // Apply filter based on KPI type
            if (filter.KpiType.HasValue)
            {
                switch (filter.KpiType.Value)
                {
                    case KpiType.KeyResultIndicator:
                        kpis.AddRange(kris.Cast<KpiBase>());
                        break;
                    case KpiType.ResultIndicator:
                        kpis.AddRange(ris.Cast<KpiBase>());
                        break;
                    case KpiType.PerformanceIndicator:
                        kpis.AddRange(pis.Cast<KpiBase>());
                        break;
                    default:
                        kpis.AddRange(kris.Cast<KpiBase>());
                        kpis.AddRange(ris.Cast<KpiBase>());
                        kpis.AddRange(pis.Cast<KpiBase>());
                        break;
                }
            }
            else
            {
                kpis.AddRange(kris.Cast<KpiBase>());
                kpis.AddRange(ris.Cast<KpiBase>());
                kpis.AddRange(pis.Cast<KpiBase>());
            }

            // Apply other filters
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
    }
}