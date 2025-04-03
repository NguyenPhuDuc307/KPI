using System;
using System.Collections.Generic;
using KPISolution.Models.Enums;
using KPISolution.Models.ViewModels.CSF;
using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.BusinessObjective
{
    public class BusinessObjectiveDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BusinessPerspective BusinessPerspective { get; set; }
        public string BusinessPerspectiveDisplayText { get; set; } = string.Empty;
        public PriorityLevel Priority { get; set; }
        public string PriorityBadgeClass { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public string ProgressBarClass { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime TargetDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CompletionDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? Budget { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? FiscalYear { get; set; }
        public TimeframeType TimeframeType { get; set; }
        public string TimeframeDisplayText { get; set; } = string.Empty;

        // Related entities
        public BusinessObjectiveSimpleViewModel? ParentObjective { get; set; }
        public List<BusinessObjectiveSimpleViewModel> ChildObjectives { get; set; } = new();
        public List<CSFSimpleViewModel> RelatedCSFs { get; set; } = new();
        public List<SFSimpleViewModel> RelatedSFs { get; set; } = new();
        public List<KPISimpleViewModel> RelatedKPIs { get; set; } = new();
        public List<KPISimpleViewModel> RelatedKRIs { get; set; } = new();
        public List<KPISimpleViewModel> RelatedRIs { get; set; } = new();
        public List<KPISimpleViewModel> RelatedPIs { get; set; } = new();
    }

    public class BusinessObjectiveSimpleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class SFSimpleViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ObjectiveStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
    }

    public class CSFSimpleViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public CSFStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
    }

    public class KPISimpleViewModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public KpiType KpiType { get; set; }
        public KpiStatus Status { get; set; }
        public string StatusBadgeClass { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal TargetValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
    }
}