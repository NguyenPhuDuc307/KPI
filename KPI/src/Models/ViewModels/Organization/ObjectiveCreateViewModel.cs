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
        public Guid? Id { get; init; }

        [Display(Name = "Mã mục tiêu")]
        [Required(ErrorMessage = "Vui lòng nhập mã mục tiêu")]
        [StringLength(20, ErrorMessage = "Mã mục tiêu không được vượt quá 20 ký tự")]
        public string Code { get; init; } = string.Empty;

        [Display(Name = "Tên mục tiêu")]
        [Required(ErrorMessage = "Vui lòng nhập tên mục tiêu")]
        [StringLength(200, ErrorMessage = "Tên mục tiêu không được vượt quá 200 ký tự")]
        public string Name { get; init; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; init; }

        [Display(Name = "Ngày bắt đầu")]
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; init; } = DateTime.Today;

        [Display(Name = "Ngày kết thúc")]
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; init; } = DateTime.Today.AddMonths(3);

        [Display(Name = "Trạng thái")]
        public ObjectiveStatus Status { get; init; } = ObjectiveStatus.NotStarted;

        [Display(Name = "Khung thời gian")]
        [Required(ErrorMessage = "Vui lòng chọn khung thời gian")]
        public TimeframeType Timeframe { get; init; }

        [Display(Name = "Khía cạnh kinh doanh")]
        [Required(ErrorMessage = "Vui lòng chọn khía cạnh kinh doanh")]
        public BusinessPerspective Perspective { get; init; }

        [Display(Name = "Đơn vị")]
        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        public Guid DepartmentId { get; init; }

        [Display(Name = "Người phụ trách")]
        [Required(ErrorMessage = "Vui lòng chọn người phụ trách")]
        public Guid ResponsiblePersonId { get; init; }

        [Display(Name = "Mục tiêu cha")]
        public Guid? ParentObjectiveId { get; init; }

        [Display(Name = "Tiến độ (%)")]
        [Range(0, 100, ErrorMessage = "Tiến độ phải từ 0 đến 100%")]
        public int ProgressPercentage { get; init; } = 0;
    }
}
