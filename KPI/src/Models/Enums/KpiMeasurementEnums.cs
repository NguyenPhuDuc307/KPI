using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.Enums
{
    /// <summary>
    /// Represents the category of a KPI
    /// </summary>
    public enum KpiCategory
    {
        /// <summary>
        /// Financial indicators (e.g., revenue, profit margins)
        /// </summary>
        [Display(Name = "Financial")]
        Financial = 1,

        /// <summary>
        /// Customer-related indicators (e.g., satisfaction, retention)
        /// </summary>
        [Display(Name = "Customer")]
        Customer = 2,

        /// <summary>
        /// Internal business process indicators
        /// </summary>
        [Display(Name = "Operational")]
        Operational = 3,

        /// <summary>
        /// Learning and growth indicators
        /// </summary>
        [Display(Name = "Learning & Growth")]
        LearningAndGrowth = 4,

        /// <summary>
        /// Environmental indicators
        /// </summary>
        [Display(Name = "Environmental")]
        Environmental = 5,

        /// <summary>
        /// Social responsibility indicators
        /// </summary>
        [Display(Name = "Social")]
        Social = 6,

        /// <summary>
        /// Governance indicators
        /// </summary>
        [Display(Name = "Governance")]
        Governance = 7,

        /// <summary>
        /// Quality indicators
        /// </summary>
        [Display(Name = "Quality")]
        Quality = 8,

        /// <summary>
        /// Innovation indicators
        /// </summary>
        [Display(Name = "Innovation")]
        Innovation = 9,

        /// <summary>
        /// Productivity indicators
        /// </summary>
        [Display(Name = "Productivity")]
        Productivity = 10,

        /// <summary>
        /// Human resources indicators
        /// </summary>
        [Display(Name = "Human Resources")]
        HumanResources = 11,

        /// <summary>
        /// IT performance indicators
        /// </summary>
        [Display(Name = "IT")]
        IT = 12,

        /// <summary>
        /// Safety indicators
        /// </summary>
        [Display(Name = "Safety")]
        Safety = 13,

        /// <summary>
        /// Project management indicators
        /// </summary>
        [Display(Name = "Project")]
        Project = 14,

        /// <summary>
        /// Risk management indicators
        /// </summary>
        [Display(Name = "Risk")]
        Risk = 15,

        /// <summary>
        /// Other indicators not fitting into the categories above
        /// </summary>
        [Display(Name = "Other")]
        Other = 16
    }

    /// <summary>
    /// Represents the frequency of measurement for a KPI
    /// </summary>
    public enum MeasurementFrequency
    {
        /// <summary>
        /// Daily measurement
        /// </summary>
        [Display(Name = "Daily")]
        Daily = 1,

        /// <summary>
        /// Weekly measurement
        /// </summary>
        [Display(Name = "Weekly")]
        Weekly = 2,

        /// <summary>
        /// Biweekly measurement
        /// </summary>
        [Display(Name = "Biweekly")]
        Biweekly = 3,

        /// <summary>
        /// Monthly measurement
        /// </summary>
        [Display(Name = "Monthly")]
        Monthly = 4,

        /// <summary>
        /// Quarterly measurement
        /// </summary>
        [Display(Name = "Quarterly")]
        Quarterly = 5,

        /// <summary>
        /// Semi-annual measurement
        /// </summary>
        [Display(Name = "Semiannually")]
        Semiannually = 6,

        /// <summary>
        /// Annual measurement
        /// </summary>
        [Display(Name = "Annually")]
        Annually = 7,

        /// <summary>
        /// Custom period
        /// </summary>
        [Display(Name = "Custom")]
        Custom = 99
    }

    /// <summary>
    /// Represents the measurement direction for a KPI
    /// </summary>
    public enum MeasurementDirection
    {
        /// <summary>
        /// Higher values are better (e.g., profit, customer satisfaction)
        /// </summary>
        [Display(Name = "Higher is Better")]
        HigherIsBetter = 1,

        /// <summary>
        /// Lower values are better (e.g., costs, defects)
        /// </summary>
        [Display(Name = "Lower is Better")]
        LowerIsBetter = 2,

        /// <summary>
        /// Values closer to target are better (e.g., temperature, inventory levels)
        /// </summary>
        [Display(Name = "Target is Best")]
        TargetIsBest = 3,

        /// <summary>
        /// Values within a range are best
        /// </summary>
        [Display(Name = "Range is Best")]
        RangeIsBest = 4
    }

    /// <summary>
    /// Status of a KPI
    /// </summary>
    public enum KpiStatus
    {
        /// <summary>
        /// KPI is in draft mode, not yet published
        /// </summary>
        [Display(Name = "Draft")]
        Draft = 1,

        /// <summary>
        /// KPI is active and being tracked
        /// </summary>
        [Display(Name = "Active")]
        Active = 2,

        /// <summary>
        /// KPI is under review
        /// </summary>
        [Display(Name = "Under Review")]
        UnderReview = 3,

        /// <summary>
        /// KPI is approved
        /// </summary>
        [Display(Name = "Approved")]
        Approved = 4,

        /// <summary>
        /// KPI is archived and no longer in use
        /// </summary>
        [Display(Name = "Archived")]
        Archived = 5,

        /// <summary>
        /// KPI is deprecated
        /// </summary>
        [Display(Name = "Deprecated")]
        Deprecated = 6,

        /// <summary>
        /// KPI is on target
        /// </summary>
        [Display(Name = "On Target")]
        OnTarget = 7,

        /// <summary>
        /// KPI is at risk
        /// </summary>
        [Display(Name = "At Risk")]
        AtRisk = 8,

        /// <summary>
        /// KPI is below target
        /// </summary>
        [Display(Name = "Below Target")]
        BelowTarget = 9
    }

    /// <summary>
    /// Performance trend indicating how a KPI is trending over time
    /// </summary>
    public enum PerformanceTrend
    {
        /// <summary>
        /// KPI is improving
        /// </summary>
        [Display(Name = "Improving")]
        Improving = 1,

        /// <summary>
        /// KPI is stable
        /// </summary>
        [Display(Name = "Stable")]
        Stable = 2,

        /// <summary>
        /// KPI is declining
        /// </summary>
        [Display(Name = "Declining")]
        Declining = 3,

        /// <summary>
        /// KPI is showing mixed results
        /// </summary>
        [Display(Name = "Mixed")]
        Mixed = 4,

        /// <summary>
        /// Not enough data to determine trend
        /// </summary>
        [Display(Name = "Not Enough Data")]
        NotEnoughData = 5
    }

    /// <summary>
    /// Type of indicator (Leading or Lagging)
    /// </summary>
    public enum IndicatorType
    {
        /// <summary>
        /// Leading indicators predict future performance
        /// </summary>
        [Display(Name = "Leading")]
        Leading = 1,

        /// <summary>
        /// Lagging indicators measure past performance
        /// </summary>
        [Display(Name = "Lagging")]
        Lagging = 2,

        /// <summary>
        /// Real-time indicators measure current performance
        /// </summary>
        [Display(Name = "Real-Time")]
        RealTime = 3
    }
}
