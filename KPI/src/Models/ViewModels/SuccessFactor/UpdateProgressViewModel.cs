using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model cho việc cập nhật tiến độ của yếu tố thành công
    /// </summary>
    public class UpdateProgressViewModel
    {
        /// <summary>
        /// ID của yếu tố thành công
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Mã yếu tố thành công
        /// </summary>
        [Display(Name = "Mã yếu tố")]
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Tên yếu tố thành công
        /// </summary>
        [Display(Name = "Tên yếu tố")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Mô tả chi tiết
        /// </summary>
        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; init; }

        /// <summary>
        /// Phòng ban chịu trách nhiệm
        /// </summary>
        [Display(Name = "Phòng ban")]
        public string DepartmentName { get; init; } = string.Empty;

        /// <summary>
        /// Mục tiêu liên quan
        /// </summary>
        [Display(Name = "Mục tiêu")]
        public string? ObjectiveName { get; init; }

        /// <summary>
        /// Tỷ lệ hoàn thành hiện tại
        /// </summary>
        [Display(Name = "% Hiện tại")]
        public decimal CurrentProgressPercentage { get; init; }

        /// <summary>
        /// Trạng thái hiện tại
        /// </summary>
        [Display(Name = "Trạng thái hiện tại")]
        public SuccessFactorStatus CurrentStatus { get; init; }

        /// <summary>
        /// Tỷ lệ hoàn thành mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập tỷ lệ hoàn thành")]
        [Range(0, 100, ErrorMessage = "Tỷ lệ hoàn thành phải từ 0 đến 100")]
        [Display(Name = "% Hoàn thành mới")]
        public decimal NewProgressPercentage { get; init; }

        /// <summary>
        /// Trạng thái mới
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái mới")]
        public SuccessFactorStatus NewStatus { get; init; }

        /// <summary>
        /// Ghi chú cập nhật
        /// </summary>
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú cập nhật")]
        public string? Notes { get; init; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày cập nhật")]
        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdateDate { get; init; } = DateTime.Today;

        /// <summary>
        /// Danh sách các trạng thái để chọn
        /// </summary>
        public IEnumerable<SelectListItem>? StatusList { get; init; }

        /// <summary>
        /// Lịch sử cập nhật tiến độ
        /// </summary>
        public List<ProgressUpdateViewModel>? ProgressHistory { get; init; }
    }
}
