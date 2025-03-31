using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for linked KPIs (minimal information for references)
    /// </summary>
    public class LinkedKpiViewModel
    {
        /// <summary>
        /// KPI unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// KPI code 
        /// </summary>
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// KPI name
        /// </summary>
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// KPI status
        /// </summary>
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Target value
        /// </summary>
        [Display(Name = "Target")]
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Current value
        /// </summary>
        [Display(Name = "Current Value")]
        public decimal? CurrentValue { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;
    }
}