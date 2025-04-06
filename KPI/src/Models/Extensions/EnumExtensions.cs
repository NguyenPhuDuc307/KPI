using System.ComponentModel.DataAnnotations;
using System.Reflection;
using KPISolution.Models.Enums;
using KPISolution.Models.Enums.SuccessFactor;

namespace KPISolution.Models.Extensions
{
    /// <summary>
    /// Extension methods for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the display name attribute value for an enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The display name attribute value if found; otherwise, the enum value name.</returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attribute = field.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }

        /// <summary>
        /// Gets the display name for a nullable enum value.
        /// </summary>
        /// <param name="value">The nullable enum value.</param>
        /// <returns>The display name if the enum has a value; otherwise, an empty string.</returns>
        public static string GetDisplayName<T>(this T? value) where T : struct, Enum
        {
            return value.HasValue ? GetDisplayName((Enum)(object)value.Value) : string.Empty;
        }

        /// <summary>
        /// Gets the CSS class for a badge based on the SuccessFactorStatus.
        /// </summary>
        /// <param name="status">The SuccessFactorStatus value.</param>
        /// <returns>A Bootstrap CSS class for a badge.</returns>
        public static string GetStatusBadgeClass(this SuccessFactorStatus status)
        {
            return status switch
            {
                SuccessFactorStatus.NotStarted => "bg-secondary",
                SuccessFactorStatus.InProgress => "bg-primary",
                SuccessFactorStatus.OnTrack => "bg-success",
                SuccessFactorStatus.AtRisk => "bg-warning text-dark",
                SuccessFactorStatus.OffTrack => "bg-danger",
                SuccessFactorStatus.Completed => "bg-success",
                SuccessFactorStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };
        }


        /// <summary>
        /// Gets the CSS class for a badge based on the RiskLevel.
        /// </summary>
        /// <param name="riskLevel">The RiskLevel value.</param>
        /// <returns>A Bootstrap CSS class for a badge.</returns>
        public static string GetRiskBadgeClass(this RiskLevel riskLevel)
        {
            return riskLevel switch
            {
                RiskLevel.None => "bg-light text-dark",
                RiskLevel.Negligible => "bg-info",
                RiskLevel.Low => "bg-success",
                RiskLevel.Medium => "bg-primary",
                RiskLevel.High => "bg-warning text-dark",
                RiskLevel.Critical => "bg-danger",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Gets the CSS class for a badge based on a nullable RiskLevel.
        /// </summary>
        /// <param name="riskLevel">The nullable RiskLevel value.</param>
        /// <returns>A Bootstrap CSS class for a badge, or "bg-secondary" if null.</returns>
        public static string GetRiskBadgeClass(this RiskLevel? riskLevel)
        {
            return riskLevel.HasValue ? riskLevel.Value.GetRiskBadgeClass() : "bg-secondary";
        }

        /// <summary>
        /// Gets the CSS class for a badge based on the PriorityLevel.
        /// </summary>
        /// <param name="priority">The PriorityLevel value.</param>
        /// <returns>A Bootstrap CSS class for a badge.</returns>
        public static string GetPriorityBadgeClass(this PriorityLevel priority)
        {
            return priority switch
            {
                PriorityLevel.Low => "bg-success",
                PriorityLevel.Medium => "bg-primary",
                PriorityLevel.High => "bg-warning text-dark",
                PriorityLevel.Critical => "bg-danger",
                _ => "bg-secondary"
            };
        }

        /// <summary>
        /// Gets the CSS class for a badge based on a nullable PriorityLevel.
        /// </summary>
        /// <param name="priority">The nullable PriorityLevel value.</param>
        /// <returns>A Bootstrap CSS class for a badge, or "bg-secondary" if null.</returns>
        public static string GetPriorityBadgeClass(this PriorityLevel? priority)
        {
            return priority.HasValue ? priority.Value.GetPriorityBadgeClass() : "bg-secondary";
        }
    }
}