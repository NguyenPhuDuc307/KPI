using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;
using System.Collections.Generic;

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
        /// KPI unique identifier (alias for Id for compatibility)
        /// </summary>
        public Guid KpiId { get => Id; set => Id = value; }

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
        /// Measurement unit
        /// </summary>
        [Display(Name = "Unit")]
        public string? Unit { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Type of KPI
        /// </summary>
        [Display(Name = "KPI Type")]
        public KpiType KpiType { get; set; }

        /// <summary>
        /// Linked Performance Indicators (for RI)
        /// </summary>
        [Display(Name = "Linked Performance Indicators")]
        public List<LinkedKpiViewModel> LinkedPIs { get; set; } = new List<LinkedKpiViewModel>();
    }
}