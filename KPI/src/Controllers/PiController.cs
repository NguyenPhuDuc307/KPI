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
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="PIController"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations</param>
        /// <param name="logger">The logger for logging information and errors</param>
        /// <param name="mapper">The mapper for mapping between entities and view models</param>
        /// <param name="authorizationService">The authorization service for checking permissions</param>
        /// <param name="memoryCache">The memory cache for caching data</param>
        public PIController(
            IUnitOfWork unitOfWork,
            ILogger<PIController> logger,
            IMapper mapper,
            IAuthorizationService authorizationService,
            IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _memoryCache = memoryCache;
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

                // Get total count for pagination
                viewModel.TotalCount = query.Count();

                // Apply pagination
                var pis = query
                            .OrderBy(p => p.Id)
                            .Skip((page - 1) * viewModel.PageSize)
                            .Take(viewModel.PageSize)
                            .ToList();

                // Sử dụng AutoMapper để map toàn bộ properties từ PI sang KpiListItemViewModel
                viewModel.KpiItems = pis.Select(pi => _mapper.Map<KpiListItemViewModel>(pi)).ToList();

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

                viewModel.ActivityType = pi.ActivityType;
                viewModel.PerformanceLevel = pi.PerformanceLevel.ToString();
                viewModel.IsPIKey = pi.IsKey;

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
        public async Task<IActionResult> Create(Guid? riId = null, Guid? kriId = null)
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

                if (riId.HasValue)
                {
                    // Get the parent RI information
                    var parentRi = await _unitOfWork.RIs.GetByIdAsync(riId.Value);
                    if (parentRi != null)
                    {
                        // Inherit some properties from the parent RI
                        viewModel.DepartmentId = !string.IsNullOrEmpty(parentRi.Department)
                            ? await GetDepartmentIdFromName(parentRi.Department) ?? Guid.Empty
                            : Guid.Empty;
                        viewModel.Unit = parentRi.Unit;
                        viewModel.Frequency = parentRi.Frequency;
                        viewModel.MeasurementDirection = parentRi.MeasurementDirection;

                        // Set the parent RI reference
                        viewModel.RIId = riId;

                        // Create dropdown for parent RIs with the current RI pre-selected
                        var riItems = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Value = parentRi.Id.ToString(),
                                Text = $"{parentRi.Code} - {parentRi.Name}",
                                Selected = true
                            }
                        };
                        ViewBag.RIs = new SelectList(riItems, "Value", "Text");
                    }
                    else
                    {
                        ViewBag.RIs = await GetRiSelectList();
                    }
                }
                else if (kriId.HasValue)
                {
                    // Get the parent KRI information
                    var parentKri = await _unitOfWork.KRIs.GetByIdAsync(kriId.Value);
                    if (parentKri != null)
                    {
                        // Inherit some properties from the parent KRI
                        viewModel.DepartmentId = !string.IsNullOrEmpty(parentKri.Department)
                            ? await GetDepartmentIdFromName(parentKri.Department) ?? Guid.Empty
                            : Guid.Empty;
                        viewModel.Unit = parentKri.Unit;
                        viewModel.Frequency = parentKri.Frequency;
                        viewModel.MeasurementDirection = parentKri.MeasurementDirection;

                        // Set the parent KRI reference
                        viewModel.KRIId = kriId;

                        // Create dropdown for parent KRIs with the current KRI pre-selected
                        var kriItems = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Value = parentKri.Id.ToString(),
                                Text = $"{parentKri.Code} - {parentKri.Name}",
                                Selected = true
                            }
                        };
                        ViewBag.KRIs = new SelectList(kriItems, "Value", "Text");
                    }
                }
                else
                {
                    ViewBag.RIs = await GetRiSelectList();
                }

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
                ViewBag.RIs = await GetRiSelectList();
                ViewBag.Users = new SelectList(new List<SelectListItem>());
                ViewBag.ActivityTypes = GetEnumSelectList<ActivityType>();
                return View("Create", viewModel);
            }

            try
            {
                // Đảm bảo Unit không rỗng
                if (string.IsNullOrEmpty(viewModel.Unit))
                {
                    // Nếu Unit trống, báo lỗi validation
                    ModelState.AddModelError("Unit", "Đơn vị đo là bắt buộc");
                    ViewBag.Departments = await GetDepartmentSelectList();
                    ViewBag.CSFs = await GetCsfSelectList();
                    ViewBag.RIs = await GetRiSelectList();
                    ViewBag.Users = new SelectList(new List<SelectListItem>());
                    ViewBag.ActivityTypes = GetEnumSelectList<ActivityType>();
                    return View("Create", viewModel);
                }

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
                    IsPIKey = viewModel.IsPIKey,
                    EffectiveDate = viewModel.EffectiveDate ?? DateTime.Now,
                    SelectedCsfIds = SelectedCsfs,
                    RIId = viewModel.RIId,
                    KRIId = viewModel.KRIId
                };

                // Create PI from view model
                var pi = _mapper.Map<PI>(createViewModel);

                // Set PI-specific properties if not mapped by AutoMapper
                if (viewModel.ActivityType.HasValue)
                {
                    pi.ActivityType = viewModel.ActivityType.Value;
                }

                // PerformanceLevel is a non-nullable int in the PI entity, but a string in the view model
                if (int.TryParse(viewModel.PerformanceLevel, out int performanceLevel))
                {
                    pi.PerformanceLevel = performanceLevel;
                }
                else
                {
                    pi.PerformanceLevel = 3; // Default value if parsing fails
                }

                // Set IsKey property based on the view model
                pi.IsKey = createViewModel.IsPIKey;

                // Set Department name from DepartmentId
                if (viewModel.DepartmentId != Guid.Empty)
                {
                    var department = await _unitOfWork.Departments.GetByIdAsync(viewModel.DepartmentId);
                    if (department != null)
                    {
                        pi.Department = department.Name;
                    }
                    else
                    {
                        // Nếu không tìm thấy department, sử dụng giá trị mặc định
                        pi.Department = !string.IsNullOrEmpty(viewModel.DepartmentName) ? viewModel.DepartmentName : "Unknown Department";
                        _logger.LogWarning("Department with ID {DepartmentId} not found. Using default value.", viewModel.DepartmentId);
                    }
                }

                // Set RIId if provided
                if (viewModel.RIId.HasValue)
                {
                    pi.RIId = viewModel.RIId.Value;
                }

                // Set KRIId if provided
                if (viewModel.KRIId.HasValue)
                {
                    pi.KRIId = viewModel.KRIId.Value;
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
                            KpiType = KpiType.PerformanceIndicator,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.GetUserId()
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // If this was created from an RI, redirect back to the RI details
                if (viewModel.RIId.HasValue)
                {
                    return RedirectToAction("Details", "Ri", new { id = viewModel.RIId.Value });
                }
                // If this was created from a KRI, redirect back to the KRI details
                else if (viewModel.KRIId.HasValue)
                {
                    return RedirectToAction("Details", "Kri", new { id = viewModel.KRIId.Value });
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating PI");
                ModelState.AddModelError("", "An error occurred while saving the PI. Please try again.");
                ViewBag.Departments = await GetDepartmentSelectList();
                ViewBag.CSFs = await GetCsfSelectList();
                ViewBag.RIs = await GetRiSelectList();
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
                viewModel.PerformanceLevel = pi.PerformanceLevel.ToString();
                viewModel.IsPIKey = pi.IsKey;

                // Đảm bảo Department và Owner được map đúng
                if (!string.IsNullOrEmpty(pi.Department))
                {
                    var departments = await _unitOfWork.Departments.GetAllAsync();
                    var department = departments.FirstOrDefault(d => d.Name == pi.Department);
                    if (department != null)
                    {
                        viewModel.DepartmentId = department.Id;
                        viewModel.Department = department.Name;
                    }
                }

                viewModel.Owner = pi.ResponsiblePerson;

                // Get selected CSF IDs
                var csfKpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                viewModel.SelectedCsfIds = csfKpis
                    .Where(ck => ck.KpiId == id)
                    .Select(ck => ck.CsfId)
                    .ToList();

                // Populate dropdown items
                await PopulateDropdownsWithSelection(viewModel);

                // Ensure MeasurementUnit is set
                if (string.IsNullOrEmpty(viewModel.MeasurementUnit) && !string.IsNullOrEmpty(pi.Unit))
                {
                    viewModel.MeasurementUnit = pi.Unit;
                }

                // Ensure MinimumValue and MaximumValue have defaults if not set
                if (viewModel.MinimumValue == 0 && pi.MinimumValue != 0)
                {
                    viewModel.MinimumValue = pi.MinimumValue;
                }

                if (viewModel.MaximumValue == 0 && pi.MaximumValue != 0)
                {
                    viewModel.MaximumValue = pi.MaximumValue;
                }
                else if (viewModel.MaximumValue == 0)
                {
                    // Default maximum value if none exists
                    viewModel.MaximumValue = 100;
                }

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

            // Đồng bộ Unit và MeasurementUnit trước khi validate
            if (!string.IsNullOrEmpty(viewModel.MeasurementUnit) && string.IsNullOrEmpty(viewModel.Unit))
            {
                viewModel.Unit = viewModel.MeasurementUnit;
                ModelState.SetModelValue("Unit", new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult(viewModel.Unit, CultureInfo.CurrentCulture));
            }
            else if (string.IsNullOrEmpty(viewModel.MeasurementUnit) && !string.IsNullOrEmpty(viewModel.Unit))
            {
                viewModel.MeasurementUnit = viewModel.Unit;
                ModelState.SetModelValue("MeasurementUnit", new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult(viewModel.MeasurementUnit, CultureInfo.CurrentCulture));
            }

            // Set default values for MinimumValue and MaximumValue if they're empty
            if (viewModel.MinimumValue == 0)
            {
                // Default minimum is 0, already set
            }

            if (viewModel.MaximumValue == 0)
            {
                viewModel.MaximumValue = 100; // Default maximum
                ModelState.SetModelValue("MaximumValue", new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult("100", CultureInfo.CurrentCulture));
            }

            // Ensure Category is set to a valid value
            if ((int)viewModel.Category == 0)
            {
                ModelState.AddModelError("Category", "Danh mục là bắt buộc");
            }

            // Log validation errors for debugging
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for Edit PI with ID: {PiId}", id);
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        _logger.LogWarning("Validation error for {Property}: {Errors}",
                            state.Key,
                            string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage)));
                    }
                }

                await PopulateDropdownsWithSelection(viewModel);
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

                try
                {
                    // Thay đổi cách thức mapping để tránh lỗi AutoMapper
                    // Map các thuộc tính cơ bản thủ công thay vì sử dụng AutoMapper
                    pi.Name = viewModel.Name;
                    pi.Code = viewModel.Code;
                    pi.Description = viewModel.Description;

                    // Gán giá trị đơn vị đo từ các trường đã đồng bộ
                    pi.Unit = !string.IsNullOrEmpty(viewModel.MeasurementUnit) ? viewModel.MeasurementUnit : viewModel.Unit;

                    pi.TargetValue = viewModel.TargetValue;
                    pi.MinimumValue = viewModel.MinimumValue;
                    pi.MaximumValue = viewModel.MaximumValue;
                    pi.Weight = viewModel.Weight;
                    pi.Frequency = viewModel.MeasurementFrequency;

                    // Lấy Department từ DepartmentId
                    if (viewModel.DepartmentId.HasValue && viewModel.DepartmentId.Value != Guid.Empty)
                    {
                        var department = await _unitOfWork.Departments.GetByIdAsync(viewModel.DepartmentId.Value);
                        if (department != null)
                        {
                            pi.Department = department.Name;
                        }
                        else
                        {
                            // Nếu không tìm thấy department, sử dụng giá trị từ viewModel hoặc giá trị mặc định
                            pi.Department = !string.IsNullOrEmpty(viewModel.Department) ? viewModel.Department : "Unknown Department";
                            _logger.LogWarning("Department with ID {DepartmentId} not found. Using value from viewModel or default.", viewModel.DepartmentId);
                        }
                    }
                    else
                    {
                        pi.Department = viewModel.Department ?? "Unknown Department";
                    }

                    pi.ResponsiblePerson = viewModel.Owner;
                    pi.MeasurementDirection = viewModel.MeasurementDirection;
                    pi.EffectiveDate = viewModel.EffectiveDate;
                    pi.Status = viewModel.Status;
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Error occurred during manual mapping of PI properties with ID: {PiId}", id);
                    ModelState.AddModelError("", "Error during property mapping: " + mapEx.Message);

                    await PopulateDropdownsWithSelection(viewModel);
                    return View("Edit", viewModel);
                }

                try
                {
                    // Set PI-specific properties if not mapped by AutoMapper
                    if (viewModel.ActivityType.HasValue)
                    {
                        pi.ActivityType = viewModel.ActivityType.Value;
                    }

                    // PerformanceLevel is a non-nullable int in the entity but a string in the view model
                    if (int.TryParse(viewModel.PerformanceLevel, out int performanceLevel))
                    {
                        pi.PerformanceLevel = performanceLevel;
                    }
                    else
                    {
                        pi.PerformanceLevel = 3; // Default value if parsing fails
                    }

                    // Set IndicatorType if provided
                    if (viewModel.IndicatorType.HasValue)
                    {
                        pi.IndicatorType = viewModel.IndicatorType.Value;
                    }

                    // Set IsKey property for PI 
                    pi.IsKey = viewModel.IsPIKey;

                    // Set RIId if provided
                    if (viewModel.RIId.HasValue)
                    {
                        pi.RIId = viewModel.RIId.Value;
                    }

                    // Set action plan if provided
                    if (!string.IsNullOrEmpty(viewModel.ActionPlan))
                    {
                        pi.ActionPlan = viewModel.ActionPlan;
                    }

                    // Set audit fields
                    pi.ModifiedAt = DateTime.UtcNow;
                    pi.ModifiedBy = User.GetUserId();
                }
                catch (Exception specialPropsEx)
                {
                    _logger.LogError(specialPropsEx, "Error occurred during mapping special properties of PI with ID: {PiId}", id);
                    ModelState.AddModelError("", "Error during special properties mapping: " + specialPropsEx.Message);

                    await PopulateDropdownsWithSelection(viewModel);
                    return View("Edit", viewModel);
                }

                try
                {
                    await _unitOfWork.PIs.UpdateAsync(pi);
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Error occurred during UpdateAsync for PI with ID: {PiId}", id);
                    ModelState.AddModelError("", "Error during UpdateAsync: " + updateEx.Message);

                    await PopulateDropdownsWithSelection(viewModel);
                    return View("Edit", viewModel);
                }

                try
                {
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
                                KpiType = KpiType.PerformanceIndicator,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.GetUserId()
                            });
                        }
                    }
                }
                catch (Exception csfEx)
                {
                    _logger.LogError(csfEx, "Error occurred during CSF links update for PI with ID: {PiId}", id);
                    ModelState.AddModelError("", "Error during CSF links update: " + csfEx.Message);

                    await PopulateDropdownsWithSelection(viewModel);
                    return View("Edit", viewModel);
                }

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception saveEx)
                {
                    _logger.LogError(saveEx, "Error occurred during SaveChangesAsync for PI with ID: {PiId}", id);
                    ModelState.AddModelError("", "Error during SaveChangesAsync: " + saveEx.Message);

                    await PopulateDropdownsWithSelection(viewModel);
                    return View("Edit", viewModel);
                }

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

                await PopulateDropdownsWithSelection(viewModel);
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

        // GET: PI/PromoteToKpi/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> PromoteToKpi(Guid id)
        {
            var pi = await _unitOfWork.PIs.GetByIdAsync(id);
            if (pi == null || !pi.IsActive)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<KpiDetailsViewModel>(pi);
            viewModel.KpiType = KpiType.PerformanceIndicator;

            // Get department name if available
            if (!string.IsNullOrEmpty(pi.Department))
            {
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var department = departments.FirstOrDefault(d => d.Name == pi.Department);
                if (department != null)
                {
                    viewModel.DepartmentName = department.Name;
                }
            }

            return View(viewModel);
        }

        // POST: PI/PromoteToKpi/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> PromoteToKpi(Guid id, KpiDetailsViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            try
            {
                var pi = await _unitOfWork.PIs.GetByIdAsync(id);
                if (pi == null || !pi.IsActive)
                {
                    return NotFound();
                }

                // Đánh dấu Performance Indicator là Key Performance Indicator
                pi.IsKey = true;
                pi.UpdatedAt = DateTime.UtcNow;
                pi.UpdatedBy = User.GetUserId();

                // Cập nhật PI
                _unitOfWork.PIs.Update(pi);
                await _unitOfWork.SaveChangesAsync();

                // Xóa bỏ cache để đảm bảo dữ liệu mới được hiển thị
                if (_memoryCache != null)
                {
                    string cacheKey = $"PI_{id}";
                    _memoryCache.Remove(cacheKey);
                    _memoryCache.Remove("AllKpis");
                    _memoryCache.Remove("AllPIs");
                }

                TempData["Success"] = "Chỉ số hoạt động đã được chuyển đổi thành KPI thành công!";

                // Redirect về trang KPI/Details để xem dưới dạng KPI
                return RedirectToAction("Details", "Kpi", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error promoting PI to KPI");
                TempData["Error"] = "Đã xảy ra lỗi khi chuyển đổi PI thành KPI.";
                return RedirectToAction(nameof(Details), new { id });
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
        /// Gets a select list for RIs
        /// </summary>
        /// <returns>SelectList of RIs</returns>
        private async Task<SelectList> GetRiSelectList()
        {
            var ris = await _unitOfWork.RIs.GetAllAsync();
            var items = ris.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = $"{r.Code} - {r.Name}"
            }).OrderBy(r => r.Text).ToList();

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

        /// <summary>
        /// Gets Department ID from Department name
        /// </summary>
        /// <param name="departmentName">Name of the department</param>
        /// <returns>Department ID or null if not found</returns>
        private async Task<Guid?> GetDepartmentIdFromName(string departmentName)
        {
            if (string.IsNullOrEmpty(departmentName))
                return null;

            var departments = await _unitOfWork.Departments.GetAllAsync();
            return departments.FirstOrDefault(d => d.Name == departmentName)?.Id;
        }

        /// <summary>
        /// Helper method to create a SelectList for departments with the currently selected department
        /// </summary>
        /// <param name="viewModel">The view model containing department information</param>
        /// <returns>Task completing with populated select lists</returns>
        private async Task PopulateDropdownsWithSelection(EditKpiViewModel viewModel)
        {
            // Create department list with selected item
            var departmentList = await GetDepartmentSelectList();
            if (viewModel.DepartmentId.HasValue)
            {
                var departmentItems = departmentList.Items.Cast<SelectListItem>().ToList();
                var selectedItem = departmentItems.FirstOrDefault(d => d.Value == viewModel.DepartmentId.Value.ToString());
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
                viewModel.Departments = new SelectList(departmentItems, "Value", "Text");
            }
            else
            {
                viewModel.Departments = departmentList;
            }

            // Create categories list with explicit enumeration
            var categories = Enum.GetValues(typeof(KpiCategory))
                .Cast<KpiCategory>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = GetCategoryDisplayName(c),
                    Selected = c == viewModel.Category
                })
                .ToList();

            // Ensure there's always a category selected
            if (viewModel.Category == 0)
            {
                viewModel.Category = KpiCategory.Financial; // Default to Financial if not set
            }

            viewModel.CriticalSuccessFactors = await GetCsfSelectList();
            viewModel.ActivityTypes = GetEnumSelectList<ActivityType>();
            viewModel.ParentRis = await GetRiSelectList();
            viewModel.IndicatorTypes = GetEnumSelectList<IndicatorType>();
        }

        /// <summary>
        /// Gets a friendly display name for KPI categories
        /// </summary>
        /// <param name="category">The KPI category</param>
        /// <returns>A user-friendly display name</returns>
        private string GetCategoryDisplayName(KpiCategory category)
        {
            return category switch
            {
                KpiCategory.Financial => "Tài chính",
                KpiCategory.Customer => "Khách hàng",
                KpiCategory.Operational => "Quy trình nội bộ",
                KpiCategory.LearningAndGrowth => "Học tập và phát triển",
                KpiCategory.Environmental => "Môi trường",
                KpiCategory.Social => "Xã hội",
                KpiCategory.Governance => "Quản trị",
                KpiCategory.Quality => "Chất lượng",
                KpiCategory.Innovation => "Đổi mới",
                KpiCategory.Productivity => "Năng suất",
                KpiCategory.HumanResources => "Nhân sự",
                KpiCategory.IT => "CNTT",
                KpiCategory.Safety => "An toàn",
                KpiCategory.Project => "Dự án",
                KpiCategory.Risk => "Rủi ro",
                KpiCategory.Other => "Khác",
                _ => category.ToString()
            };
        }
    }
}