using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// ViewModel for displaying separated KPI types
    /// </summary>
    public class KpiSeparatedViewModel
    {
        /// <summary>
        /// List of Key Performance Indicators
        /// </summary>
        public List<KpiInfoViewModel> KeyPerformanceIndicators { get; set; } = new List<KpiInfoViewModel>();

        /// <summary>
        /// List of Performance Indicators
        /// </summary>
        public List<KpiInfoViewModel> PerformanceIndicators { get; set; } = new List<KpiInfoViewModel>();

        /// <summary>
        /// List of Key Result Indicators
        /// </summary>
        public List<KpiInfoViewModel> KeyResultIndicators { get; set; } = new List<KpiInfoViewModel>();

        /// <summary>
        /// List of Result Indicators
        /// </summary>
        public List<KpiInfoViewModel> ResultIndicators { get; set; } = new List<KpiInfoViewModel>();
    }
}