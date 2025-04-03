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

                // Get total count for pagination
                viewModel.TotalCount = query.Count();

                // Apply pagination
                var ris = query
                            .OrderBy(r => r.Id)
                            .Skip((page - 1) * viewModel.PageSize)
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

                // Get related PIs that have this RI as their parent
                var allPIs = await _unitOfWork.PIs.GetAllAsync();
                var relatedPIs = allPIs
                    .Where(p => p.RIId == id)
                    .ToList();

                if (relatedPIs.Any())
                {
                    // Initialize collection for related PIs if not already initialized
                    viewModel.RelatedPIs ??= new List<LinkedKpiViewModel>();

                    // Map and add each related PI
                    foreach (var pi in relatedPIs)
                    {
                        var piViewModel = _mapper.Map<LinkedKpiViewModel>(pi);
                        viewModel.RelatedPIs.Add(piViewModel);
                    }
                }

                // Set RI-specific properties
                viewModel.ProcessArea = ri.ProcessArea;
                viewModel.ParentKriId = ri.ParentKriId;
                viewModel.IsRIKey = ri.IsKey;

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
        /// <param name="csfId">Optional CSF ID to link this RI with</param>
        /// <returns>Create RI view</returns>
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> Create(Guid? kriId = null, Guid? csfId = null)
        {
            try
            {
                var viewModel = new CreateKpiViewModel
                {
                    KpiType = KpiType.ResultIndicator,
                    Departments = await GetDepartmentSelectList(),
                    CriticalSuccessFactors = await GetCsfSelectList(),
                    ParentKris = await GetKriSelectList(),
                    ProcessAreas = GetEnumSelectList<ProcessArea>(),
                    RelatedPis = await GetPiSelectList()
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

                // Nếu csfId được truyền vào, tự động thêm vào danh sách CSF được chọn
                if (csfId.HasValue)
                {
                    // Kiểm tra sự tồn tại của CSF
                    var csf = await _unitOfWork.CriticalSuccessFactors.GetByIdAsync(csfId.Value);
                    if (csf != null)
                    {
                        // Khởi tạo danh sách nếu null
                        viewModel.SelectedCsfIds ??= new List<Guid>();
                        // Thêm csfId vào danh sách
                        viewModel.SelectedCsfIds.Add(csfId.Value);

                        // Có thể lấy thêm thông tin từ CSF để điền trước cho RI
                        if (!string.IsNullOrEmpty(csf.Department?.Name))
                        {
                            viewModel.Department = csf.Department.Name;
                        }

                        if (!string.IsNullOrEmpty(csf.Owner))
                        {
                            viewModel.Owner = csf.Owner;
                        }

                        // Sinh mã tự động dựa trên mã CSF
                        viewModel.Code = $"RI-{csf.Code}-{DateTime.Now.ToString("yyyyMMdd")}";
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
        public async Task<IActionResult> Create(CreateKpiViewModel viewModel, [FromForm] List<Guid> SelectedPis)
        {
            // Force KPI type to be RI
            viewModel.KpiType = KpiType.ResultIndicator;

            if (!ModelState.IsValid)
            {
                viewModel.Departments = await GetDepartmentSelectList();
                viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                viewModel.ParentKris = await GetKriSelectList();
                viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                viewModel.RelatedPis = await GetPiSelectList();
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

                // Set IsKey property to indicate whether this RI is a KRI
                ri.IsKey = viewModel.IsRIKey;

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
                            KpiType = KpiType.ResultIndicator,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.GetUserId()
                        });
                    }
                }

                // Update related PIs to link with this RI
                if (SelectedPis != null && SelectedPis.Any())
                {
                    var pis = await _unitOfWork.PIs.GetAllAsync();
                    var selectedPis = pis.Where(pi => SelectedPis.Contains(pi.Id)).ToList();

                    foreach (var pi in selectedPis)
                    {
                        pi.RIId = ri.Id;
                        pi.UpdatedAt = DateTime.UtcNow;
                        pi.UpdatedBy = User.GetUserId();
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
                viewModel.IsRIKey = ri.IsKey;

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

                try
                {
                    // Đảm bảo Unit và MeasurementUnit có giá trị đồng nhất
                    if (string.IsNullOrEmpty(viewModel.Unit) && !string.IsNullOrEmpty(viewModel.MeasurementUnit))
                    {
                        viewModel.Unit = viewModel.MeasurementUnit;
                    }
                    else if (!string.IsNullOrEmpty(viewModel.Unit) && string.IsNullOrEmpty(viewModel.MeasurementUnit))
                    {
                        viewModel.MeasurementUnit = viewModel.Unit;
                    }

                    // Thay đổi cách thức mapping để tránh lỗi AutoMapper
                    // Map các thuộc tính cơ bản thủ công thay vì sử dụng AutoMapper
                    ri.Name = viewModel.Name;
                    ri.Code = viewModel.Code;
                    ri.Description = viewModel.Description;
                    ri.Unit = !string.IsNullOrEmpty(viewModel.MeasurementUnit) ? viewModel.MeasurementUnit : viewModel.Unit;
                    ri.TargetValue = viewModel.TargetValue;
                    ri.MinimumValue = viewModel.MinimumValue;
                    ri.MaximumValue = viewModel.MaximumValue;
                    ri.Weight = viewModel.Weight;
                    ri.Frequency = viewModel.MeasurementFrequency;
                    ri.Department = viewModel.Department;
                    ri.ResponsiblePerson = viewModel.Owner;
                    ri.MeasurementDirection = viewModel.MeasurementDirection;
                    ri.EffectiveDate = viewModel.EffectiveDate;
                    ri.Status = viewModel.Status;
                }
                catch (Exception mapEx)
                {
                    _logger.LogError(mapEx, "Error occurred during manual mapping of RI properties with ID: {RiId}", id);
                    ModelState.AddModelError("", "Error during property mapping: " + mapEx.Message);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                    viewModel.ParentKris = await GetKriSelectList();
                    viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                    return View("Edit", viewModel);
                }

                try
                {
                    // Cập nhật các thuộc tính đặc thù của RI
                    if (viewModel.ProcessArea.HasValue)
                    {
                        ri.ProcessArea = viewModel.ProcessArea.Value;
                    }

                    if (viewModel.ParentKriId.HasValue)
                    {
                        ri.ParentKriId = viewModel.ParentKriId;
                    }

                    // Các thuộc tính bổ sung của RI
                    ri.ResponsibleManager = viewModel.ResponsibleManager;
                    ri.MeasurementScope = viewModel.MeasurementScope;
                    ri.TimeFrame = viewModel.TimeFrame;
                    ri.ResultType = viewModel.ResultType;
                    ri.ContributionPercentage = viewModel.ContributionPercentage;
                    ri.DataSource = viewModel.DataSource;
                    ri.Formula = viewModel.CalculationMethod ?? viewModel.Formula;

                    // Set IsKey property to indicate whether this RI is a KRI
                    ri.IsKey = viewModel.IsRIKey;

                    // Set audit fields
                    ri.ModifiedAt = DateTime.UtcNow;
                    ri.ModifiedBy = User.GetUserId();
                }
                catch (Exception specialPropsEx)
                {
                    _logger.LogError(specialPropsEx, "Error occurred during mapping special properties of RI with ID: {RiId}", id);
                    ModelState.AddModelError("", "Error during special properties mapping: " + specialPropsEx.Message);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                    viewModel.ParentKris = await GetKriSelectList();
                    viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                    return View("Edit", viewModel);
                }

                try
                {
                    await _unitOfWork.RIs.UpdateAsync(ri);
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Error occurred during UpdateAsync for RI with ID: {RiId}", id);
                    ModelState.AddModelError("", "Error during UpdateAsync: " + updateEx.Message);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                    viewModel.ParentKris = await GetKriSelectList();
                    viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
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
                                KpiId = ri.Id,
                                KpiType = KpiType.ResultIndicator,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.GetUserId()
                            });
                        }
                    }
                }
                catch (Exception csfEx)
                {
                    _logger.LogError(csfEx, "Error occurred during CSF links update for RI with ID: {RiId}", id);
                    ModelState.AddModelError("", "Error during CSF links update: " + csfEx.Message);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                    viewModel.ParentKris = await GetKriSelectList();
                    viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                    return View("Edit", viewModel);
                }

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception saveEx)
                {
                    _logger.LogError(saveEx, "Error occurred during SaveChangesAsync for RI with ID: {RiId}", id);
                    ModelState.AddModelError("", "Error during SaveChangesAsync: " + saveEx.Message);
                    viewModel.Departments = await GetDepartmentSelectList();
                    viewModel.CriticalSuccessFactors = await GetCsfSelectList();
                    viewModel.ParentKris = await GetKriSelectList();
                    viewModel.ProcessAreas = GetEnumSelectList<ProcessArea>();
                    return View("Edit", viewModel);
                }

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

        // GET: RI/PromoteToKri/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> PromoteToKri(Guid id)
        {
            var ri = await _unitOfWork.RIs.GetByIdAsync(id);
            if (ri == null || !ri.IsActive)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<KpiDetailsViewModel>(ri);
            viewModel.KpiType = KpiType.ResultIndicator;

            // Get department name if available
            if (!string.IsNullOrEmpty(ri.Department))
            {
                var departments = await _unitOfWork.Departments.GetAllAsync();
                var department = departments.FirstOrDefault(d => d.Name == ri.Department);
                if (department != null)
                {
                    viewModel.DepartmentName = department.Name;
                }
            }

            return View(viewModel);
        }

        // POST: RI/PromoteToKri/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
        public async Task<IActionResult> PromoteToKri(Guid id, KpiDetailsViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            try
            {
                // Lấy thông tin RI
                var ri = await _unitOfWork.RIs.GetByIdAsync(id);
                if (ri == null || !ri.IsActive)
                {
                    return NotFound();
                }

                // Tạo mới một KRI từ RI hiện tại
                var kri = new KRI
                {
                    Id = Guid.NewGuid(),
                    Name = ri.Name,
                    Description = ri.Description,
                    Code = "KRI-" + ri.Code.Replace("RI-", ""),
                    Unit = ri.Unit,
                    Formula = ri.Formula,
                    TargetValue = ri.TargetValue,
                    MinimumValue = ri.MinimumValue,
                    MaximumValue = ri.MaximumValue,
                    Weight = ri.Weight,
                    Frequency = ri.Frequency,
                    Department = ri.Department,
                    ResponsiblePerson = ri.ResponsiblePerson,
                    EffectiveDate = ri.EffectiveDate,
                    Status = ri.Status,
                    MeasurementDirection = ri.MeasurementDirection,
                    PerformanceTrend = ri.PerformanceTrend,
                    // KRI-specific properties
                    StrategicObjective = "Chuyển đổi từ RI",
                    BusinessArea = BusinessArea.Other,
                    ImpactLevel = ImpactLevel.Medium,
                    ConfidenceLevel = 50, // Trung bình
                    ExecutiveOwner = ri.ResponsiblePerson,
                    // Audit fields
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.GetUserId(),
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = User.GetUserId()
                };

                // Thêm KRI mới
                await _unitOfWork.KRIs.AddAsync(kri);

                // Đánh dấu RI đã có KRI và không hoạt động
                ri.IsActive = false;
                ri.UpdatedAt = DateTime.UtcNow;
                ri.UpdatedBy = User.GetUserId();
                _unitOfWork.RIs.Update(ri);

                // Chuyển đổi các CSF-KPI liên quan nếu có
                var csfkpis = await _unitOfWork.CSFKPIs.GetAllAsync();
                var riCsfKpis = csfkpis.Where(c => c.KpiId == ri.Id).ToList();

                foreach (var csfkpi in riCsfKpis)
                {
                    await _unitOfWork.CSFKPIs.AddAsync(new Models.Entities.CSF.CSFKPI
                    {
                        CsfId = csfkpi.CsfId,
                        KpiId = kri.Id,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.GetUserId()
                    });
                }

                // Lưu thay đổi
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Chỉ số kết quả đã được chuyển đổi thành KRI thành công!";
                return RedirectToAction("Details", "KRI", new { id = kri.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error promoting RI to KRI");
                TempData["Error"] = "Đã xảy ra lỗi khi chuyển đổi RI thành KRI.";
                return RedirectToAction(nameof(Details), new { id });
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
                    Value = d.Name,
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
        /// Gets a select list of PIs for dropdowns
        /// </summary>
        /// <returns>List of SelectListItem for PIs</returns>
        private async Task<SelectList> GetPiSelectList()
        {
            var pis = await _unitOfWork.PIs.GetAllAsync();
            var items = pis
                .OrderBy(p => p.Name)
                .Where(p => !p.RIId.HasValue) // Only include PIs not already linked to an RI
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Code} - {p.Name}"
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