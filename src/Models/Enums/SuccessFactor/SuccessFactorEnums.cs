namespace KPISolution.Models.Enums.SuccessFactor
{
    /// <summary>
    /// Trạng thái của yếu tố thành công (Deprecated - Use SuccessFactorStatus instead)
    /// </summary>
    public enum SuccessFactorStatus
    {
        [Display(Name = "Chưa bắt đầu")]
        NotStarted = 0,

        [Display(Name = "Đang tiến hành")]
        InProgress = 1,

        [Display(Name = "Đúng tiến độ")]
        OnTrack = 2,

        [Display(Name = "Có rủi ro")]
        AtRisk = 3,

        [Display(Name = "Chậm tiến độ")]
        OffTrack = 4,

        [Display(Name = "Hoàn thành")]
        Completed = 5,

        [Display(Name = "Hủy bỏ")]
        Cancelled = 6
    }

    /// <summary>
    /// Phân loại yếu tố thành công
    /// </summary>
    public enum SuccessFactorCategory
    {
        [Display(Name = "Tài chính")]
        Financial = 0,

        [Display(Name = "Khách hàng")]
        Customer = 1,

        [Display(Name = "Quy trình nội bộ")]
        InternalProcess = 2,

        [Display(Name = "Học hỏi & phát triển")]
        LearningAndGrowth = 3,

        [Display(Name = "Chất lượng")]
        Quality = 4,

        [Display(Name = "Hiệu quả")]
        Efficiency = 5,

        [Display(Name = "Thị trường")]
        Market = 6,

        [Display(Name = "Công nghệ")]
        Technology = 7,

        [Display(Name = "Nhân lực")]
        Human = 8,

        [Display(Name = "Khác")]
        Other = 9
    }
}
