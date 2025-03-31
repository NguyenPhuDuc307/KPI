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
using KPISolution.Models.Entities.Measurement;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.KPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for managing Result Indicators (RIs)
    /// </summary>
    [Authorize]
    public class RiController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RiController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RiController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations</param>
        /// <param name="logger">The logger for logging information and errors</param>
        /// <param name="mapper">The mapper for mapping between entities and view models</param>
        /// <param name="authorizationService">The authorization service for checking permissions</param>
        public RiController(
            IUnitOfWork unitOfWork,
            ILogger<RiController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Displays the list of RIs with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter criteria for RIs</param>
        /// <param name="page">Page number for pagination</param>
        /// <returns>View with list of RIs</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Index(KpiFilterViewModel filter, int page = 1)
        {
            try
            {
                // Force KPI type to be RI
                filter.KpiType = KpiType.ResultIndicator;

                var viewModel = new KpiListViewModel
                {
                    Filter = filter,
                    CurrentPage = page,
                    Departments = await GetDepartmentSelectList(),
                    Categories = GetEnumSelectList<KpiCategory>(),
                    Frequencies = GetEnumSelectList<MeasurementFrequency>(),
                    PageSize = 10
                };

                // Get RIs as a list first to avoid multiple enumeration
                var allRis = await _unitOfWork.RIs.GetAllAsync();
                var query = allRis.AsQueryable();

                // Apply additional filters specific to RIs
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

                if (filter.ProcessArea.HasValue)
                {
                    query = query.Where(k => k.ProcessArea == filter.ProcessArea.Value);
                }

                // Apply sorting
                query = ApplySorting(query, filter.SortBy, filter.SortDirection);

                // Get total count before pagination
                viewModel.TotalCount = query.Count();

                // Apply pagination
                var ris = query.Skip((page - 1) * viewModel.PageSize)
                              .Take(viewModel.PageSize)
                              .ToList();

                // Map to view models and add RI-specific information
                viewModel.KpiItems = ris.Select(k =>
                {
                    var item = _mapper.Map<KpiListItemViewModel>(k);
                    // Add RI-specific properties
                    item.KpiType = KpiType.ResultIndicator;
                    item.ProcessArea = k.ProcessArea.ToString();
                    item.StatusString = k.Status.ToString();
                    return item;
                }).ToList();

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving RI list");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the details of a specific RI
        /// </summary>
        /// <param name="id">The ID of the RI to display</param>
        /// <returns>View with RI details</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the RI from the repository
                var ri = await _unitOfWork.RIs.GetByIdAsync(id.Value);

                if (ri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, ri, KpiAuthorizationHandler.Operations.Read);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model
                var viewModel = _mapper.Map<KpiDetailsViewModel>(ri);
                viewModel.KpiType = KpiType.ResultIndicator;
                viewModel.ProcessAreaDisplay = ri.ProcessArea.ToString();

                // Get historical values if available
                var kpiValues = await _unitOfWork.KpiValues.GetAllAsync();
                var filteredValues = kpiValues
                    .Where(v => v.KpiId == id)
                    .OrderByDescending(v => v.MeasurementDate)
                    .ToList();

                // Initialize collection if null
                viewModel.HistoricalValues ??= new List<KpiValueViewModel>();

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

                // Initialize collection if null
                viewModel.LinkedCsfs ??= new List<LinkedCsfViewModel>();

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

                // Get parent KRI if available
                if (ri.ParentKriId.HasValue)
                {
                    var kri = await _unitOfWork.KRIs.GetByIdAsync(ri.ParentKriId.Value);
                    if (kri != null)
                    {
                        viewModel.ParentKpi = _mapper.Map<LinkedKpiViewModel>(kri);
                    }
                }

                // Set RI-specific properties
                viewModel.ProcessArea = ri.ProcessArea;
                viewModel.ParentKriId = ri.ParentKriId;

                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving RI details for ID: {RiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the form for creating a new RI
        /// </summary>
        /// <param name="kriId">Optional parent KRI ID</param>
        /// <returns>Create RI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(Guid? kriId = null)
        {
            try
            {
                var viewModel = new CreateKpiViewModel
                {
                    KpiType = KpiType.ResultIndicator,
                    Departments = await GetDepartmentSelectList(),
                    CriticalSuccessFactors = await GetCsfSelectList(),
                    ParentKris = await GetKriSelectList(),
                    ProcessAreas = GetEnumSelectList<ProcessArea>()
                };

                // If kriId is provided, preselect the parent KRI
                if (kriId.HasValue)
                {
                    viewModel.ParentKriId = kriId.Value;

                    // Get the parent KRI details to potentially inherit some properties
                    var kri = await _unitOfWork.KRIs.GetByIdAsync(kriId.Value);
                    if (kri != null)
                    {
                        // Inherit some properties from parent KRI
                        viewModel.Department = kri.Department;
                        viewModel.Owner = kri.ResponsiblePerson;
                    }
                }

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing RI creation form");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for creating a new RI
        /// </summary>
        /// <param name="viewModel">The view model containing RI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(CreateKpiViewModel viewModel)
        {
            // Force KPI type to be RI
            viewModel.KpiType = KpiType.ResultIndicator;

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                return View("Create", viewModel);
            }

            try
            {
                // Create RI from view model
                var ri = _mapper.Map<RI>(viewModel);

                // Set additional RI-specific properties if not mapped by AutoMapper
                if (viewModel.ProcessArea.HasValue)
                {
                    ri.ProcessArea = viewModel.ProcessArea.Value;
                }

                if (viewModel.ParentKriId.HasValue)
                {
                    ri.ParentKriId = viewModel.ParentKriId;
                }

                // Set audit fields
                ri.CreatedAt = DateTime.UtcNow;
                ri.CreatedBy = User.GetUserId();

                await _unitOfWork.RIs.AddAsync(ri);

                // Link to CSFs if any are selected
                if (viewModel.SelectedCsfIds != null && viewModel.SelectedCsfIds.Any())
                {
                    foreach (var csfId in viewModel.SelectedCsfIds)
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

                await _unitOfWork.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating RI");
                ModelState.AddModelError("", "An error occurred while saving the RI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                return View("Create", viewModel);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing RI
        /// </summary>
        /// <param name="id">The ID of the RI to edit</param>
        /// <returns>Edit RI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the RI from the repository
                var ri = await _unitOfWork.RIs.GetByIdAsync(id.Value);

                if (ri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, ri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to edit view model
                var viewModel = _mapper.Map<EditKpiViewModel>(ri);
                viewModel.KpiType = KpiType.ResultIndicator;

                // Set RI-specific properties
                viewModel.ProcessArea = ri.ProcessArea;
                viewModel.ParentKriId = ri.ParentKriId;

                // Get selected CSF IDs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                viewModel.SelectedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                // Populate dropdown items
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();

                return View("Edit", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing RI edit form for ID: {RiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for editing an existing RI
        /// </summary>
        /// <param name="id">The ID of the RI to edit</param>
        /// <param name="viewModel">The view model containing updated RI data</param>
        /// <returns>Redirect to Index on success, view with errors otherwise</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Edit(Guid id, EditKpiViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            // Force KPI type to be RI
            viewModel.KpiType = KpiType.ResultIndicator;

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                return View("Edit", viewModel);
            }

            try
            {
                // Get the existing RI
                var ri = await _unitOfWork.RIs.GetByIdAsync(id);

                if (ri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, ri, KpiAuthorizationHandler.Operations.Update);

                if (!authResult.Succeeded)
                    return Forbid();

                // Update RI properties
                _mapper.Map(viewModel, ri);

                // Set RI-specific properties if not mapped by AutoMapper
                if (viewModel.ProcessArea.HasValue)
                {
                    ri.ProcessArea = viewModel.ProcessArea.Value;
                }

                if (viewModel.ParentKriId.HasValue)
                {
                    ri.ParentKriId = viewModel.ParentKriId;
                }

                // Set audit fields
                ri.ModifiedAt = DateTime.UtcNow;
                ri.ModifiedBy = User.GetUserId();

                await _unitOfWork.RIs.UpdateAsync(ri);

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
                            KpiId = ri.Id,
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
                if (!(await RiExistsAsync(id)))
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
                _logger.LogError(ex, "Error occurred while updating RI with ID: {RiId}", id);
                ModelState.AddModelError("", "An error occurred while saving the RI. Please try again.");
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                return View("Edit", viewModel);
            }
        }

        /// <summary>
        /// Displays the confirmation form for deleting a RI
        /// </summary>
        /// <param name="id">The ID of the RI to delete</param>
        /// <returns>Delete confirmation view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                // Get the RI from the repository
                var ri = await _unitOfWork.RIs.GetByIdAsync(id.Value);

                if (ri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, ri, KpiAuthorizationHandler.Operations.Delete);

                if (!authResult.Succeeded)
                    return Forbid();

                // Map to view model for display
                var viewModel = _mapper.Map<KpiDetailsViewModel>(ri);
                viewModel.KpiType = KpiType.ResultIndicator;

                return View("Delete", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing RI deletion for ID: {RiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Processes the form submission for deleting a RI
        /// </summary>
        /// <param name="id">The ID of the RI to delete</param>
        /// <returns>Redirect to Index on success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Get the RI from the repository
                var ri = await _unitOfWork.RIs.GetByIdAsync(id);

                if (ri == null)
                    return NotFound();

                // Check authorization
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, ri, KpiAuthorizationHandler.Operations.Delete);

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

                    // Delete the RI
                    await _unitOfWork.RIs.DeleteAsync(ri);

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    _logger.LogError(ex, "Error occurred during transaction while deleting RI with ID: {RiId}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting RI with ID: {RiId}", id);
                return RedirectToAction("Error", "Home");
            }
        }

        // Helper methods

        /// <summary>
        /// Checks if a RI exists by its ID
        /// </summary>
        /// <param name="id">The ID of the RI to check</param>
        /// <returns>True if the RI exists, false otherwise</returns>
        private async Task<bool> RiExistsAsync(Guid id)
        {
            return await _unitOfWork.RIs.ExistsAsync(e => e.Id == id);
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
        /// Gets a select list of KRIs for dropdowns
        /// </summary>
        /// <returns>List of SelectListItem for KRIs</returns>
        private async Task<SelectList> GetKriSelectList()
        {
            var kris = await _unitOfWork.KRIs.GetAllAsync();
            var items = kris
                .OrderBy(k => k.Name)
                .Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = $"{k.Code} - {k.Name}"
                })
                .ToList();

            return new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// Creates a select list from an enum type
        /// </summary>
        /// <typeparam name="T">The enum type</typeparam>
        /// <returns>List of SelectListItem for the enum values</returns>
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

        /// <summary>
        /// Applies sorting to a query based on sort parameters
        /// </summary>
        /// <param name="query">The query to sort</param>
        /// <param name="sortBy">The property to sort by</param>
        /// <param name="sortDirection">The sort direction (asc/desc)</param>
        /// <returns>The sorted query</returns>
        private IQueryable<RI> ApplySorting(IQueryable<RI> query, string? sortBy, string? sortDirection)
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
                "processarea" => isDescending ? query.OrderByDescending(k => k.ProcessArea) : query.OrderBy(k => k.ProcessArea),
                _ => query.OrderBy(k => k.Name) // Default sort
            };
        }
    }
}