namespace KPISolution.Models.ViewModels.Users
{
    /// <summary>
    /// View model for editing an existing user
    /// </summary>
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [Display(Name = "Tên")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [Display(Name = "Họ")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Chức danh")]
        public string JobTitle { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        [Display(Name = "Quản lý")]
        public string ManagerId { get; set; } = string.Empty;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Là người sở hữu Indicator")]
        public bool IsIndicatorOwner { get; set; }

        [Display(Name = "Là quản trị viên phòng ban")]
        public bool IsDepartmentAdmin { get; set; }

        [Display(Name = "Là người sở hữu KPI")]
        public bool IsKpiOwner { get; set; }
    }
}
