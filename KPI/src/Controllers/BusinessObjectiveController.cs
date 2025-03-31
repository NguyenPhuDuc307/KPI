using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public BusinessObjectiveController(IUnitOfWork unitOfWork, ILogger<BusinessObjectiveController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: BusinessObjective
        public async Task<IActionResult> Index()
        {
            var objectives = await _unitOfWork.BusinessObjectives.GetAllAsync();
            var viewModel = new BusinessObjectiveListViewModel
            {
                Objectives = objectives.Where(o => o.IsActive).Select(o => new BusinessObjectiveListItemViewModel
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
                }).OrderBy(o => o.Name).ToList()
            };

            return View(viewModel);
        }

        // GET: BusinessObjective/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var objective = await _unitOfWork.BusinessObjectives.GetByIdAsync(id);
            if (objective == null || !objective.IsActive)
            {
                return NotFound();
            }

            // Get related CSFs
            var allCsfs = await _unitOfWork.CriticalSuccessFactors.GetAllAsync();
            var csfs = allCsfs.Where(c => c.BusinessObjectiveId == id && c.IsActive).ToList();

            var viewModel = new BusinessObjectiveDetailsViewModel
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
                Department = objective.Department?.Name,
                Budget = objective.Budget,
                Notes = objective.Notes,
                FiscalYear = objective.FiscalYear,
                Timeframe = objective.Timeframe,
                RelatedCSFs = csfs.Select(c => new CsfListItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Code = c.Code,
                    Status = c.Status,
                    ProgressPercentage = c.ProgressPercentage
                }).ToList()
            };

            // Get child objectives if any
            if (objective.ChildObjectives != null && objective.ChildObjectives.Any())
            {
                viewModel.ChildObjectives = objective.ChildObjectives.Where(o => o.IsActive).Select(o => new BusinessObjectiveListItemViewModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Status = o.Status,
                    ProgressPercentage = o.ProgressPercentage,
                    TargetDate = o.TargetDate
                }).ToList();
            }

            // Get parent objective if any
            if (objective.ParentObjectiveId.HasValue)
            {
                var parentObjective = await _unitOfWork.BusinessObjectives.GetByIdAsync(objective.ParentObjectiveId.Value);
                if (parentObjective != null)
                {
                    viewModel.ParentObjective = new BusinessObjectiveListItemViewModel
                    {
                        Id = parentObjective.Id,
                        Name = parentObjective.Name,
                        Status = parentObjective.Status,
                        ProgressPercentage = parentObjective.ProgressPercentage
                    };
                }
            }

            return View(viewModel);
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