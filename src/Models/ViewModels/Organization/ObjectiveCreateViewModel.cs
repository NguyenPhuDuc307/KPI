namespace KPISolution.Models.ViewModels.Organization
{
    /// <summary>
    /// ViewModel sử dụng cho form tạo mới mục tiêu
    /// </summary>
    public class ObjectiveCreateViewModel
    {
        /// <summary>
        /// Identifies the objective being edited (null for new objectives)
        /// </summary>
        public Guid? Id { get; set; }

        [Display(Name = "Mã mục tiêu")]
        [Required(ErrorMessage = "Vui lòng nhập mã mục tiêu")]
        [StringLength(20, ErrorMessage = "Mã mục tiêu không được vượt quá 20 ký tự")]
        public string Code { get; set; } = string.Empty;

        [Display(Name = "Tên mục tiêu")]
        [Required(ErrorMessage = "Vui lòng nhập tên mục tiêu")]
        [StringLength(200, ErrorMessage = "Tên mục tiêu không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "Ngày kết thúc")]
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(3);

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; set; } = ObjectiveStatus.NotStarted;

        [Display(Name = "Khung thời gian")]
        [Required(ErrorMessage = "Vui lòng chọn khung thời gian")]
        public TimeframeType Timeframe { get; set; }

        [Display(Name = "Khía cạnh kinh doanh")]
        [Required(ErrorMessage = "Vui lòng chọn khía cạnh kinh doanh")]
        public BusinessPerspective Perspective { get; set; }

        [Display(Name = "Đơn vị")]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "Người phụ trách")]
        [Required(ErrorMessage = "Vui lòng chọn người phụ trách")]
        public Guid ResponsiblePersonId { get; set; }

        [Display(Name = "Mục tiêu cha")]
        public Guid? ParentObjectiveId { get; set; }

        [Display(Name = "Tiến độ (%)")]
        [Range(0, 100, ErrorMessage = "Tiến độ phải từ 0 đến 100%")]
        public int ProgressPercentage { get; set; } = 0;
    }
}
