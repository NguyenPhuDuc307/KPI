using System;
using System.Collections.Generic;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for the KPI tree view
    /// </summary>
    public class KpiTreeViewModel
    {
        /// <summary>
        /// List of Key Result Indicators (KRIs) - these are at the top level
        /// </summary>
        public List<KpiTreeNodeViewModel> KeyResultIndicators { get; set; } = new List<KpiTreeNodeViewModel>();

        /// <summary>
        /// List of Result Indicators (RIs) - these can be children of KRIs or standalone
        /// </summary>
        public List<KpiTreeNodeViewModel> ResultIndicators { get; set; } = new List<KpiTreeNodeViewModel>();

        /// <summary>
        /// List of Performance Indicators (PIs) - these can be children of RIs or standalone
        /// </summary>
        public List<KpiTreeNodeViewModel> PerformanceIndicators { get; set; } = new List<KpiTreeNodeViewModel>();
    }

    /// <summary>
    /// View model for a node in the KPI tree
    /// </summary>
    public class KpiTreeNodeViewModel
    {
        /// <summary>
        /// Unique identifier for the KPI
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the KPI
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Code of the KPI
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Type of the KPI (KRI, RI, PI)
        /// </summary>
        public KpiType Type { get; set; }

        /// <summary>
        /// Department that owns this KPI
        /// </summary>
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Description of the KPI
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Status of the KPI (e.g., Active, Draft)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// ID of the parent node (if any)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Child nodes of this KPI
        /// </summary>
        public List<KpiTreeNodeViewModel>? Children { get; set; }

        /// <summary>
        /// Calculated property to determine if this node has children
        /// </summary>
        public bool HasChildren => Children != null && Children.Count > 0;

        /// <summary>
        /// CSS class based on KPI type for display styling
        /// </summary>
        public string GetTypeClass()
        {
            return Type switch
            {
                KpiType.KeyResultIndicator => "kri",
                KpiType.ResultIndicator => "ri",
                KpiType.PerformanceIndicator => "pi",
                _ => "unknown"
            };
        }

        /// <summary>
        /// Icon based on KPI type for display
        /// </summary>
        public string GetTypeIcon()
        {
            return Type switch
            {
                KpiType.KeyResultIndicator => "bi-bullseye",
                KpiType.ResultIndicator => "bi-graph-up",
                KpiType.PerformanceIndicator => "bi-speedometer",
                _ => "bi-question-circle"
            };
        }

        /// <summary>
        /// Get the controller name based on KPI type
        /// </summary>
        public string GetController()
        {
            return Type switch
            {
                KpiType.KeyResultIndicator => "Kri",
                KpiType.ResultIndicator => "Ri",
                KpiType.PerformanceIndicator => "PI",
                _ => "Kpi"
            };
        }
    }
}