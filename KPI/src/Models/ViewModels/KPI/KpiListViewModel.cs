using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for displaying a list of KPIs with filtering and pagination
    /// </summary>
    public class KpiListViewModel
    {
        /// <summary>
        /// Constructor to initialize collections
        /// </summary>
        public KpiListViewModel()
        {
            KpiItems = new List<KpiListItemViewModel>();
            Filter = new KpiFilterViewModel();
            Departments = new SelectList(Array.Empty<SelectListItem>());
            Categories = new SelectList(Array.Empty<SelectListItem>());
            KpiTypes = new SelectList(Array.Empty<SelectListItem>());
            Statuses = new SelectList(Array.Empty<SelectListItem>());
            PerformanceLevels = new SelectList(Array.Empty<SelectListItem>());
            CriticalSuccessFactors = new SelectList(Array.Empty<SelectListItem>());
            Frequencies = new SelectList(Array.Empty<SelectListItem>());
            Directions = new SelectList(Array.Empty<SelectListItem>());
            BusinessAreas = new SelectList(Array.Empty<SelectListItem>());
            ImpactLevels = new SelectList(Array.Empty<SelectListItem>());
            MeasurementFrequencies = new SelectList(Array.Empty<SelectListItem>());
            ActivityTypes = new SelectList(Array.Empty<SelectListItem>());
            ProcessAreas = new SelectList(Array.Empty<SelectListItem>());
            SortOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "Name", Text = "Name" },
                new SelectListItem { Value = "Code", Text = "Code" },
                new SelectListItem { Value = "Department", Text = "Department" },
                new SelectListItem { Value = "Status", Text = "Status" },
                new SelectListItem { Value = "CurrentValue", Text = "Current Value" },
                new SelectListItem { Value = "UpdatedAt", Text = "Last Updated" }
            }, "Value", "Text");
        }

        /// <summary>
        /// List of KPI items to display
        /// </summary>
        public List<KpiListItemViewModel> KpiItems { get; set; } = new List<KpiListItemViewModel>();

        /// <summary>
        /// Total count of KPIs matching the filter
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Filter criteria
        /// </summary>
        public KpiFilterViewModel Filter { get; set; } = new KpiFilterViewModel();

        /// <summary>
        /// List of departments for dropdown
        /// </summary>
        public SelectList Departments { get; set; }

        /// <summary>
        /// List of categories for dropdown
        /// </summary>
        public SelectList Categories { get; set; }

        /// <summary>
        /// List of KPI types for dropdown
        /// </summary>
        public SelectList KpiTypes { get; set; }

        /// <summary>
        /// List of statuses for dropdown
        /// </summary>
        public SelectList Statuses { get; set; }

        /// <summary>
        /// List of performance levels for dropdown
        /// </summary>
        public SelectList PerformanceLevels { get; set; }

        /// <summary>
        /// List of CSFs for dropdown
        /// </summary>
        public SelectList CriticalSuccessFactors { get; set; }

        /// <summary>
        /// List of frequencies for dropdown
        /// </summary>
        public SelectList Frequencies { get; set; }

        /// <summary>
        /// List of directions for dropdown
        /// </summary>
        public SelectList Directions { get; set; }

        /// <summary>
        /// List of business areas for dropdown
        /// </summary>
        public SelectList BusinessAreas { get; set; }

        /// <summary>
        /// List of impact levels for dropdown
        /// </summary>
        public SelectList ImpactLevels { get; set; }

        /// <summary>
        /// List of measurement frequencies for dropdown
        /// </summary>
        public SelectList MeasurementFrequencies { get; set; }

        /// <summary>
        /// List of activity types for dropdown
        /// </summary>
        public SelectList ActivityTypes { get; set; }

        /// <summary>
        /// List of process areas for dropdown
        /// </summary>
        public SelectList ProcessAreas { get; set; }

        /// <summary>
        /// List of sort options for dropdown
        /// </summary>
        public SelectList SortOptions { get; set; } = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Name", Text = "Name" },
            new SelectListItem { Value = "Code", Text = "Code" },
            new SelectListItem { Value = "Department", Text = "Department" },
            new SelectListItem { Value = "Status", Text = "Status" },
            new SelectListItem { Value = "CurrentValue", Text = "Current Value" },
            new SelectListItem { Value = "UpdatedAt", Text = "Last Updated" }
        }, "Value", "Text");

        /// <summary>
        /// Whether to show archived KPIs
        /// </summary>
        public bool ShowArchived { get; set; } = false;

        /// <summary>
        /// Period start date for filtering
        /// </summary>
        public DateTime? PeriodStart { get; set; }

        /// <summary>
        /// Period end date for filtering
        /// </summary>
        public DateTime? PeriodEnd { get; set; }

        /// <summary>
        /// Whether there is a next page
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;

        /// <summary>
        /// Whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Sort by field name
        /// </summary>
        public string SortBy { get; set; } = "Name";

        /// <summary>
        /// Sort direction (asc or desc)
        /// </summary>
        public string SortDirection { get; set; } = "asc";

        /// <summary>
        /// Whether to show only KPIs that are at risk or below target
        /// </summary>
        public bool ShowAtRiskOnly { get; set; } = false;
    }
}
