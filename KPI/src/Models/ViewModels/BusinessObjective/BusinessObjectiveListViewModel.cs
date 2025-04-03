using System;
using System.Collections.Generic;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.BusinessObjective
{
    public class BusinessObjectiveListViewModel
    {
        public List<BusinessObjectiveListItemViewModel> Objectives { get; set; } = new List<BusinessObjectiveListItemViewModel>();
        public string SearchTerm { get; set; } = string.Empty;
        public BusinessPerspective? FilterPerspective { get; set; }
        public ObjectiveStatus? FilterStatus { get; set; }
        public PriorityLevel? FilterPriority { get; set; }
        public Guid? FilterDepartmentId { get; set; }
        public TimeframeType? FilterTimeframe { get; set; }

        // Sorting properties
        public string SortBy { get; set; } = "Name";
        public string SortDirection { get; set; } = "asc";

        // Helper method to get the opposite sort direction for toggling
        public string GetOppositeSortDirection() => SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

        // Helper method to get the sort icon based on the current sort direction
        public string GetSortIcon(string columnName)
        {
            if (SortBy.Equals(columnName, StringComparison.OrdinalIgnoreCase))
            {
                return SortDirection.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? "bi-sort-up"
                    : "bi-sort-down";
            }

            return "bi-sort";
        }
    }

    public class BusinessObjectiveListItemViewModel
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
        public string Department { get; set; } = string.Empty;
        public TimeframeType TimeframeType { get; set; }

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

        public string BusinessPerspectiveClass
        {
            get
            {
                return BusinessPerspective switch
                {
                    BusinessPerspective.Financial => "text-success",
                    BusinessPerspective.Customer => "text-primary",
                    BusinessPerspective.InternalProcess => "text-info",
                    BusinessPerspective.LearningGrowth => "text-warning",
                    _ => ""
                };
            }
        }

        public string TimeframeBadgeClass
        {
            get
            {
                return TimeframeType switch
                {
                    TimeframeType.ShortTerm => "badge bg-info text-dark",
                    TimeframeType.MediumTerm => "badge bg-primary",
                    TimeframeType.LongTerm => "badge bg-dark",
                    _ => "badge bg-secondary"
                };
            }
        }
    }
}