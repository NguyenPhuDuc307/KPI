using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KPISolution.Authorization;
using KPISolution.Authorization.Handlers;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Extensions;
using KPISolution.Models.Entities.KPI;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.KPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing Key Result Indicators (KRIs)
    /// </summary>
    [Authorize]
    public class KriController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<KriController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="KriController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations</param>
        /// <param name="logger">The logger for logging information and errors</param>
        /// <param name="mapper">The mapper for mapping between entities and view models</param>
        /// <param name="authorizationService">The authorization service for checking permissions</param>
        public KriController(
            IUnitOfWork unitOfWork,
            ILogger<KriController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Displays the list of KRIs with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria for KRIs</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>View with list of KRIs</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Index(KpiFilterViewModel filter, int page = 1)
        {
            try
            {
                // Force KPI type to be KRI
                filter.KpiType = KpiType.KeyResultIndicator;

                var viewModel = new KpiListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<KpiCategory>(),
                    Frequencies = GetEnumSelectList<MeasurementFrequency>(),
                    BusinessAreas = GetEnumSelectList<BusinessArea>(),
                    ImpactLevels = GetEnumSelectList<ImpactLevel>(),
                    PageSize = 10
                };

                // Get KRIs as a list first to avoid multiple enumeration
                var allKris = await _unitOfWork.KRIs.GetAllAsync();
                var query = allKris.AsQueryable();

                // Apply additional filters specific to KRIs
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    query = query.Where(k => k.Name.Contains(filter.SearchTerm) ||
                                        k.Code.Contains(filter.SearchTerm) ||
                                        (k.Description != null && k.Description.Contains(filter.SearchTerm)));
                }

                if (!string.IsNullOrWhiteSpace(filter.Department))
                {
                    query = query.Where(k => k.Department == filter.Department);
                }

                if (filter.Status.HasValue)
                {
                    query = query.Where(k => k.Status == filter.Status.Value);
                }

                if (filter.BusinessArea.HasValue)
                {
                    query = query.Where(k => k.BusinessArea == filter.BusinessArea.Value);
                }

                if (filter.ImpactLevel.HasValue)
                {
                    query = query.Where(k => k.ImpactLevel == filter.ImpactLevel.Value);
                }

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Get total count before pagination
                viewModel.TotalCount = query.Count();

                // Apply pagination
                var kris = query.Skip((page - 1) * viewModel.PageSize)
                              .Take(viewModel.PageSize)
                              .ToList();

                // Map to view models and add KRI-specific information
                viewModel.KpiItems = kris.Select(k =>
                {
                    var item = _mapper.Map<KpiListItemViewModel>(k);
                    // Add KRI-specific properties
                    item.KpiType = KpiType.KeyResultIndicator;
                    item.StrategicObjective = k.StrategicObjective;
                    item.ExecutiveOwner = k.ExecutiveOwner;
                    item.StatusString = k.Status.ToString();
                    return item;
                }).ToList();

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving KRI list");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the details of a specific KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to display</param>
        /// <returns>View with KRI details</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the KRI from the repository
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id.Value);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Read);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model for display
                var viewModel = _mapper.Map<KpiDetailsViewModel>(kri);
                viewModel.KpiType = KpiType.KeyResultIndicator;

                // Set calculated properties
                if (viewModel.ImpactLevel.HasValue)
                {
                    viewModel.ImpactLevelDisplay = viewModel.ImpactLevel.Value.ToString();
                }

                if (viewModel.BusinessArea.HasValue)
                {
                    viewModel.BusinessAreaDisplay = viewModel.BusinessArea.Value.ToString();
                }

                // Load linked CSFs
                var linkedCsfs = new List<LinkedCsfViewModel>();
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                var csfLinks = csfKpis.Where(ck => ck.KpiId == id).ToList();

                foreach (var link in csfLinks)
                {
                    var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(link.CsfId);
                    if (csf != null)
                    {
                        var csfViewModel = _mapper.Map<LinkedCsfViewModel>(csf);
                        linkedCsfs.Add(csfViewModel);
                    }
                }

                viewModel.LinkedCsfs = linkedCsfs;

                // Load measurements for displaying history and current value
                var measurements = await _unitOfWork.KpiMeasurements.GetAllAsync();
                var kriMeasurements = measurements
                    .Where(m => m.KpiId == id)
                    .OrderByDescending(m => m.MeasurementDate)
                    .ToList();

                // Map measurements to view models
                var historicalValues = new List<KpiValueViewModel>();
                foreach (var measurement in kriMeasurements)
                {
                    var valueViewModel = new KpiValueViewModel
                    {
                        Id = measurement.Id,
                        KpiId = measurement.KpiId,
                        KpiType = KpiType.KeyResultIndicator.ToString(),
                        ActualValue = measurement.Value,
                        TargetValue = kri.TargetValue,
                        MeasurementDate = measurement.MeasurementDate,
                        Notes = measurement.Notes,
                        CreatedAt = measurement.CreatedAt,
                        CreatedBy = measurement.CreatedBy
                    };

                    // Calculate status and achievement percentage
                    if (kri.TargetValue != 0)
                    {
                        valueViewModel.AchievementPercentage = (measurement.Value / kri.TargetValue) * 100;

                        // Set status based on direction and target
                        if (kri.MeasurementDirection == MeasurementDirection.HigherIsBetter)
                        {
                            valueViewModel.Status = measurement.Value >= kri.TargetValue ? "On Target" : "Below Target";
                            valueViewModel.StatusCssClass = measurement.Value >= kri.TargetValue ? "text-success" : "text-danger";
                        }
                        else if (kri.MeasurementDirection == MeasurementDirection.LowerIsBetter)
                        {
                            valueViewModel.Status = measurement.Value <= kri.TargetValue ? "On Target" : "Above Target";
                            valueViewModel.StatusCssClass = measurement.Value <= kri.TargetValue ? "text-success" : "text-danger";
                        }
                    }

                    historicalValues.Add(valueViewModel);
                }

                viewModel.HistoricalValues = historicalValues;

                // Set current value from most recent measurement
                if (kriMeasurements.Any())
                {
                    viewModel.CurrentValue = kriMeasurements.First().Value;
                    viewModel.LastMeasuredAt = kriMeasurements.First().MeasurementDate;
                }

                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving KRI details for ID: {KriId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form for creating a new KRI
        /// </summary>
        /// <returns>Create KRI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new CreateKpiViewModel
                {
                    KpiType = KpiType.KeyResultIndicator,
                    Departments = await GetDepartmentSelectList(),
                    CriticalSuccessFactors = await GetCsfSelectList(),
                    BusinessAreas = GetEnumSelectList<BusinessArea>(),
                    ImpactLevels = GetEnumSelectList<ImpactLevel>()
                };

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KRI creation form");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for creating a new KRI
        /// </summary>
        /// <param name="viewModel">The view model containing KRI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(CreateKpiViewModel viewModel)
        {
            // Force KPI type to be KRI
            viewModel.KpiType = KpiType.KeyResultIndicator;

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.BusinessAreas = GetEnumSelectList<BusinessArea>();
                viewModel.ImpactLevels = GetEnumSelectList<ImpactLevel>();
                return View("Create", viewModel);
            }

            try
            {
                // Create KRI from view model
                var kri = _mapper.Map<KRI>(viewModel);

                // Set additional KRI-specific properties if not mapped by AutoMapper
                if (viewModel.ImpactLevel.HasValue)
                {
                    kri.ImpactLevel = viewModel.ImpactLevel.Value;
                }

                if (viewModel.BusinessArea.HasValue)
                {
                    kri.BusinessArea = viewModel.BusinessArea.Value;
                }

                // Set audit fields
                kri.CreatedAt = DateTime.UtcNow;
                kri.CreatedBy = User.GetUserId();

                await _unitOfWork.KRIs.AddAsync(kri);

                // Link to CSFs if any are selected
                if (viewModel.SelectedCsfIds != null && viewModel.SelectedCsfIds.Any())
                {
                    foreach (var csfId in viewModel.SelectedCsfIds)
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

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating KRI");
                ModelState.AddModelError("", "An error occurred while saving the KRI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.BusinessAreas = GetEnumSelectList<BusinessArea>();
                viewModel.ImpactLevels = GetEnumSelectList<ImpactLevel>();
                return View("Create", viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to edit</param>
        /// <returns>Edit KRI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the KRI from the repository
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id.Value);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to edit view model
                var viewModel = _mapper.Map<EditKpiViewModel>(kri);
                viewModel.KpiType = KpiType.KeyResultIndicator;

                // Set KRI-specific properties
                viewModel.ImpactLevel = kri.ImpactLevel;
                viewModel.BusinessArea = kri.BusinessArea;

                // Get selected CSF IDs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                viewModel.SelectedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                // Populate dropdown items
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.BusinessAreas = GetEnumSelectList<BusinessArea>();
                viewModel.ImpactLevels = GetEnumSelectList<ImpactLevel>();

                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KRI edit form for ID: {KriId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for editing an existing KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to edit</param>
        /// <param name="viewModel">The view model containing updated KRI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid id, EditKpiViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            // Force KPI type to be KRI
            viewModel.KpiType = KpiType.KeyResultIndicator;

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.BusinessAreas = GetEnumSelectList<BusinessArea>();
                viewModel.ImpactLevels = GetEnumSelectList<ImpactLevel>();
                return View("Edit", viewModel);
            }

            try
            {
                // Get the existing KRI
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Update KRI properties
                _mapper.Map(viewModel, kri);

                // Set KRI-specific properties if not mapped by AutoMapper
                if (viewModel.ImpactLevel.HasValue)
                {
                    kri.ImpactLevel = viewModel.ImpactLevel.Value;
                }

                if (viewModel.BusinessArea.HasValue)
                {
                    kri.BusinessArea = viewModel.BusinessArea.Value;
                }

                // Set audit fields
                kri.ModifiedAt = DateTime.UtcNow;
                kri.ModifiedBy = User.GetUserId();

                await _unitOfWork.KRIs.UpdateAsync(kri);

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
                            KpiId = kri.Id,
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
                if (!(await KriExistsAsync(id)))
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
                _logger.LogError(ex, "Error occurred while updating KRI with ID: {KriId}", id);
                ModelState.AddModelError("", "An error occurred while saving the KRI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.BusinessAreas = GetEnumSelectList<BusinessArea>();
                viewModel.ImpactLevels = GetEnumSelectList<ImpactLevel>();
                return View("Edit", viewModel);
            }
        }

        /// <summary>
        /// Displays the confirmation form for deleting a KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to delete</param>
        /// <returns>Delete confirmation view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the KRI from the repository
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id.Value);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Delete);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model for display
                var viewModel = _mapper.Map<KpiDetailsViewModel>(kri);
                viewModel.KpiType = KpiType.KeyResultIndicator;

                return View("Delete", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KRI deletion for ID: {KriId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for deleting a KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to delete</param>
        /// <returns>Redirect to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Get the KRI from the repository
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Delete);

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

                    // Delete the KRI
                    await _unitOfWork.KRIs.DeleteAsync(kri);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "Error occurred during transaction while deleting KRI with ID: {KriId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting KRI with ID: {KriId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form for adding a measurement to a KRI
        /// </summary>
        /// <param name="id">The ID of the KRI to add a measurement for</param>
        /// <returns>Add measurement form view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> AddMeasurement(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the KRI from the repository
                var kri = await _unitOfWork.KRIs.GetByIdAsync(id.Value);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Create view model for the form
                var viewModel = new AddKpiMeasurementViewModel
                {
                    KpiId = kri.Id,
                    KpiName = kri.Name,
                    KpiCode = kri.Code,
                    Unit = kri.Unit,
                    TargetValue = kri.TargetValue,
                    MeasurementDate = DateTime.Now,
                    Period = DateTime.Now.ToString("MMM yyyy") // e.g., Mar 2025
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing KRI measurement form for ID: {KriId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for adding a measurement to a KRI
        /// </summary>
        /// <param name="viewModel">The view model containing measurement data</param>
        /// <returns>Redirect to Details on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> AddMeasurement(AddKpiMeasurementViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                // Get the KRI to ensure it exists
                var kri = await _unitOfWork.KRIs.GetByIdAsync(viewModel.KpiId);

                if (kri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, kri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Create new KPI measurement
                var measurement = new KpiMeasurement
                {
                    KpiId = viewModel.KpiId,
                    Value = viewModel.Value,
                    MeasurementDate = viewModel.MeasurementDate,
                    Status = viewModel.Status,
                    Notes = viewModel.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.GetUserId()
                };

                // Add measurement to repository
                await _unitOfWork.KpiMeasurements.AddAsync(measurement);
                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = viewModel.KpiId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving KRI measurement for KPI ID: {KpiId}", viewModel.KpiId);
                ModelState.AddModelError("", "An error occurred while saving the measurement. Please try again.");
                return View(viewModel);
            }
        }

        // Helper methods

        /// <summary>
        /// Checks if a KRI exists by its ID
        /// </summary>
        /// <param name="id">The ID of the KRI to check</param>
        /// <returns>True if the KRI exists, false otherwise</returns>
        private async Task<bool> KriExistsAsync(Guid id)
        {
            return await _unitOfWork.KRIs.ExistsAsync(e => e.Id == id);
        }

        /// <summary>
        /// Gets a select list of departments for dropdowns
        /// </summary>
        /// <returns>List of SelectListItem for departments</returns>
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
        /// <returns>List of SelectListItem for CSFs</returns>
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
        /// <returns>List of SelectListItem for the enum values</returns>
        private SelectList GetEnumSelectList<TEnum>() where TEnum : struct, Enum
        {
            var items = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString().SplitCamelCase()
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// Applies sorting to a query based on sort parameters
        /// </summary>
        /// <param name="query">The query to sort</param>
        /// <param name="sortBy">The property to sort by</param>
        /// <param name="sortDirection">The sort direction (asc/desc)</param>
        /// <returns>The sorted query</returns>
        private IQueryable<KRI> ApplySorting(IQueryable<KRI> query, string? sortBy, string? sortDirection)
        {
            var isDescending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(k => k.Name) : query.OrderBy(k => k.Name),
                "code" => isDescending ? query.OrderByDescending(k => k.Code) : query.OrderBy(k => k.Code),
                "department" => isDescending ? query.OrderByDescending(k => k.Department) : query.OrderBy(k => k.Department),
                "frequency" => isDescending ? query.OrderByDescending(k => k.Frequency) : query.OrderBy(k => k.Frequency),
                "effectivedate" => isDescending ? query.OrderByDescending(k => k.EffectiveDate) : query.OrderBy(k => k.EffectiveDate),
                "status" => isDescending ? query.OrderByDescending(k => k.Status) : query.OrderBy(k => k.Status),
                "strategicobjective" => isDescending ? query.OrderByDescending(k => k.StrategicObjective) : query.OrderBy(k => k.StrategicObjective),
                "businessarea" => isDescending ? query.OrderByDescending(k => k.BusinessArea) : query.OrderBy(k => k.BusinessArea),
                "impactlevel" => isDescending ? query.OrderByDescending(k => k.ImpactLevel) : query.OrderBy(k => k.ImpactLevel),
                _ => query.OrderBy(k => k.Name) // Default sort
            };
        }

        private SelectList GetImpactLevelSelectList()
        {
            var items = Enum.GetValues(typeof(ImpactLevel))
                .Cast<ImpactLevel>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString().SplitCamelCase()
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        private SelectList GetBusinessAreaSelectList()
        {
            var items = Enum.GetValues(typeof(BusinessArea))
                .Cast<BusinessArea>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString().SplitCamelCase()
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }
    }
}