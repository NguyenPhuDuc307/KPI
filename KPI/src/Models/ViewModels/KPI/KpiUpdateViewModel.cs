using System;
using System.ComponentModel.DataAnnotations;
using KPISolution.Models.Enums;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// View model for updating KPI measurement values
    /// </summary>
    public class KpiUpdateViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the KPI.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the KPI.
        /// </summary>
        [Display(Name = "KPI Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the code or identifier of the KPI.
        /// </summary>
        [Display(Name = "KPI Code")]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target value for this KPI.
        /// </summary>
        [Display(Name = "Target Value")]
        public decimal? TargetValue { get; set; }

        /// <summary>
        /// Gets or sets the actual value for this KPI.
        /// </summary>
        [Required]
        [Display(Name = "Actual Value")]
        public decimal ActualValue { get; set; }

        /// <summary>
        /// Gets or sets the unit of measurement.
        /// </summary>
        [Display(Name = "Unit")]
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the measurement date.
        /// </summary>
        [Required]
        [Display(Name = "Measurement Date")]
        [DataType(DataType.Date)]
        public DateTime MeasurementDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the notes for this measurement.
        /// </summary>
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the measurement direction.
        /// </summary>
        [Display(Name = "Measurement Direction")]
        public MeasurementDirection MeasurementDirection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to update the KPI status based on the actual value.
        /// </summary>
        [Display(Name = "Update Status")]
        public bool UpdateStatus { get; set; } = true;
    }
}