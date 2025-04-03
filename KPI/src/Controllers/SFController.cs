using AutoMapper;
using KPISolution.Authorization;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.Entities.Objective;
using KPISolution.Models.Entities.CSF;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.SuccessFactor;
using KPISolution.Models.ViewModels.CSF;
using KPISolution.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace KPISolution.Controllers
{
    [Authorize]
    public class SFController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SFController> _logger;
        private readonly IMapper _mapper;

        public SFController(
            IUnitOfWork unitOfWork,
            ILogger<SFController> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: SF
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var successFactors = await _unitOfWork.SuccessFactors.GetAllAsync();
                var activeFactors = successFactors.Where(sf => sf.IsActive).ToList();

                return View(activeFactors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Success Factors");
                TempData["Error"] = "Đã xảy ra lỗi khi tải danh sách yếu tố thành công.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: SF/Details/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null || !successFactor.IsActive)
                {
                    return NotFound();
                }

                // Lấy danh sách CSF liên quan
                var allCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                var relatedCsfs = allCsfs
                    .Where(csf => csf.SuccessFactorId == id && csf.IsActive)
                    .ToList();

                var viewModel = new SuccessFactorDetailsViewModel
                {
                    Id = successFactor.Id,
                    Name = successFactor.Name,
                    Description = successFactor.Description,
                    Code = successFactor.Code,
                    Priority = successFactor.Priority,
                    Status = successFactor.Status,
                    ProgressPercentage = successFactor.ProgressPercentage,
                    StartDate = successFactor.StartDate,
                    TargetDate = successFactor.TargetDate,
                    DepartmentId = successFactor.DepartmentId,
                    Owner = successFactor.Owner,
                    Notes = successFactor.Notes,
                    BusinessObjectiveId = successFactor.BusinessObjectiveId
                };

                // Lấy thông tin BusinessObjective
                if (successFactor.BusinessObjectiveId != Guid.Empty)
                {
                    var businessObjective = await _unitOfWork.BusinessObjectives.GetByIdAsync(successFactor.BusinessObjectiveId);
                    if (businessObjective != null && businessObjective.IsActive)
                    {
                        viewModel.BusinessObjectiveName = businessObjective.Name;
                    }
                }

                // Lấy thông tin Department
                if (successFactor.DepartmentId.HasValue)
                {
                    var department = await _unitOfWork.Departments.GetByIdAsync(successFactor.DepartmentId.Value);
                    if (department != null && department.IsActive)
                    {
                        viewModel.DepartmentName = department.Name;
                    }
                }

                // Map Critical Success Factors
                viewModel.CriticalSuccessFactors = relatedCsfs.Select(csf => new CSFListItemViewModel
                {
                    Id = csf.Id,
                    Name = csf.Name,
                    Code = csf.Code,
                    Status = csf.Status,
                    ProgressPercentage = csf.ProgressPercentage,
                    SuccessFactorId = csf.SuccessFactorId
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Success Factor details");
                TempData["Error"] = "Đã xảy ra lỗi khi tải chi tiết yếu tố thành công.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: SF/Create
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Create(Guid? businessObjectiveId = null)
        {
            var viewModel = new SuccessFactorEditViewModel
            {
                BusinessObjectiveId = businessObjectiveId ?? Guid.Empty,
                BusinessObjectives = await GetBusinessObjectiveSelectList(businessObjectiveId),
                Departments = await GetDepartmentSelectList()
            };

            return View(viewModel);
        }

        // POST: SF/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Create(SuccessFactorEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var successFactor = new SuccessFactor
                    {
                        Id = Guid.NewGuid(),
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        Code = viewModel.Code,
                        Priority = viewModel.Priority,
                        Status = viewModel.Status,
                        IsCSF = viewModel.IsCSF,
                        ProgressPercentage = viewModel.ProgressPercentage,
                        StartDate = viewModel.StartDate,
                        TargetDate = viewModel.TargetDate,
                        DepartmentId = viewModel.DepartmentId,
                        BusinessObjectiveId = viewModel.BusinessObjectiveId,
                        Owner = viewModel.Owner,
                        Notes = viewModel.Notes,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? "System",
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = User.Identity?.Name ?? "System"
                    };

                    await _unitOfWork.SuccessFactors.AddAsync(successFactor);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Yếu tố thành công đã được tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Success Factor");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo yếu tố thành công.");
                }
            }

            // If we got to here, something failed; redisplay form
            viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
            viewModel.Departments = await GetDepartmentSelectList(viewModel.DepartmentId);
            return View(viewModel);
        }

        // GET: SF/Edit/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
            if (successFactor == null || !successFactor.IsActive)
            {
                return NotFound();
            }

            var viewModel = new SuccessFactorEditViewModel
            {
                Id = successFactor.Id,
                Name = successFactor.Name,
                Description = successFactor.Description,
                Code = successFactor.Code,
                Priority = successFactor.Priority,
                Status = successFactor.Status,
                IsCSF = successFactor.IsCSF,
                ProgressPercentage = successFactor.ProgressPercentage,
                StartDate = successFactor.StartDate,
                TargetDate = successFactor.TargetDate,
                DepartmentId = successFactor.DepartmentId,
                BusinessObjectiveId = successFactor.BusinessObjectiveId,
                Owner = successFactor.Owner,
                Notes = successFactor.Notes,
                BusinessObjectives = await GetBusinessObjectiveSelectList(successFactor.BusinessObjectiveId),
                Departments = await GetDepartmentSelectList(successFactor.DepartmentId)
            };

            return View(viewModel);
        }

        // POST: SF/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Edit(Guid id, SuccessFactorEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
                    if (successFactor == null || !successFactor.IsActive)
                    {
                        return NotFound();
                    }

                    // Update entity from view model
                    successFactor.Name = viewModel.Name;
                    successFactor.Description = viewModel.Description;
                    successFactor.Code = viewModel.Code;
                    successFactor.Priority = viewModel.Priority;
                    successFactor.Status = viewModel.Status;
                    successFactor.IsCSF = viewModel.IsCSF;
                    successFactor.ProgressPercentage = viewModel.ProgressPercentage;
                    successFactor.StartDate = viewModel.StartDate;
                    successFactor.TargetDate = viewModel.TargetDate;
                    successFactor.DepartmentId = viewModel.DepartmentId;
                    successFactor.BusinessObjectiveId = viewModel.BusinessObjectiveId;
                    successFactor.Owner = viewModel.Owner;
                    successFactor.Notes = viewModel.Notes;
                    successFactor.UpdatedAt = DateTime.UtcNow;
                    successFactor.UpdatedBy = User.Identity?.Name ?? "System";

                    _unitOfWork.SuccessFactors.Update(successFactor);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Yếu tố thành công đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Details), new { id = successFactor.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating Success Factor");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật yếu tố thành công.");
                }
            }

            // If we got to here, something failed; redisplay form
            viewModel.BusinessObjectives = await GetBusinessObjectiveSelectList(viewModel.BusinessObjectiveId);
            viewModel.Departments = await GetDepartmentSelectList(viewModel.DepartmentId);
            return View(viewModel);
        }

        // GET: SF/Delete/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
            if (successFactor == null || !successFactor.IsActive)
            {
                return NotFound();
            }

            // Kiểm tra xem có CSF nào tham chiếu đến không
            var allCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
            var relatedCsfs = allCsfs.Where(csf => csf.SuccessFactorId == id && csf.IsActive).ToList();

            if (relatedCsfs.Any())
            {
                // Thêm thông tin về CSF vào ViewBag để hiển thị trong view
                ViewBag.RelatedCsfs = relatedCsfs;
                ViewBag.CannotDelete = true;
                ViewBag.RelatedCsfCount = relatedCsfs.Count;

                // Lấy thông tin chi tiết hơn về CSF đầu tiên để hướng dẫn
                if (relatedCsfs.Count > 0)
                {
                    var firstCsf = relatedCsfs.First();
                    ViewBag.FirstCsfId = firstCsf.Id;
                    ViewBag.FirstCsfName = firstCsf.Name;
                    ViewBag.FirstCsfCode = firstCsf.Code;
                }
            }

            return View(successFactor);
        }

        // POST: SF/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null)
                {
                    return NotFound();
                }

                // Kiểm tra xem có CSF nào tham chiếu đến không
                var allCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
                var relatedCsfs = allCsfs.Where(csf => csf.SuccessFactorId == id && csf.IsActive).ToList();

                if (relatedCsfs.Any())
                {
                    var csfNames = string.Join(", ", relatedCsfs.Take(3).Select(c => c.Code));
                    if (relatedCsfs.Count > 3)
                    {
                        csfNames += $" và {relatedCsfs.Count - 3} CSF khác";
                    }

                    TempData["Error"] = $"Không thể xóa yếu tố thành công này vì có {relatedCsfs.Count} CSF đang tham chiếu đến nó: {csfNames}. Vui lòng chỉnh sửa các CSF này trước.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Thực hiện soft delete
                successFactor.IsActive = false;
                successFactor.UpdatedAt = DateTime.UtcNow;
                successFactor.UpdatedBy = User.Identity?.Name ?? "System";

                _unitOfWork.SuccessFactors.Update(successFactor);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Yếu tố thành công đã được xóa thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Success Factor");
                TempData["Error"] = "Đã xảy ra lỗi khi xóa yếu tố thành công.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: SF/PromoteToCsf/5
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> PromoteToCsf(Guid id)
        {
            var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
            if (successFactor == null || !successFactor.IsActive)
            {
                return NotFound();
            }

            return View(new SuccessFactorDetailsViewModel
            {
                Id = successFactor.Id,
                Name = successFactor.Name,
                Description = successFactor.Description,
                Code = successFactor.Code,
                BusinessObjectiveId = successFactor.BusinessObjectiveId,
                DepartmentId = successFactor.DepartmentId,
                Owner = successFactor.Owner,
                Priority = successFactor.Priority,
                Status = successFactor.Status,
                ProgressPercentage = successFactor.ProgressPercentage,
                StartDate = successFactor.StartDate,
                TargetDate = successFactor.TargetDate,
                Notes = successFactor.Notes
            });
        }

        // POST: SF/PromoteToCsf/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
        public async Task<IActionResult> PromoteToCsf(Guid id, SuccessFactorDetailsViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            try
            {
                var successFactor = await _unitOfWork.SuccessFactors.GetByIdAsync(id);
                if (successFactor == null || !successFactor.IsActive)
                {
                    return NotFound();
                }

                // Kiểm tra xem SF này đã được chuyển thành CSF chưa
                var existingCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync(
                    csf => csf.SuccessFactorId == id && csf.IsActive);

                if (existingCsfs.Any())
                {
                    // SF đã được chuyển thành CSF trước đó
                    var existingCsf = existingCsfs.First();
                    TempData["Info"] = "Yếu tố thành công này đã được chuyển đổi thành CSF trước đó.";
                    return RedirectToAction("Details", "CSF", new { id = existingCsf.Id });
                }

                // Tạo mới một Critical Success Factor từ Success Factor hiện tại
                var criticalSuccessFactor = new CriticalSuccessFactor
                {
                    Id = Guid.NewGuid(),
                    Name = successFactor.Name,
                    Description = successFactor.Description,
                    Code = "CSF-" + successFactor.Code,
                    SuccessFactorId = successFactor.Id,
                    BusinessObjectiveId = successFactor.BusinessObjectiveId,
                    DepartmentId = successFactor.DepartmentId,
                    Owner = successFactor.Owner,
                    Priority = successFactor.Priority,
                    Status = (CSFStatus)(int)successFactor.Status, // Convert from ObjectiveStatus to CSFStatus
                    ProgressPercentage = successFactor.ProgressPercentage,
                    StartDate = successFactor.StartDate,
                    TargetDate = successFactor.TargetDate,
                    Notes = successFactor.Notes,
                    Category = CSFCategory.Other,
                    RiskLevel = RiskLevel.Medium,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = User.Identity?.Name ?? "System"
                };

                // Đánh dấu Success Factor hiện tại là CSF
                successFactor.IsCSF = true;
                successFactor.UpdatedAt = DateTime.UtcNow;
                successFactor.UpdatedBy = User.Identity?.Name ?? "System";

                // Lưu thay đổi vào database
                await _unitOfWork.CriticalSuccessFactors.AddAsync(criticalSuccessFactor);
                _unitOfWork.SuccessFactors.Update(successFactor);
                await _unitOfWork.SaveChangesAsync();

                TempData["Success"] = "Yếu tố thành công đã được chuyển đổi thành Yếu tố thành công then chốt!";
                return RedirectToAction("Details", "CSF", new { id = criticalSuccessFactor.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error promoting Success Factor to Critical Success Factor");
                TempData["Error"] = "Đã xảy ra lỗi khi chuyển đổi SF thành CSF.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        private async Task<SelectList> GetBusinessObjectiveSelectList(Guid? selectedId = null)
        {
            var allObjectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var objectives = allObjectives.Where(d => d.IsActive).ToList();
            return new SelectList(objectives.OrderBy(d => d.Name), "Id", "Name", selectedId);
        }

        private async Task<SelectList> GetDepartmentSelectList(Guid? selectedId = null)
        {
            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            var departments = allDepartments.Where(d => d.IsActive).ToList();
            return new SelectList(departments.OrderBy(d => d.Name), "Id", "Name", selectedId);
        }
    }
}