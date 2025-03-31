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
    /// Controller for managing Performance Indicators (PIs)
    /// </summary>
    [Authorize]
    public class PIController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PIController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PIController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations</param>
        /// <param name="logger">The logger for logging information and errors</param>
        /// <param name="mapper">The mapper for mapping between entities and view models</param>
        /// <param name="authorizationService">The authorization service for checking permissions</param>
        public PIController(
            IUnitOfWork unitOfWork,
            ILogger<PIController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Displays the list of PIs with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria for PIs</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>View with list of PIs</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Index(KpiFilterViewModel filter, int page = 1)
        {
            try
            {
                // Force KPI type to be PI
                filter.KpiType = KpiType.PerformanceIndicator;

                var viewModel = new KpiListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<KpiCategory>(),
                    Frequencies = GetEnumSelectList<MeasurementFrequency>(),
                    PageSize = 10
                };

                // Get PIs as a list first to avoid multiple enumeration
                var allPis = await _unitOfWork.PIs.GetAllAsync();
                var query = allPis.AsQueryable();

                // Apply additional filters specific to PIs
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

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Get total count before pagination
                viewModel.TotalCount = query.Count();

                // Apply pagination
                var pis = query.Skip((page - 1) * viewModel.PageSize)
                              .Take(viewModel.PageSize)
                              .ToList();

                // Map to view models and add PI-specific information
                viewModel.KpiItems = pis.Select(k =>
                {
                    var item = _mapper.Map<KpiListItemViewModel>(k);
                    // Add PI-specific properties
                    item.KpiType = KpiType.PerformanceIndicator;
                    item.ActivityTypeName = k.ActivityType.ToString();
                    item.StatusString = k.Status.ToString();
                    return item;
                }).ToList();

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving PI list");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the details of a specific PI
        /// </summary>
        /// <param name="id">The ID of the PI to display</param>
        /// <returns>View with PI details</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the PI from the repository
                var pi = await _unitOfWork.PIs.GetByIdAsync(id.Value);

                if (pi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, pi, KpiAuthorizationHandler.Operations.Read);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model
                var viewModel = _mapper.Map<KpiDetailsViewModel>(pi);
                viewModel.KpiType = KpiType.PerformanceIndicator;
                viewModel.ActivityTypeDisplay = pi.ActivityType.ToString();

                // Get historical values if available
                var kpiValues = await _unitOfWork.KpiValues.GetAllAsync();
                var filteredValues = kpiValues
                    .Where(v => v.KpiId == id)
                    .OrderByDescending(v => v.MeasurementDate)
                    .ToList();

                // Initialize empty lists if not already initialized
                viewModel.HistoricalValues = new List<KpiValueViewModel>();
                viewModel.LinkedCsfs = new List<LinkedCsfViewModel>();

                // Manually map and add each value to the collection
                foreach (var value in filteredValues)
                {
                    viewModel.HistoricalValues.Add(_mapper.Map<KpiValueViewModel>(value));
                }

                // Get linked CSFs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                var linkedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                if (linkedCsfIds.Any())
                {
                    var csfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                    var linkedCsfs = csfs
                        .Where(c => linkedCsfIds.Contains(c.Id))
                        .ToList();

                    // Manually map and add each CSF to the collection
                    foreach (var csf in linkedCsfs)
                    {
                        viewModel.LinkedCsfs.Add(_mapper.Map<LinkedCsfViewModel>(csf));
                    }
                }

                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving PI details for ID: {PiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form for creating a new PI
        /// </summary>
        /// <returns>Create PI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var viewModel = new KpiViewModel
                {
                    KpiType = KpiType.PerformanceIndicator,
                    Type = KpiType.PerformanceIndicator,
                    Status = "Draft",
                    Frequency = MeasurementFrequency.Monthly,
                    MeasurementDirection = MeasurementDirection.HigherIsBetter,
                    EffectiveDate = DateTime.Now
                };

                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CSFs = await GetCsfSelectList();
                ViewBag.Users = new SelectList(new List<SelectListItem>());
                ViewBag.ActivityTypes = GetEnumSelectList<ActivityType>();

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing PI creation form");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for creating a new PI
        /// </summary>
        /// <param name="viewModel">The view model containing PI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(KpiViewModel viewModel, [FromForm] List<Guid> SelectedCsfs)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CSFs = await GetCsfSelectList();
                ViewBag.Users = new SelectList(new List<SelectListItem>());
                ViewBag.ActivityTypes = GetEnumSelectList<ActivityType>();
                return View("Create", viewModel);
            }

            try
            {
                // Convert to CreateKpiViewModel for mapping
                var createViewModel = new CreateKpiViewModel
                {
                    KpiType = viewModel.KpiType,
                    Name = viewModel.Name,
                    Description = viewModel.Description ?? string.Empty,
                    Code = viewModel.Code,
                    Unit = viewModel.Unit,
                    TargetValue = viewModel.TargetValue ?? 0,
                    DepartmentId = viewModel.DepartmentId,
                    Owner = viewModel.Owner,
                    MeasurementDirection = viewModel.MeasurementDirection,
                    MeasurementFrequency = viewModel.Frequency,
                    Weight = viewModel.Weight.HasValue ? (int)viewModel.Weight.Value : 0,
                    Formula = viewModel.Formula,
                    ActionPlan = viewModel.ActionPlan,
                    ActivityType = viewModel.ActivityType,
                    EffectiveDate = viewModel.EffectiveDate ?? DateTime.Now,
                    SelectedCsfIds = SelectedCsfs
                };

                // Create PI from view model
                var pi = _mapper.Map<PI>(createViewModel);

                // Set PI-specific properties if not mapped by AutoMapper
                if (viewModel.ActivityType.HasValue)
                {
                    pi.ActivityType = viewModel.ActivityType.Value;
                }

                // PerformanceLevel is a non-nullable int - assign directly
                if (int.TryParse(viewModel.PerformanceLevel, out int performanceLevel))
                {
                    pi.PerformanceLevel = performanceLevel;
                }
                else
                {
                    pi.PerformanceLevel = 3; // Default value if parsing fails
                }

                // Set audit fields
                pi.CreatedAt = DateTime.UtcNow;
                pi.CreatedBy = User.GetUserId();

                await _unitOfWork.PIs.AddAsync(pi);

                // Link to CSFs if any are selected
                if (SelectedCsfs != null && SelectedCsfs.Any())
                {
                    foreach (var csfId in SelectedCsfs)
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

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating PI");
                ModelState.AddModelError("", "An error occurred while saving the PI. Please try again.");
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CSFs = await GetCsfSelectList();
                ViewBag.Users = new SelectList(new List<SelectListItem>());
                ViewBag.ActivityTypes = GetEnumSelectList<ActivityType>();
                return View("Create", viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing PI
        /// </summary>
        /// <param name="id">The ID of the PI to edit</param>
        /// <returns>Edit PI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the PI from the repository
                var pi = await _unitOfWork.PIs.GetByIdAsync(id.Value);

                if (pi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, pi, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to edit view model
                var viewModel = _mapper.Map<EditKpiViewModel>(pi);

                // Set PI-specific properties
                viewModel.ActivityType = pi.ActivityType;

                viewModel.PerformanceLevel = pi.PerformanceLevel;

                // Get selected CSF IDs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                viewModel.SelectedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                // Populate dropdown items
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ActivityTypes = GetEnumSelectList<ActivityType>();

                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing PI edit form for ID: {PiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for editing an existing PI
        /// </summary>
        /// <param name="id">The ID of the PI to edit</param>
        /// <param name="viewModel">The view model containing updated PI data</param>
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
                viewModel.ActivityTypes = GetEnumSelectList<ActivityType>();
                return View("Edit", viewModel);
            }

            try
            {
                // Get the existing PI
                var pi = await _unitOfWork.PIs.GetByIdAsync(id);

                if (pi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, pi, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Update PI properties
                _mapper.Map(viewModel, pi);

                // Set PI-specific properties if not mapped by AutoMapper
                if (viewModel.ActivityType.HasValue)
                {
                    pi.ActivityType = viewModel.ActivityType.Value;
                }

                // PerformanceLevel is a non-nullable int - assign with default value if null
                pi.PerformanceLevel = viewModel.PerformanceLevel.GetValueOrDefault(3);

                // Set audit fields
                pi.ModifiedAt = DateTime.UtcNow;
                pi.ModifiedBy = User.GetUserId();

                await _unitOfWork.PIs.UpdateAsync(pi);

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
                            KpiId = pi.Id,
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
                if (!(await PiExistsAsync(id)))
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
                _logger.LogError(ex, "Error occurred while updating PI with ID: {PiId}", id);
                ModelState.AddModelError("", "An error occurred while saving the PI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ActivityTypes = GetEnumSelectList<ActivityType>();
                return View("Edit", viewModel);
            }
        }

        /// <summary>
        /// Displays the confirmation form for deleting a PI
        /// </summary>
        /// <param name="id">The ID of the PI to delete</param>
        /// <returns>Delete confirmation view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the PI from the repository
                var pi = await _unitOfWork.PIs.GetByIdAsync(id.Value);

                if (pi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, pi, KpiAuthorizationHandler.Operations.Delete);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model for display
                var viewModel = _mapper.Map<KpiDetailsViewModel>(pi);
                viewModel.KpiType = KpiType.PerformanceIndicator;

                return View("Delete", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing PI deletion for ID: {PiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for deleting a PI
        /// </summary>
        /// <param name="id">The ID of the PI to delete</param>
        /// <returns>Redirect to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Get the PI from the repository
                var pi = await _unitOfWork.PIs.GetByIdAsync(id);

                if (pi == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, pi, KpiAuthorizationHandler.Operations.Delete);

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

                    // Delete the PI
                    await _unitOfWork.PIs.DeleteAsync(pi);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "Error occurred during transaction while deleting PI with ID: {PiId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting PI with ID: {PiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // Helper methods

        /// <summary>
        /// Checks if a PI exists by its ID
        /// </summary>
        /// <param name="id">The ID of the PI to check</param>
        /// <returns>True if the PI exists, false otherwise</returns>
        private async Task<bool> PiExistsAsync(Guid id)
        {
            return await _unitOfWork.PIs.ExistsAsync(e => e.Id == id);
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
        private IQueryable<PI> ApplySorting(IQueryable<PI> query, string? sortBy, string? sortDirection)
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
                "activitytype" => isDescending ? query.OrderByDescending(k => k.ActivityType) : query.OrderBy(k => k.ActivityType),
                "performancelevel" => isDescending ? query.OrderByDescending(k => k.PerformanceLevel) : query.OrderBy(k => k.PerformanceLevel),
                _ => query.OrderBy(k => k.Name) // Default sort
            };
        }
    }
}