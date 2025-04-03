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
    /// Represents the performance trend of a KPI over time
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
    /// Type of period for measurements
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// Daily period
        /// </summary>
        [Display(Name = "Daily")]
        Daily = 0,

        /// <summary>
        /// Weekly period
        /// </summary>
        [Display(Name = "Weekly")]
        Weekly = 1,

        /// <summary>
        /// Monthly period
        /// </summary>
        [Display(Name = "Monthly")]
        Monthly = 2,

        /// <summary>
        /// Quarterly period
        /// </summary>
        [Display(Name = "Quarterly")]
        Quarterly = 3,

        /// <summary>
        /// Yearly period
        /// </summary>
        [Display(Name = "Yearly")]
        Yearly = 4
    }
}
