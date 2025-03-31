using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Type of item that can be displayed on a dashboard
    /// </summary>
    public enum DashboardItemType
    {
        /// <summary>
        /// KPI chart or card
        /// </summary>
        [Display(Name = "KPI")]
        Kpi = 1,

        /// <summary>
        /// CSF progress
        /// </summary>
        [Display(Name = "CSF")]
        Csf = 2,

        /// <summary>
        /// Department overview
        /// </summary>
        [Display(Name = "Department")]
        Department = 3,

        /// <summary>
        /// Custom widget
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 4
    }
}