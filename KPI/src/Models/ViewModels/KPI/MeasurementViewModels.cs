using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for filtering measurements
    /// </summary>
    public class MeasurementFilterViewModel
    {
        /// <summary>
        /// Search term for filtering measurements
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Start date for filtering measurements
        /// </summary>
        [Display(Name = "Từ ngày")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// End date for filtering measurements
        /// </summary>
        [Display(Name = "Đến ngày")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Department ID for filtering
        /// </summary>
        [Display(Name = "Phòng ban")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// Type of KPI for filtering
        /// </summary>
        [Display(Name = "Loại chỉ số")]
        public KpiType? KpiType { get; set; }

        /// <summary>
        /// Measurement frequency for filtering
        /// </summary>
        [Display(Name = "Tần suất đo")]
        public MeasurementFrequency? Frequency { get; set; }
    }

    /// <summary>
    /// View model for displaying list of measurements
    /// </summary>
    public class MeasurementListViewModel
    {
        /// <summary>
        /// Filter criteria
        /// </summary>
        public MeasurementFilterViewModel Filter { get; set; } = new();

        /// <summary>
        /// List of measurement items
        /// </summary>
        public List<KpiValueViewModel> Items { get; set; } = new();

        /// <summary>
        /// Current page number
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Total number of items
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// List of departments for dropdown
        /// </summary>
        public SelectList? Departments { get; set; }

        /// <summary>
        /// List of KPI types for dropdown
        /// </summary>
        public SelectList? KpiTypes { get; set; }

        /// <summary>
        /// List of measurement frequencies for dropdown
        /// </summary>
        public SelectList? MeasurementFrequencies { get; set; }
    }

    /// <summary>
    /// View model for adding a new measurement
    /// </summary>
    public class AddMeasurementViewModel
    {
        /// <summary>
        /// ID of the KPI being measured
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// Code of the KPI
        /// </summary>
        [Display(Name = "Mã chỉ số")]
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// Name of the KPI
        /// </summary>
        [Display(Name = "Tên chỉ số")]
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Display(Name = "Đơn vị")]
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Target value for comparison
        /// </summary>
        [Display(Name = "Giá trị mục tiêu")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Actual measured value
        /// </summary>
        [Required(ErrorMessage = "Vui lòng nhập giá trị đo lường")]
        [Display(Name = "Giá trị đo được")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Date of measurement
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn ngày đo lường")]
        [Display(Name = "Ngày đo")]
        public DateTime MeasurementDate { get; set; }

        /// <summary>
        /// Additional notes about the measurement
        /// </summary>
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// View model for displaying KPI measurement history
    /// </summary>
    public class KpiMeasurementHistoryViewModel
    {
        /// <summary>
        /// ID of the KPI
        /// </summary>
        public Guid KpiId { get; set; }

        /// <summary>
        /// Code of the KPI
        /// </summary>
        public string KpiCode { get; set; } = string.Empty;

        /// <summary>
        /// Name of the KPI
        /// </summary>
        public string KpiName { get; set; } = string.Empty;

        /// <summary>
        /// Unit of measurement
        /// </summary>
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Target value for comparison
        /// </summary>
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// List of historical measurements
        /// </summary>
        public List<KpiValueViewModel> Measurements { get; set; } = new();

        /// <summary>
        /// Chart data for visualization
        /// </summary>
        public object? ChartData => GenerateChartData();

        private object? GenerateChartData()
        {
            if (!Measurements.Any())
                return null;

            var labels = Measurements
                .OrderBy(m => m.MeasurementDate)
                .Select(m => m.MeasurementDate.ToString("dd/MM/yyyy"))
                .ToList();

            var values = Measurements
                .OrderBy(m => m.MeasurementDate)
                .Select(m => m.ActualValue)
                .ToList();

            var targetValues = Enumerable
                .Repeat(TargetValue ?? 0, labels.Count)
                .ToList();

            return new ChartDataModel
            {
                Labels = labels,
                Datasets = new[]
                {
                    new ChartDataset
                    {
                        Label = "Giá trị đo được",
                        Data = values,
                        BorderColor = "#4CAF50",
                        Fill = false
                    },
                    new ChartDataset
                    {
                        Label = "Mục tiêu",
                        Data = targetValues,
                        BorderColor = "#FFA726",
                        BorderDash = new[] { 5, 5 },
                        Fill = false
                    }
                }
            };
        }
    }

    /// <summary>
    /// Model for chart data
    /// </summary>
    public class ChartDataModel
    {
        public List<string> Labels { get; set; } = new();
        public ChartDataset[] Datasets { get; set; } = Array.Empty<ChartDataset>();
    }

    /// <summary>
    /// Model for chart dataset
    /// </summary>
    public class ChartDataset
    {
        public string Label { get; set; } = string.Empty;
        public List<decimal> Data { get; set; } = new();
        public string BorderColor { get; set; } = string.Empty;
        public bool Fill { get; set; }
        public int[]? BorderDash { get; set; }
    }
}