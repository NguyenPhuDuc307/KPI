namespace KPISolution.Models.Enums.Object
{
    /// <summary>
    /// Status of a business objective
    /// </summary>
    public enum ObjectiveStatus
    {
        /// <summary>
        /// Not started yet
        /// </summary>
        [Display(Name = "Chưa bắt đầu")]
        NotStarted = 1,

        /// <summary>
        /// Currently in progress
        /// </summary>
        [Display(Name = "Đang tiến hành")]
        InProgress = 2,

        /// <summary>
        /// At risk of not meeting objectives
        /// </summary>
        [Display(Name = "Có rủi ro")]
        AtRisk = 3,

        /// <summary>
        /// Delayed from original timeline
        /// </summary>
        [Display(Name = "Bị trì hoãn")]
        Delayed = 4,

        /// <summary>
        /// Successfully completed
        /// </summary>
        [Display(Name = "Hoàn thành")]
        Completed = 5,

        /// <summary>
        /// Cancelled and no longer being pursued
        /// </summary>
        [Display(Name = "Đã hủy bỏ")]
        Cancelled = 6
    }

    /// <summary>
    /// Business perspectives based on balanced scorecard
    /// </summary>
    public enum BusinessPerspective
    {
        /// <summary>
        /// Financial perspective
        /// </summary>
        [Display(Name = "Tài chính")]
        Financial = 1,

        /// <summary>
        /// Customer perspective
        /// </summary>
        [Display(Name = "Khách hàng")]
        Customer = 2,

        /// <summary>
        /// Internal process perspective
        /// </summary>
        [Display(Name = "Quy trình nội bộ")]
        InternalProcess = 3,

        /// <summary>
        /// Learning and growth perspective
        /// </summary>
        [Display(Name = "Học hỏi và phát triển")]
        LearningAndGrowth = 4,

        /// <summary>
        /// Sustainability perspective
        /// </summary>
        [Display(Name = "Phát triển bền vững")]
        Sustainability = 5,

        /// <summary>
        /// Innovation perspective
        /// </summary>
        [Display(Name = "Đổi mới sáng tạo")]
        Innovation = 6,

        /// <summary>
        /// Digital transformation perspective
        /// </summary>
        [Display(Name = "Chuyển đổi số")]
        DigitalTransformation = 7,

        /// <summary>
        /// Risk management perspective
        /// </summary>
        [Display(Name = "Quản lý rủi ro")]
        RiskManagement = 8,

        /// <summary>
        /// Operational excellence perspective
        /// </summary>
        [Display(Name = "Hoạt động xuất sắc")]
        OperationalExcellence = 9,

        /// <summary>
        /// Employee engagement perspective
        /// </summary>
        [Display(Name = "Gắn kết nhân viên")]
        EmployeeEngagement = 10,

        /// <summary>
        /// Other perspective
        /// </summary>
        [Display(Name = "Khác")]
        Other = 11
    }

    /// <summary>
    /// Timeframe types for objectives and goals
    /// </summary>
    public enum TimeframeType
    {
        /// <summary>
        /// Short-term timeframe
        /// </summary>
        [Display(Name = "Ngắn hạn")]
        ShortTerm = 1,

        /// <summary>
        /// Medium-term timeframe
        /// </summary>
        [Display(Name = "Trung hạn")]
        MediumTerm = 2,

        /// <summary>
        /// Long-term timeframe
        /// </summary>
        [Display(Name = "Dài hạn")]
        LongTerm = 3,

        /// <summary>
        /// Strategic timeframe
        /// </summary>
        [Display(Name = "Chiến lược")]
        Strategic = 4,

        /// <summary>
        /// Operational timeframe
        /// </summary>
        [Display(Name = "Vận hành")]
        Operational = 5,

        /// <summary>
        /// Continuous timeframe
        /// </summary>
        [Display(Name = "Liên tục")]
        Continuous = 6,

        /// <summary>
        /// Annual timeframe
        /// </summary>
        [Display(Name = "Hàng năm")]
        Annual = 7,

        /// <summary>
        /// Quarterly timeframe
        /// </summary>
        [Display(Name = "Hàng quý")]
        Quarterly = 8,

        /// <summary>
        /// Monthly timeframe
        /// </summary>
        [Display(Name = "Hàng tháng")]
        Monthly = 9,

        /// <summary>
        /// Custom timeframe
        /// </summary>
        [Display(Name = "Tùy chỉnh")]
        Custom = 10
    }

    /// <summary>
    /// Business areas for objectives and initiatives
    /// </summary>
    public enum BusinessArea
    {
        /// <summary>
        /// Finance business area
        /// </summary>
        [Display(Name = "Tài chính")]
        Finance = 1,

        /// <summary>
        /// Operations business area
        /// </summary>
        [Display(Name = "Vận hành")]
        Operations = 2,

        /// <summary>
        /// Human Resources business area
        /// </summary>
        [Display(Name = "Nhân sự")]
        HumanResources = 3,

        /// <summary>
        /// Marketing business area
        /// </summary>
        [Display(Name = "Marketing")]
        Marketing = 4,

        /// <summary>
        /// Sales business area
        /// </summary>
        [Display(Name = "Kinh doanh")]
        Sales = 5,

        /// <summary>
        /// IT business area
        /// </summary>
        [Display(Name = "Công nghệ thông tin")]
        IT = 6,

        /// <summary>
        /// Customer Service business area
        /// </summary>
        [Display(Name = "Dịch vụ khách hàng")]
        CustomerService = 7,

        /// <summary>
        /// Product Development business area
        /// </summary>
        [Display(Name = "Phát triển sản phẩm")]
        ProductDevelopment = 8,

        /// <summary>
        /// Supply Chain business area
        /// </summary>
        [Display(Name = "Chuỗi cung ứng")]
        SupplyChain = 9,

        /// <summary>
        /// Legal business area
        /// </summary>
        [Display(Name = "Pháp lý")]
        Legal = 10
    }
}