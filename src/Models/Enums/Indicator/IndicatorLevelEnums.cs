namespace KPISolution.Models.Enums.Indicator
{
    /// <summary>
    /// Cấp độ kiểm soát chỉ số
    /// </summary>
    public enum ControlLevel
    {
        [Display(Name = "Chiến lược")]
        Strategic = 0,

        [Display(Name = "Chiến thuật")]
        Tactical = 1,

        [Display(Name = "Hoạt động")]
        Operational = 2,

        [Display(Name = "Tài chính")]
        Financial = 3,

        [Display(Name = "Nhân sự")]
        HR = 4,

        [Display(Name = "Khách hàng")]
        Customer = 5,

        [Display(Name = "Quy trình nội bộ")]
        Process = 6,

        [Display(Name = "Học hỏi & phát triển")]
        Learning = 7,

        [Display(Name = "Khác")]
        Other = 8
    }

    /// <summary>
    /// Loại chỉ số
    /// </summary>
    public enum IndicatorType
    {
        [Display(Name = "KPI")]
        KPI = 0,

        [Display(Name = "KRI")]
        KRI = 1,

        [Display(Name = "PI")]
        PI = 2,

        [Display(Name = "RI")]
        RI = 3,

        [Display(Name = "CSF")]
        CSF = 4,

        [Display(Name = "SF")]
        SF = 5
    }

    /// <summary>
    /// Cấp độ ưu tiên
    /// </summary>
    public enum PriorityLevel
    {
        [Display(Name = "Thấp")]
        Low = 0,

        [Display(Name = "Trung bình")]
        Medium = 1,

        [Display(Name = "Cao")]
        High = 2,

        [Display(Name = "Quan trọng")]
        Critical = 3
    }

    /// <summary>
    /// Cấp độ rủi ro
    /// </summary>
    public enum RiskLevel
    {
        [Display(Name = "Không có")]
        None = 0,

        [Display(Name = "Không đáng kể")]
        Negligible = 1,

        [Display(Name = "Thấp")]
        Low = 2,

        [Display(Name = "Trung bình")]
        Medium = 3,

        [Display(Name = "Cao")]
        High = 4,

        [Display(Name = "Nghiêm trọng")]
        Critical = 5
    }
}
