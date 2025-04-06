namespace KPISolution.Models.Enums.Measurement
{
    /// <summary>
    /// Represents the direction of a measurement (whether higher or lower values are better)
    /// </summary>
    public enum MeasurementDirection
    {
        [Display(Name = "Cao hơn là tốt hơn")]
        HigherIsBetter = 1,

        [Display(Name = "Thấp hơn là tốt hơn")]
        LowerIsBetter = 2,

        [Display(Name = "Đạt mục tiêu là tốt nhất")]
        TargetIsBetter = 3
    }

    /// <summary>
    /// Represents the trend direction of measurements
    /// </summary>
    public enum TrendDirection
    {
        [Display(Name = "Tăng")]
        Up = 1,

        [Display(Name = "Giảm")]
        Down = 2,

        [Display(Name = "Ổn định")]
        Steady = 3,

        [Display(Name = "Dao động")]
        Fluctuating = 4,

        [Display(Name = "Chưa đủ dữ liệu")]
        NotEnoughData = 5
    }

    /// <summary>
    /// Represents the frequency of measurements
    /// </summary>
    public enum MeasurementFrequency
    {
        [Display(Name = "Hàng ngày")]
        Daily = 1,

        [Display(Name = "Hàng tuần")]
        Weekly = 2,

        [Display(Name = "Hai tuần một lần")]
        BiWeekly = 3,

        [Display(Name = "Hàng tháng")]
        Monthly = 4,

        [Display(Name = "Hàng quý")]
        Quarterly = 5,

        [Display(Name = "Nửa năm")]
        SemiAnnually = 6,

        [Display(Name = "Hàng năm")]
        Annually = 7,

        [Display(Name = "Tùy chỉnh")]
        Custom = 8
    }

    /// <summary>
    /// Represents the frequency of reviews
    /// </summary>
    public enum ReviewFrequency
    {
        [Display(Name = "Hàng ngày")]
        Daily = 1,

        [Display(Name = "Hàng tuần")]
        Weekly = 2,

        [Display(Name = "Hai tuần một lần")]
        BiWeekly = 3,

        [Display(Name = "Hàng tháng")]
        Monthly = 4,

        [Display(Name = "Hàng quý")]
        Quarterly = 5,

        [Display(Name = "Nửa năm")]
        SemiAnnually = 6,

        [Display(Name = "Hàng năm")]
        Annually = 7,

        [Display(Name = "Khi cần thiết")]
        AsNeeded = 8
    }

    /// <summary>
    /// Trạng thái của giá trị đo lường
    /// </summary>
    public enum MeasurementStatus
    {
        [Display(Name = "Chưa thiết lập")]
        NotSet = -1,

        [Display(Name = "Thực tế")]
        Actual = 0,

        [Display(Name = "Dự kiến")]
        Expected = 1,

        [Display(Name = "Mục tiêu")]
        Target = 2,

        [Display(Name = "Ngưỡng tối thiểu")]
        Threshold = 3
    }

    /// <summary>
    /// Represents the method used to collect measurement data
    /// </summary>
    public enum DataCollectionMethod
    {
        [Display(Name = "Nhập thủ công")]
        ManualInput = 1,

        [Display(Name = "Hệ thống tự động")]
        AutomatedSystem = 2,

        [Display(Name = "Nhập từ file")]
        FileImport = 3,

        [Display(Name = "Tích hợp API")]
        ApiIntegration = 4,

        [Display(Name = "Truy vấn cơ sở dữ liệu")]
        DatabaseQuery = 5,

        [Display(Name = "Khảo sát/Phản hồi")]
        SurveyFeedback = 6,

        [Display(Name = "Thiết bị IoT")]
        IoTDevices = 7,

        [Display(Name = "Khác")]
        Other = 8
    }

    /// <summary>
    /// Represents the data source of measurements
    /// </summary>
    public enum DataSource
    {
        [Display(Name = "Hệ thống nội bộ")]
        InternalSystem = 1,

        [Display(Name = "Hệ thống bên ngoài")]
        ExternalSystem = 2,

        [Display(Name = "Nhập thủ công")]
        ManualEntry = 3,

        [Display(Name = "Tính toán")]
        Calculation = 4,

        [Display(Name = "Khảo sát")]
        Survey = 5,

        [Display(Name = "Cơ sở dữ liệu")]
        Database = 6,

        [Display(Name = "API")]
        API = 7,

        [Display(Name = "Thiết bị IoT")]
        IoTDevice = 8,

        [Display(Name = "Bên thứ ba")]
        ThirdParty = 9,

        [Display(Name = "Khác")]
        Other = 10
    }

    /// <summary>
    /// Đơn vị đo lường
    /// </summary>
    public enum MeasurementUnit
    {
        [Display(Name = "Phần trăm (%)")]
        Percentage = 0,

        [Display(Name = "Số lượng (#)")]
        Count = 1,

        [Display(Name = "Tiền tệ (đ)")]
        Currency = 2,

        [Display(Name = "Thời gian (phút)")]
        Time = 3,

        [Display(Name = "Tỷ lệ (x:y)")]
        Ratio = 4,

        [Display(Name = "Điểm số")]
        Score = 5,

        [Display(Name = "Khác")]
        Other = 6,

        [Display(Name = "Số")]
        Number = 7
    }

    /// <summary>
    /// Represents the type of measurement period
    /// </summary>
    public enum PeriodType
    {
        [Display(Name = "Ngày")]
        Day = 1,

        [Display(Name = "Tuần")]
        Week = 2,

        [Display(Name = "Tháng")]
        Month = 3,

        [Display(Name = "Quý")]
        Quarter = 4,

        [Display(Name = "Năm")]
        Year = 5,

        [Display(Name = "Tùy chỉnh")]
        Custom = 6
    }

    /// <summary>
    /// Trạng thái của target
    /// </summary>
    public enum TargetStatus
    {
        [Display(Name = "Chờ xác nhận")]
        Pending = 0,

        [Display(Name = "Đã xác nhận")]
        Active = 1,

        [Display(Name = "Đạt mục tiêu")]
        Achieved = 2,

        [Display(Name = "Chưa đạt")]
        Missed = 3,

        [Display(Name = "Đang tiến hành")]
        InProgress = 4,

        [Display(Name = "Có rủi ro")]
        AtRisk = 5,

        [Display(Name = "Đã hủy bỏ")]
        Cancelled = 6
    }
}
