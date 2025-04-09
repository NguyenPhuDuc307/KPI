namespace KPISolution.Models.Enums.Indicator
{
    /// <summary>
    /// Enum representing basic indicator types in the system
    /// </summary>
    public enum BasicIndicatorType
    {
        /// <summary>
        /// Key Result Indicator
        /// </summary>
        [Display(Name = "Chỉ số kết quả chính (KRI)")]
        KRI = 1,

        /// <summary>
        /// Result Indicator
        /// </summary>
        [Display(Name = "Chỉ số kết quả (RI)")]
        RI = 2,

        /// <summary>
        /// Performance Indicator
        /// </summary>
        [Display(Name = "Chỉ số hiệu suất (PI)")]
        PI = 3,

        /// <summary>
        /// Key Performance Indicator
        /// </summary>
        [Display(Name = "Chỉ số hiệu suất chính (KPI)")]
        KPI = 4
    }

    /// <summary>
    /// Status options for Performance Indicators
    /// </summary>
    public enum IndicatorStatus
    {
        /// <summary>
        /// Draft status - not yet active
        /// </summary>
        [Display(Name = "Bản nháp")]
        Draft = 1,

        /// <summary>
        /// Active status - currently being tracked
        /// </summary>
        [Display(Name = "Đang hoạt động")]
        Active = 2,

        /// <summary>
        /// Under review status
        /// </summary>
        [Display(Name = "Đang xem xét")]
        UnderReview = 3,

        /// <summary>
        /// Archived status - no longer active
        /// </summary>
        [Display(Name = "Đã lưu trữ")]
        Archived = 4,

        /// <summary>
        /// Approved status
        /// </summary>
        [Display(Name = "Đã phê duyệt")]
        Approved = 5,

        /// <summary>
        /// On target status
        /// </summary>
        [Display(Name = "Đạt mục tiêu")]
        OnTarget = 6,

        /// <summary>
        /// At risk status
        /// </summary>
        [Display(Name = "Có rủi ro")]
        AtRisk = 7,

        /// <summary>
        /// Below target status
        /// </summary>
        [Display(Name = "Dưới mục tiêu")]
        BelowTarget = 8,

        /// <summary>
        /// Deprecated status
        /// </summary>
        [Display(Name = "Không còn sử dụng")]
        Deprecated = 9
    }

    /// <summary>
    /// Categories of Performance Indicators
    /// </summary>
    public enum IndicatorCategory
    {
        /// <summary>
        /// Financial category
        /// </summary>
        [Display(Name = "Tài chính")]
        Financial = 1,

        /// <summary>
        /// Customer category
        /// </summary>
        [Display(Name = "Khách hàng")]
        Customer = 2,

        /// <summary>
        /// Internal business process category
        /// </summary>
        [Display(Name = "Quy trình nội bộ")]
        InternalBusiness = 3,

        /// <summary>
        /// Learning and growth category
        /// </summary>
        [Display(Name = "Học hỏi & Phát triển")]
        LearningAndGrowth = 4,

        /// <summary>
        /// Operations category
        /// </summary>
        [Display(Name = "Vận hành")]
        Operations = 5,

        /// <summary>
        /// Quality category
        /// </summary>
        [Display(Name = "Chất lượng")]
        Quality = 6,

        /// <summary>
        /// Human resources category
        /// </summary>
        [Display(Name = "Nhân sự")]
        HumanResources = 7,

        /// <summary>
        /// Information technology category
        /// </summary>
        [Display(Name = "Công nghệ thông tin")]
        IT = 8,

        /// <summary>
        /// Environmental category
        /// </summary>
        [Display(Name = "Môi trường")]
        Environmental = 9,

        /// <summary>
        /// Social category
        /// </summary>
        [Display(Name = "Xã hội")]
        Social = 10,

        /// <summary>
        /// Governance category
        /// </summary>
        [Display(Name = "Quản trị")]
        Governance = 11,

        /// <summary>
        /// Innovation category
        /// </summary>
        [Display(Name = "Đổi mới sáng tạo")]
        Innovation = 12,

        /// <summary>
        /// Productivity category
        /// </summary>
        [Display(Name = "Năng suất")]
        Productivity = 13,

        /// <summary>
        /// Safety category
        /// </summary>
        [Display(Name = "An toàn")]
        Safety = 14,

        /// <summary>
        /// Project category
        /// </summary>
        [Display(Name = "Dự án")]
        Project = 15,

        /// <summary>
        /// Risk category
        /// </summary>
        [Display(Name = "Rủi ro")]
        Risk = 16,

        /// <summary>
        /// Other category
        /// </summary>
        [Display(Name = "Khác")]
        Other = 99
    }

    /// <summary>
    /// Enum representing different impact levels of indicators
    /// </summary>
    public enum ImpactLevel
    {
        /// <summary>
        /// Low impact
        /// </summary>
        [Display(Name = "Thấp")]
        Low = 1,

        /// <summary>
        /// Medium impact
        /// </summary>
        [Display(Name = "Trung bình")]
        Medium = 2,

        /// <summary>
        /// High impact
        /// </summary>
        [Display(Name = "Cao")]
        High = 3,

        /// <summary>
        /// Critical impact
        /// </summary>
        [Display(Name = "Rất cao")]
        Critical = 4
    }

    /// <summary>
    /// Enum representing different business areas
    /// </summary>
    public enum BusinessArea
    {
        /// <summary>
        /// General business area
        /// </summary>
        [Display(Name = "Chung")]
        General = 1,

        /// <summary>
        /// Sales business area
        /// </summary>
        [Display(Name = "Bán hàng")]
        Sales = 2,

        /// <summary>
        /// Marketing business area
        /// </summary>
        [Display(Name = "Marketing")]
        Marketing = 3,

        /// <summary>
        /// Finance business area
        /// </summary>
        [Display(Name = "Tài chính")]
        Finance = 4,

        /// <summary>
        /// Human Resources business area
        /// </summary>
        [Display(Name = "Nhân sự")]
        HR = 5,

        /// <summary>
        /// Operations business area
        /// </summary>
        [Display(Name = "Vận hành")]
        Operations = 6,

        /// <summary>
        /// Information Technology business area
        /// </summary>
        [Display(Name = "Công nghệ thông tin")]
        IT = 7,

        /// <summary>
        /// Research and Development business area
        /// </summary>
        [Display(Name = "R&D")]
        RnD = 8,

        /// <summary>
        /// Customer Service business area
        /// </summary>
        [Display(Name = "Dịch vụ khách hàng")]
        CustomerService = 9,

        /// <summary>
        /// Production business area
        /// </summary>
        [Display(Name = "Sản xuất")]
        Production = 10,

        /// <summary>
        /// Supply Chain business area
        /// </summary>
        [Display(Name = "Chuỗi cung ứng")]
        SupplyChain = 11,

        /// <summary>
        /// Quality business area
        /// </summary>
        [Display(Name = "Chất lượng")]
        Quality = 12,

        /// <summary>
        /// Other business area
        /// </summary>
        [Display(Name = "Khác")]
        Other = 13
    }

    /// <summary>
    /// Enum representing different activity types
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Business Process activity type
        /// </summary>
        [Display(Name = "Quy trình kinh doanh")]
        BusinessProcess = 1,

        /// <summary>
        /// Project activity type
        /// </summary>
        [Display(Name = "Dự án")]
        Project = 2,

        /// <summary>
        /// Operational activity type
        /// </summary>
        [Display(Name = "Hoạt động vận hành")]
        Operational = 3,

        /// <summary>
        /// Strategic activity type
        /// </summary>
        [Display(Name = "Hoạt động chiến lược")]
        Strategic = 4,

        /// <summary>
        /// Compliance activity type
        /// </summary>
        [Display(Name = "Hoạt động tuân thủ")]
        Compliance = 5,

        /// <summary>
        /// Innovation activity type
        /// </summary>
        [Display(Name = "Hoạt động đổi mới")]
        Innovation = 6,

        /// <summary>
        /// Maintenance activity type
        /// </summary>
        [Display(Name = "Hoạt động bảo trì")]
        Maintenance = 7,

        /// <summary>
        /// Other activity type
        /// </summary>
        [Display(Name = "Hoạt động khác")]
        Other = 8
    }

    /// <summary>
    /// Enum representing indicator hierarchy types
    /// </summary>
    public enum IndicatorHierarchyType
    {
        /// <summary>
        /// Objective - Strategic goals or objectives
        /// </summary>
        [Display(Name = "Mục tiêu")]
        Objective = 1,

        /// <summary>
        /// Success Factor - Factors contributing to objectives
        /// </summary>
        [Display(Name = "SuccessFactor")]
        SuccessFactor = 2,

        /// <summary>
        /// Critical Success Factor - Critical elements for strategy success
        /// </summary>
        [Display(Name = "Critical SuccessFactor")]
        CriticalSuccessFactor = 3,

        /// <summary>
        /// Result Indicator - Measures what has been achieved
        /// </summary>
        [Display(Name = "Chỉ số kết quả")]
        ResultIndicator = 4,

        /// <summary>
        /// Performance Indicator - Measures what to do to improve performance
        /// </summary>
        [Display(Name = "Chỉ số hiệu suất")]
        PerformanceIndicator = 5,

        /// <summary>
        /// Key Result Indicator - Key measures of what has been achieved
        /// </summary>
        [Display(Name = "Chỉ số kết quả chính")]
        KeyResultIndicator = 6,

        /// <summary>
        /// Key Performance Indicator - Key measures of what to do to improve performance
        /// </summary>
        [Display(Name = "Chỉ số hiệu suất chính")]
        KeyPerformanceIndicator = 7
    }

    /// <summary>
    /// Enum representing different indicator timing types
    /// </summary>
    public enum IndicatorTimingType
    {
        /// <summary>
        /// Leading indicator - predictive measure
        /// </summary>
        [Display(Name = "Chỉ báo dẫn đầu")]
        Leading = 1,

        /// <summary>
        /// Lagging indicator - output measure
        /// </summary>
        [Display(Name = "Chỉ báo trễ")]
        Lagging = 2,

        /// <summary>
        /// Coincident indicator - real-time measure
        /// </summary>
        [Display(Name = "Chỉ báo đồng thời")]
        Coincident = 3
    }

    /// <summary>
    /// Permission levels for Performance Indicator access
    /// </summary>
    public enum IndicatorAccessPermission
    {
        /// <summary>
        /// View permission
        /// </summary>
        [Display(Name = "Xem")]
        View = 1,

        /// <summary>
        /// Edit permission
        /// </summary>
        [Display(Name = "Chỉnh sửa")]
        Edit = 2,

        /// <summary>
        /// Delete permission
        /// </summary>
        [Display(Name = "Xóa")]
        Delete = 3,

        /// <summary>
        /// Approve permission
        /// </summary>
        [Display(Name = "Phê duyệt")]
        Approve = 4,

        /// <summary>
        /// Measure permission
        /// </summary>
        [Display(Name = "Đo lường")]
        Measure = 5,

        /// <summary>
        /// Admin permission
        /// </summary>
        [Display(Name = "Quản trị")]
        Admin = 10
    }

    /// <summary>
    /// Permission level in the system
    /// </summary>
    public enum IndicatorPermissionLevel
    {
        /// <summary>
        /// None permission
        /// </summary>
        [Display(Name = "Không có")]
        None = 0,

        /// <summary>
        /// Read permission
        /// </summary>
        [Display(Name = "Đọc")]
        Read = 1,

        /// <summary>
        /// Write permission
        /// </summary>
        [Display(Name = "Ghi")]
        Write = 2,

        /// <summary>
        /// Delete permission
        /// </summary>
        [Display(Name = "Xóa")]
        Delete = 3,

        /// <summary>
        /// Admin permission
        /// </summary>
        [Display(Name = "Quản trị")]
        Admin = 4
    }

    /// <summary>
    /// Represents the scope of measurement
    /// </summary>
    public enum MeasurementScope
    {
        /// <summary>
        /// Individual scope
        /// </summary>
        [Display(Name = "Cá nhân")]
        Individual = 1,

        /// <summary>
        /// Team scope
        /// </summary>
        [Display(Name = "Nhóm")]
        Team = 2,

        /// <summary>
        /// Department scope
        /// </summary>
        [Display(Name = "Phòng ban")]
        Department = 3,

        /// <summary>
        /// Division scope
        /// </summary>
        [Display(Name = "Bộ phận")]
        Division = 4,

        /// <summary>
        /// Organization scope
        /// </summary>
        [Display(Name = "Tổ chức")]
        Organization = 5,

        /// <summary>
        /// Project scope
        /// </summary>
        [Display(Name = "Dự án")]
        Project = 6,

        /// <summary>
        /// Process scope
        /// </summary>
        [Display(Name = "Quy trình")]
        Process = 7,

        /// <summary>
        /// Other scope
        /// </summary>
        [Display(Name = "Khác")]
        Other = 8
    }

    /// <summary>
    /// Represents process areas in the organization
    /// </summary>
    public enum ProcessArea
    {
        /// <summary>
        /// Core Business Processes
        /// </summary>
        [Display(Name = "Quy trình kinh doanh cốt lõi")]
        CoreBusiness = 1,

        /// <summary>
        /// Support Processes
        /// </summary>
        [Display(Name = "Quy trình hỗ trợ")]
        Support = 2,

        /// <summary>
        /// Management Processes
        /// </summary>
        [Display(Name = "Quy trình quản lý")]
        Management = 3,

        /// <summary>
        /// Strategic Processes
        /// </summary>
        [Display(Name = "Quy trình chiến lược")]
        Strategic = 4,

        /// <summary>
        /// Operational Processes
        /// </summary>
        [Display(Name = "Quy trình vận hành")]
        Operational = 5,

        /// <summary>
        /// Innovation Processes
        /// </summary>
        [Display(Name = "Quy trình đổi mới")]
        Innovation = 6,

        /// <summary>
        /// Customer-facing Processes
        /// </summary>
        [Display(Name = "Quy trình tiếp xúc khách hàng")]
        CustomerFacing = 7,

        /// <summary>
        /// Administrative Processes
        /// </summary>
        [Display(Name = "Quy trình hành chính")]
        Administrative = 8,

        /// <summary>
        /// Compliance Processes
        /// </summary>
        [Display(Name = "Quy trình tuân thủ")]
        Compliance = 9,

        /// <summary>
        /// Other Processes
        /// </summary>
        [Display(Name = "Quy trình khác")]
        Other = 10
    }

    /// <summary>
    /// Represents time frames for indicators
    /// </summary>
    public enum TimeFrame
    {
        /// <summary>
        /// Daily time frame
        /// </summary>
        [Display(Name = "Hàng ngày")]
        Daily = 1,

        /// <summary>
        /// Weekly time frame
        /// </summary>
        [Display(Name = "Hàng tuần")]
        Weekly = 2,

        /// <summary>
        /// Monthly time frame
        /// </summary>
        [Display(Name = "Hàng tháng")]
        Monthly = 3,

        /// <summary>
        /// Quarterly time frame
        /// </summary>
        [Display(Name = "Hàng quý")]
        Quarterly = 4,

        /// <summary>
        /// Annual time frame
        /// </summary>
        [Display(Name = "Hàng năm")]
        Annual = 5,

        /// <summary>
        /// Multi-year time frame
        /// </summary>
        [Display(Name = "Nhiều năm")]
        MultiYear = 6,

        /// <summary>
        /// Project-based time frame
        /// </summary>
        [Display(Name = "Theo dự án")]
        ProjectBased = 7,

        /// <summary>
        /// Custom time frame
        /// </summary>
        [Display(Name = "Tùy chỉnh")]
        Custom = 8
    }

    /// <summary>
    /// Represents types of results for result indicators
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Quantitative results that can be measured numerically
        /// </summary>
        [Display(Name = "Định lượng")]
        Quantitative = 1,

        /// <summary>
        /// Qualitative results that are descriptive
        /// </summary>
        [Display(Name = "Định tính")]
        Qualitative = 2,

        /// <summary>
        /// Financial results related to money
        /// </summary>
        [Display(Name = "Tài chính")]
        Financial = 3,

        /// <summary>
        /// Operational results related to operations
        /// </summary>
        [Display(Name = "Vận hành")]
        Operational = 4,

        /// <summary>
        /// Customer-related results
        /// </summary>
        [Display(Name = "Khách hàng")]
        Customer = 5,

        /// <summary>
        /// Employee-related results
        /// </summary>
        [Display(Name = "Nhân viên")]
        Employee = 6,

        /// <summary>
        /// Process-related results
        /// </summary>
        [Display(Name = "Quy trình")]
        Process = 7,

        /// <summary>
        /// Performance-related results
        /// </summary>
        [Display(Name = "Hiệu suất")]
        Performance = 8,

        /// <summary>
        /// Project-related results
        /// </summary>
        [Display(Name = "Dự án")]
        Project = 9,

        /// <summary>
        /// Strategic results related to strategic goals
        /// </summary>
        [Display(Name = "Chiến lược")]
        Strategic = 10,

        /// <summary>
        /// Other types of results
        /// </summary>
        [Display(Name = "Khác")]
        Other = 11
    }

    /// <summary>
    /// Simplified indicator status for views
    /// </summary>
    public enum SimpleIndicatorStatus
    {
        /// <summary>
        /// Not started status
        /// </summary>
        [Display(Name = "Chưa bắt đầu")]
        NotStarted = 0,

        /// <summary>
        /// In progress status
        /// </summary>
        [Display(Name = "Đang tiến hành")]
        InProgress = 1,

        /// <summary>
        /// At risk status
        /// </summary>
        [Display(Name = "Có rủi ro")]
        AtRisk = 2,

        /// <summary>
        /// Completed status
        /// </summary>
        [Display(Name = "Hoàn thành")]
        Completed = 3,

        /// <summary>
        /// Cancelled status
        /// </summary>
        [Display(Name = "Đã hủy")]
        Cancelled = 4
    }
}