using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.BusinessObjective
{
    public class BusinessObjectiveEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên mục tiêu là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên mục tiêu không được vượt quá 200 ký tự")]
        [Display(Name = "Tên mục tiêu")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phối cảnh kinh doanh là bắt buộc")]
        [Display(Name = "Phối cảnh kinh doanh")]
        public BusinessPerspective BusinessPerspective { get; set; }

        [Required(ErrorMessage = "Mức độ ưu tiên là bắt buộc")]
        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel Priority { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;

        [Range(0, 100, ErrorMessage = "Tiến độ phải từ 0% đến 100%")]
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; } = 0;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Ngày hoàn thành dự kiến là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hoàn thành dự kiến")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddMonths(3);

        [DataType(DataType.Date)]
        [Display(Name = "Ngày hoàn thành thực tế")]
        public DateTime? CompletionDate { get; set; }

        [Display(Name = "Phòng ban phụ trách")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "Người phụ trách")]
        public string? ResponsiblePersonId { get; set; }

        [Display(Name = "Mục tiêu cha")]
        public Guid? ParentObjectiveId { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Ngân sách")]
        public decimal? Budget { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        [StringLength(9, ErrorMessage = "Năm tài chính không được vượt quá 9 ký tự")]
        [Display(Name = "Năm tài chính")]
        public string? FiscalYear { get; set; }

        [Required(ErrorMessage = "Thời hạn là bắt buộc")]
        [Display(Name = "Thời hạn")]
        public TimeframeType Timeframe { get; set; } = TimeframeType.MediumTerm;

        // Dropdown lists for select elements
        public SelectList? Departments { get; set; }
        public SelectList? ParentObjectives { get; set; }
        public SelectList? ResponsiblePersons { get; set; }
    }
}