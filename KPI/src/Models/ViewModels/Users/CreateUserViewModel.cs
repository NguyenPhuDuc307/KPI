namespace KPISolution.Models.ViewModels.Users
{
    /// <summary>
    /// View model for creating a new user
    /// </summary>
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;

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
        public bool IsActive { get; set; } = true;

        [Display(Name = "Là người sở hữu Indicator")]
        public bool IsIndicatorOwner { get; set; }

        [Display(Name = "Là quản trị viên phòng ban")]
        public bool IsDepartmentAdmin { get; set; }

        [Display(Name = "Là người sở hữu KPI")]
        public bool IsKpiOwner { get; set; }
    }
}
