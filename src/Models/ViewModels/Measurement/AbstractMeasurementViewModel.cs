namespace KPISolution.Models.ViewModels.Measurement
{
    /// <summary>
    /// Lớp cơ sở trừu tượng cho các ViewModel liên quan đến đo lường
    /// </summary>
    public abstract class AbstractMeasurementViewModel
    {
        /// <summary>
        /// ID của phép đo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ngày thực hiện đo lường
        /// </summary>
        [Display(Name = "Ngày đo")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Giá trị đo được
        /// </summary>
        [Display(Name = "Giá trị đo được")]
        public decimal Value { get; set; }

        /// <summary>
        /// Giá trị mục tiêu
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Trạng thái của phép đo
        /// </summary>
        [Display(Name = "Trạng thái")]
        public MeasurementStatus Status { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        /// <summary>
        /// Tính phần trăm hoàn thành
        /// </summary>
        [Display(Name = "% hoàn thành")]
        public decimal CalculateAchievementPercentage()
        {
            if (this.TargetValue.HasValue && this.TargetValue.Value != 0)
            {
                return Math.Round((this.Value / this.TargetValue.Value) * 100, 2);
            }
            return 0;
        }

        /// <summary>
        /// Tính sai lệch so với mục tiêu
        /// </summary>
        [Display(Name = "Sai lệch")]
        public decimal CalculateVariance()
        {
            return this.TargetValue.HasValue ? this.Value - this.TargetValue.Value : 0;
        }
    }
}