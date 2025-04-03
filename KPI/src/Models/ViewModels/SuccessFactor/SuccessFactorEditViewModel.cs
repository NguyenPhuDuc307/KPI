using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.SuccessFactor
{
    public class SuccessFactorEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên yếu tố thành công là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        [Display(Name = "Tên yếu tố thành công")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã yếu tố thành công là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã không được vượt quá 20 ký tự")]
        [Display(Name = "Mã yếu tố thành công")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Mã chỉ được chứa chữ cái, số và dấu gạch ngang")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Mức độ ưu tiên")]
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;

        [Display(Name = "Là yếu tố thành công quan trọng (CSF)")]
        public bool IsCSF { get; set; } = false;

        [Range(0, 100, ErrorMessage = "Tiến độ phải từ 0 đến 100")]
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; } = 0;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Ngày hoàn thành là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hoàn thành")]
        public DateTime TargetDate { get; set; } = DateTime.Today.AddMonths(3);

        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        [Required(ErrorMessage = "Mục tiêu kinh doanh là bắt buộc")]
        [Display(Name = "Mục tiêu kinh doanh")]
        public Guid BusinessObjectiveId { get; set; }

        [StringLength(100, ErrorMessage = "Tên người phụ trách không được vượt quá 100 ký tự")]
        [Display(Name = "Người phụ trách")]
        public string? Owner { get; set; }

        [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        // Dropdown lists for selecting related entities
        public SelectList? BusinessObjectives { get; set; }
        public SelectList? Departments { get; set; }
    }
}