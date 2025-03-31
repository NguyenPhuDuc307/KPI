using System;
using System.Collections.Generic;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.BusinessObjective
{
    public class BusinessObjectiveDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public BusinessPerspective BusinessPerspective { get; set; }
        public PriorityLevel Priority { get; set; }
        public ObjectiveStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Department { get; set; }
        public decimal? Budget { get; set; }
        public string? Notes { get; set; }
        public string? FiscalYear { get; set; }
        public TimeframeType Timeframe { get; set; }

        // Related entities
        public BusinessObjectiveListItemViewModel? ParentObjective { get; set; }
        public List<BusinessObjectiveListItemViewModel> ChildObjectives { get; set; } = new List<BusinessObjectiveListItemViewModel>();
        public List<CsfListItemViewModel> RelatedCSFs { get; set; } = new List<CsfListItemViewModel>();

        // Helper properties
        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    ObjectiveStatus.NotStarted => "badge bg-secondary",
                    ObjectiveStatus.InProgress => "badge bg-primary",
                    ObjectiveStatus.OnHold => "badge bg-warning text-dark",
                    ObjectiveStatus.Completed => "badge bg-success",
                    ObjectiveStatus.Canceled => "badge bg-danger",
                    ObjectiveStatus.Delayed => "badge bg-info text-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        public string PriorityBadgeClass
        {
            get
            {
                return Priority switch
                {
                    PriorityLevel.Low => "badge bg-success",
                    PriorityLevel.Medium => "badge bg-warning text-dark",
                    PriorityLevel.High => "badge bg-danger",
                    PriorityLevel.Critical => "badge bg-dark",
                    _ => "badge bg-secondary"
                };
            }
        }

        public string ProgressBarClass
        {
            get
            {
                return ProgressPercentage switch
                {
                    100 => "progress-bar bg-success",
                    >= 75 => "progress-bar bg-info",
                    >= 50 => "progress-bar bg-primary",
                    >= 25 => "progress-bar bg-warning",
                    _ => "progress-bar bg-danger"
                };
            }
        }

        public string TimeframeDisplayText
        {
            get
            {
                return Timeframe switch
                {
                    TimeframeType.ShortTerm => "Ngắn hạn",
                    TimeframeType.MediumTerm => "Trung hạn",
                    TimeframeType.LongTerm => "Dài hạn",
                    _ => "Không xác định"
                };
            }
        }

        public string BusinessPerspectiveDisplayText
        {
            get
            {
                return BusinessPerspective switch
                {
                    BusinessPerspective.Financial => "Tài chính",
                    BusinessPerspective.Customer => "Khách hàng",
                    BusinessPerspective.InternalProcess => "Quy trình nội bộ",
                    BusinessPerspective.LearningGrowth => "Học hỏi và phát triển",
                    _ => "Không xác định"
                };
            }
        }
    }

    public class CsfListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public CSFStatus Status { get; set; }
        public int ProgressPercentage { get; set; }

        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    CSFStatus.NotStarted => "badge bg-secondary",
                    CSFStatus.InProgress => "badge bg-primary",
                    CSFStatus.Delayed => "badge bg-warning text-dark",
                    CSFStatus.Completed => "badge bg-success",
                    CSFStatus.AtRisk => "badge bg-danger",
                    _ => "badge bg-secondary"
                };
            }
        }
    }
}