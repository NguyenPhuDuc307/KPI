namespace KPISolution.Models.ViewModels.SuccessFactor
{
    /// <summary>
    /// View model cho modal cập nhật tiến độ của yếu tố thành công
    /// </summary>
    public class SuccessFactorProgressUpdateViewModel
    {
        /// <summary>
        /// ID của yếu tố thành công cần cập nhật
        /// </summary>
        [Required]
        public Guid CSFId { get; set; }

        /// <summary>
        /// Mã của yếu tố thành công
        /// </summary>
        [Display(Name = "Mã")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên của yếu tố thành công
        /// </summary>
        [Display(Name = "Tên")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Phần trăm tiến độ hiện tại
        /// </summary>
        [Range(0, 100)]
        [Display(Name = "Tiến độ (%)")]
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// Thành tựu hoặc ghi chú cập nhật
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập thông tin về những gì đã đạt được")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Những gì đã đạt được")]
        public string Achievements { get; set; } = string.Empty;

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Người cập nhật
        /// </summary>
        [Display(Name = "Người cập nhật")]
        public string UpdatedBy { get; set; } = string.Empty;
    }
}