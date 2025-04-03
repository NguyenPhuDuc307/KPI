using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KPISolution.Data;
using KPISolution.Data.Repositories.Interfaces;
using KPISolution.Models.Entities.Organization;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.BusinessObjective;

namespace KPISolution.Controllers
{
    [Authorize]
    public class BusinessObjectiveController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusinessObjectiveController> _logger;
        private readonly ApplicationDbContext _context;

        public BusinessObjectiveController(IUnitOfWork unitOfWork, ILogger<BusinessObjectiveController> logger, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _context = context;
        }

        // GET: BusinessObjective
        public async Task<IActionResult> Index(string searchTerm, BusinessPerspective? filterPerspective, ObjectiveStatus? filterStatus, PriorityLevel? filterPriority, TimeframeType? filterTimeframe, string sortBy = "Name", string sortDirection = "asc")
        {
            var objectives = await _context.BusinessObjectives
                .Include(x => x.Department)
                .Where(o => o.IsActive)
                .ToListAsync();

            // Apply filters
            var filteredObjectives = objectives.AsEnumerable();

            // Apply search term filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredObjectives = filteredObjectives.Where(o =>
                    o.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    o.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Apply perspective filter
            if (filterPerspective.HasValue)
            {
                filteredObjectives = filteredObjectives.Where(o => o.BusinessPerspective == filterPerspective.Value);
            }

            // Apply status filter
            if (filterStatus.HasValue)
            {
                filteredObjectives = filteredObjectives.Where(o => o.Status == filterStatus.Value);
            }

            // Apply priority filter
            if (filterPriority.HasValue)
            {
                filteredObjectives = filteredObjectives.Where(o => o.Priority == filterPriority.Value);
            }

            // Apply timeframe filter
            if (filterTimeframe.HasValue)
            {
                filteredObjectives = filteredObjectives.Where(o => o.Timeframe == filterTimeframe.Value);
            }

            // Apply sorting
            filteredObjectives = ApplySorting(filteredObjectives, sortBy, sortDirection);

            var viewModel = new BusinessObjectiveListViewModel
            {
                Objectives = filteredObjectives.Select(o => new BusinessObjectiveListItemViewModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    BusinessPerspective = o.BusinessPerspective,
                    Priority = o.Priority,
                    Status = o.Status,
                    ProgressPercentage = o.ProgressPercentage,
                    StartDate = o.StartDate,
                    TargetDate = o.TargetDate,
                    Department = o.Department != null ? o.Department.Name : string.Empty,
                    TimeframeType = o.Timeframe
                }).ToList(),
                SearchTerm = searchTerm,
                FilterPerspective = filterPerspective,
                FilterStatus = filterStatus,
                FilterPriority = filterPriority,
                FilterTimeframe = filterTimeframe,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            return View(viewModel);
        }

        private IEnumerable<BusinessObjective> ApplySorting(IEnumerable<BusinessObjective> objectives, string sortBy, string sortDirection)
        {
            // Default sorting
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = "Name";
            }

            var isAscending = string.IsNullOrEmpty(sortDirection) || sortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase);

            switch (sortBy.ToLower())
            {
                case "name":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.Name);
                    break;
                case "perspective":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.BusinessPerspective).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.BusinessPerspective).ThenBy(o => o.Name);
                    break;
                case "status":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.Status).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.Status).ThenBy(o => o.Name);
                    break;
                case "priority":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.Priority).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.Priority).ThenBy(o => o.Name);
                    break;
                case "progress":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.ProgressPercentage).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.ProgressPercentage).ThenBy(o => o.Name);
                    break;
                case "timeframe":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.Timeframe).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.Timeframe).ThenBy(o => o.Name);
                    break;
                case "targetdate":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.TargetDate).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.TargetDate).ThenBy(o => o.Name);
                    break;
                case "department":
                    objectives = isAscending
                        ? objectives.OrderBy(o => o.Department != null ? o.Department.Name : string.Empty).ThenBy(o => o.Name)
                        : objectives.OrderByDescending(o => o.Department != null ? o.Department.Name : string.Empty).ThenBy(o => o.Name);
                    break;
            }

            return objectives;
        }

        // GET: BusinessObjective/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var objective = await _context.BusinessObjectives
                    .Include(x => x.Department)
                    .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);

                if (objective == null)
                {
                    return NotFound();
                }

                // Lấy danh sách các CSF liên quan đến mục tiêu
                var relatedCsfs = await _context.CriticalSuccessFactors
                    .Where(csf => csf.BusinessObjectiveId == id && csf.IsActive)
                    .ToListAsync();

                // Lấy danh sách các Yếu tố thành công (SF) liên quan
                var relatedSfs = await _context.SuccessFactors
                    .Where(sf => sf.BusinessObjectiveId == id && sf.IsActive)
                    .ToListAsync();

                // Lấy danh sách các chỉ số KPI, KRI, RI, PI liên quan - nếu interface KPIs chưa được triển khai, cần bổ sung
                var relatedKpis = new List<dynamic>(); // Sẽ thay bằng danh sách KPI thực tế khi có

                var parentId = objective.ParentObjectiveId;
                BusinessObjective? parentObjective = null;
                if (parentId.HasValue)
                {
                    parentObjective = await _context.BusinessObjectives
                        .Include(x => x.Department)
                        .FirstOrDefaultAsync(x => x.Id == parentId.Value && x.IsActive);
                }

                // Lấy danh sách các mục tiêu con
                var childObjectives = await _context.BusinessObjectives
                    .Include(x => x.Department)
                    .Where(o => o.ParentObjectiveId == id && o.IsActive)
                    .ToListAsync();

                var viewModel = new BusinessObjectiveDetailsViewModel
                {
                    Id = objective.Id,
                    Name = objective.Name,
                    Description = objective.Description,
                    BusinessPerspective = objective.BusinessPerspective,
                    BusinessPerspectiveDisplayText = GetBusinessPerspectiveText(objective.BusinessPerspective),
                    Department = objective.Department?.Name ?? "-",
                    FiscalYear = objective.FiscalYear ?? "-",
                    TimeframeType = objective.Timeframe,
                    StartDate = objective.StartDate,
                    TargetDate = objective.TargetDate,
                    CompletionDate = objective.CompletionDate,
                    Budget = objective.Budget,
                    Status = objective.Status,
                    Priority = objective.Priority,
                    ProgressPercentage = objective.ProgressPercentage,
                    Notes = objective.Notes,
                    ParentObjective = parentObjective != null ? new BusinessObjectiveSimpleViewModel
                    {
                        Id = parentObjective.Id,
                        Name = parentObjective.Name,
                        Status = parentObjective.Status,
                        StatusBadgeClass = GetStatusBadgeClass(parentObjective.Status),
                        ProgressPercentage = parentObjective.ProgressPercentage,
                        TargetDate = parentObjective.TargetDate
                    } : null,
                    ChildObjectives = childObjectives.Select(child => new BusinessObjectiveSimpleViewModel
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Status = child.Status,
                        StatusBadgeClass = GetStatusBadgeClass(child.Status),
                        ProgressPercentage = child.ProgressPercentage,
                        TargetDate = child.TargetDate
                    }).ToList(),
                    RelatedCSFs = relatedCsfs.Select(csf => new CSFSimpleViewModel
                    {
                        Id = csf.Id,
                        Code = csf.Code,
                        Name = csf.Name,
                        Status = csf.Status,
                        StatusBadgeClass = GetCsfStatusBadgeClass(csf.Status),
                        ProgressPercentage = csf.ProgressPercentage
                    }).ToList(),
                    // Thêm các SF liên quan
                    RelatedSFs = relatedSfs.Select(sf => new SFSimpleViewModel
                    {
                        Id = sf.Id,
                        Code = sf.Code,
                        Name = sf.Name,
                        Status = sf.Status,
                        StatusBadgeClass = GetStatusBadgeClass(sf.Status),
                        ProgressPercentage = sf.ProgressPercentage
                    }).ToList(),
                    // Tạm khởi tạo danh sách rỗng cho KPI, KRI, RI, PI
                    RelatedKPIs = new List<KPISimpleViewModel>(),
                    RelatedKRIs = new List<KPISimpleViewModel>(),
                    RelatedRIs = new List<KPISimpleViewModel>(),
                    RelatedPIs = new List<KPISimpleViewModel>()
                };

                // Tính toán giá trị cho các badge và display text
                viewModel.StatusBadgeClass = GetStatusBadgeClass(objective.Status);
                viewModel.PriorityBadgeClass = GetPriorityBadgeClass(objective.Priority);
                viewModel.ProgressBarClass = GetProgressBarClass(objective.ProgressPercentage);
                viewModel.TimeframeDisplayText = GetTimeframeDisplayText(objective.Timeframe);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business objective details for ID: {Id}", id);
                TempData["Error"] = "Đã xảy ra lỗi khi tải thông tin chi tiết mục tiêu.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string GetCsfStatusBadgeClass(CSFStatus status)
        {
            return status switch
            {
                CSFStatus.NotStarted => "badge bg-secondary",
                CSFStatus.InProgress => "badge bg-primary",
                CSFStatus.AtRisk => "badge bg-warning text-dark",
                CSFStatus.Delayed => "badge bg-danger",
                CSFStatus.Completed => "badge bg-success",
                CSFStatus.Cancelled => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }

        // Hỗ trợ cho KPI status
        private string GetKpiStatusBadgeClass(object status)
        {
            return "badge bg-secondary"; // Default badge cho KPI, sẽ triển khai chi tiết sau khi enum KpiStatus được định nghĩa
        }

        private string GetStatusBadgeClass(ObjectiveStatus status)
        {
            return status switch
            {
                ObjectiveStatus.NotStarted => "badge bg-secondary",
                ObjectiveStatus.InProgress => "badge bg-primary",
                ObjectiveStatus.OnHold => "badge bg-warning text-dark",
                ObjectiveStatus.Completed => "badge bg-success",
                ObjectiveStatus.Canceled => "badge bg-danger",
                ObjectiveStatus.Delayed => "badge bg-info text-dark",
                _ => "badge bg-secondary"
            };
        }

        private string GetPriorityBadgeClass(PriorityLevel priority)
        {
            return priority switch
            {
                PriorityLevel.Low => "badge bg-success",
                PriorityLevel.Medium => "badge bg-warning text-dark",
                PriorityLevel.High => "badge bg-danger",
                PriorityLevel.Critical => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }

        private string GetProgressBarClass(int progressPercentage)
        {
            return progressPercentage switch
            {
                100 => "progress-bar bg-success",
                >= 75 => "progress-bar bg-info",
                >= 50 => "progress-bar bg-primary",
                >= 25 => "progress-bar bg-warning",
                _ => "progress-bar bg-danger"
            };
        }

        private string GetTimeframeDisplayText(TimeframeType timeframe)
        {
            return timeframe switch
            {
                TimeframeType.ShortTerm => "Ngắn hạn",
                TimeframeType.MediumTerm => "Trung hạn",
                TimeframeType.LongTerm => "Dài hạn",
                _ => "Không xác định"
            };
        }

        private string GetBusinessPerspectiveText(BusinessPerspective perspective)
        {
            return perspective switch
            {
                BusinessPerspective.Financial => "Tài chính",
                BusinessPerspective.Customer => "Khách hàng",
                BusinessPerspective.InternalProcess => "Quy trình nội bộ",
                BusinessPerspective.LearningGrowth => "Học hỏi và phát triển",
                _ => "Không xác định"
            };
        }

        // GET: BusinessObjective/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new BusinessObjectiveEditViewModel
            {
                StartDate = DateTime.Today,
                TargetDate = DateTime.Today.AddMonths(3),
                Departments = await GetDepartmentSelectList(),
                ParentObjectives = await GetParentObjectiveSelectList()
            };

            return View(viewModel);
        }

        // POST: BusinessObjective/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BusinessObjectiveEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var objective = new BusinessObjective
                    {
                        Id = Guid.NewGuid(),
                        Name = viewModel.Name,
                        Description = viewModel.Description,
                        BusinessPerspective = viewModel.BusinessPerspective,
                        Priority = viewModel.Priority,
                        Status = viewModel.Status,
                        ProgressPercentage = viewModel.ProgressPercentage,
                        StartDate = viewModel.StartDate,
                        TargetDate = viewModel.TargetDate,
                        CompletionDate = viewModel.CompletionDate,
                        DepartmentId = viewModel.DepartmentId,
                        ParentObjectiveId = viewModel.ParentObjectiveId,
                        Budget = viewModel.Budget,
                        Notes = viewModel.Notes,
                        FiscalYear = viewModel.FiscalYear,
                        Timeframe = viewModel.Timeframe,
                        ResponsiblePersonId = viewModel.ResponsiblePersonId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = User.Identity?.Name ?? "System",
                        UpdatedAt = DateTime.UtcNow,
                        UpdatedBy = User.Identity?.Name ?? "System"
                    };

                    await _unitOfWork.BusinessObjectives.AddAsync(objective);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Mục tiêu kinh doanh đã được tạo thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating business objective");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo mục tiêu kinh doanh.");
                }
            }

            // If we got to here, something failed; redisplay form
            viewModel.Departments = await GetDepartmentSelectList(viewModel.DepartmentId);
            viewModel.ParentObjectives = await GetParentObjectiveSelectList(viewModel.ParentObjectiveId);
            return View(viewModel);
        }

        // GET: BusinessObjective/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
            if (objective == null || !objective.IsActive)
            {
                return NotFound();
            }

            var viewModel = new BusinessObjectiveEditViewModel
            {
                Id = objective.Id,
                Name = objective.Name,
                Description = objective.Description,
                BusinessPerspective = objective.BusinessPerspective,
                Priority = objective.Priority,
                Status = objective.Status,
                ProgressPercentage = objective.ProgressPercentage,
                StartDate = objective.StartDate,
                TargetDate = objective.TargetDate,
                CompletionDate = objective.CompletionDate,
                DepartmentId = objective.DepartmentId,
                ParentObjectiveId = objective.ParentObjectiveId,
                Budget = objective.Budget,
                Notes = objective.Notes,
                FiscalYear = objective.FiscalYear,
                Timeframe = objective.Timeframe,
                ResponsiblePersonId = objective.ResponsiblePersonId,
                Departments = await GetDepartmentSelectList(objective.DepartmentId),
                ParentObjectives = await GetParentObjectiveSelectList(objective.ParentObjectiveId, objective.Id)
            };

            return View(viewModel);
        }

        // POST: BusinessObjective/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BusinessObjectiveEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
                    if (objective == null || !objective.IsActive)
                    {
                        return NotFound();
                    }

                    objective.Name = viewModel.Name;
                    objective.Description = viewModel.Description;
                    objective.BusinessPerspective = viewModel.BusinessPerspective;
                    objective.Priority = viewModel.Priority;
                    objective.Status = viewModel.Status;
                    objective.ProgressPercentage = viewModel.ProgressPercentage;
                    objective.StartDate = viewModel.StartDate;
                    objective.TargetDate = viewModel.TargetDate;
                    objective.CompletionDate = viewModel.CompletionDate;
                    objective.DepartmentId = viewModel.DepartmentId;
                    objective.ParentObjectiveId = viewModel.ParentObjectiveId;
                    objective.Budget = viewModel.Budget;
                    objective.Notes = viewModel.Notes;
                    objective.FiscalYear = viewModel.FiscalYear;
                    objective.Timeframe = viewModel.Timeframe;
                    objective.ResponsiblePersonId = viewModel.ResponsiblePersonId;
                    objective.UpdatedAt = DateTime.UtcNow;
                    objective.UpdatedBy = User.Identity?.Name ?? "System";

                    _unitOfWork.BusinessObjectives.Update(objective);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["Success"] = "Mục tiêu kinh doanh đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating business objective");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi cập nhật mục tiêu kinh doanh.");
                }
            }

            // If we got to here, something failed; redisplay form
            viewModel.Departments = await GetDepartmentSelectList(viewModel.DepartmentId);
            viewModel.ParentObjectives = await GetParentObjectiveSelectList(viewModel.ParentObjectiveId, viewModel.Id);
            return View(viewModel);
        }

        // GET: BusinessObjective/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
            if (objective == null || !objective.IsActive)
            {
                return NotFound();
            }

            return View(objective);
        }

        // POST: BusinessObjective/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
            if (objective == null || !objective.IsActive)
            {
                return NotFound();
            }

            // Check if there are any child objectives
            var allObjectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var childObjectives = allObjectives.Where(o => o.ParentObjectiveId == id && o.IsActive).ToList();
            if (childObjectives.Any())
            {
                TempData["Error"] = "Không thể xóa mục tiêu này vì có các mục tiêu con liên quan. Vui lòng xóa các mục tiêu con trước.";
                return RedirectToAction(nameof(Index));
            }

            // Check if there are any related CSFs
            var allCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
            var relatedCsfs = allCsfs.Where(c => c.BusinessObjectiveId == id && c.IsActive).ToList();
            if (relatedCsfs.Any())
            {
                TempData["Error"] = "Không thể xóa mục tiêu này vì có các yếu tố thành công quan trọng (CSF) liên quan. Vui lòng xóa các CSF trước.";
                return RedirectToAction(nameof(Index));
            }

            objective.IsActive = false;
            objective.UpdatedAt = DateTime.UtcNow;
            objective.UpdatedBy = User.Identity?.Name ?? "System";

            _unitOfWork.BusinessObjectives.Update(objective);
            await _unitOfWork.SaveChangesAsync();

            TempData["Success"] = "Mục tiêu kinh doanh đã được xóa thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetDepartmentSelectList(Guid? selectedDepartmentId = null)
        {
            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            var departments = allDepartments.Where(d => d.IsActive).ToList();
            return new SelectList(departments.OrderBy(d => d.Name), "Id", "Name", selectedDepartmentId);
        }

        private async Task<SelectList> GetParentObjectiveSelectList(Guid? selectedObjectiveId = null, Guid? currentObjectiveId = null)
        {
            var allObjectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var objectives = allObjectives.Where(o => o.IsActive).ToList();

            // Filter out the current objective (if editing) and its children to prevent circular references
            if (currentObjectiveId.HasValue)
            {
                var childrenIds = await GetAllChildObjectiveIds(currentObjectiveId.Value);
                objectives = objectives.Where(o => o.Id != currentObjectiveId.Value && !childrenIds.Contains(o.Id)).ToList();
            }

            return new SelectList(objectives.OrderBy(o => o.Name), "Id", "Name", selectedObjectiveId);
        }

        private async Task<List<Guid>> GetAllChildObjectiveIds(Guid objectiveId)
        {
            var result = new List<Guid>();
            var allObjectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var directChildren = allObjectives.Where(o => o.ParentObjectiveId == objectiveId && o.IsActive).ToList();

            foreach (var child in directChildren)
            {
                result.Add(child.Id);
                var grandChildren = await GetAllChildObjectiveIds(child.Id);
                result.AddRange(grandChildren);
            }

            return result;
        }

        // Update the progress percentage of an objective
        [HttpPost]
        public async Task<IActionResult> UpdateProgress(Guid id, int progress)
        {
            if (progress < 0 || progress > 100)
            {
                return BadRequest("Progress must be between 0 and 100");
            }

            var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
            if (objective == null || !objective.IsActive)
            {
                return NotFound();
            }

            objective.ProgressPercentage = progress;

            // Update status based on progress
            if (progress == 0)
            {
                objective.Status = ObjectiveStatus.NotStarted;
            }
            else if (progress == 100)
            {
                objective.Status = ObjectiveStatus.Completed;
                objective.CompletionDate = DateTime.Today;
            }
            else
            {
                objective.Status = ObjectiveStatus.InProgress;
            }

            objective.UpdatedAt = DateTime.UtcNow;
            objective.UpdatedBy = User.Identity?.Name ?? "System";

            _unitOfWork.BusinessObjectives.Update(objective);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { success = true, message = "Progress updated successfully" });
        }
    }
}