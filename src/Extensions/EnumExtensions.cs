using System.Reflection;

namespace KPISolution.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());

            if (field == null)
            {
                return enumValue.ToString();
            }

            var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? enumValue.ToString();
        }

        public static string GetStatusBadgeClass(this SuccessFactorStatus status)
        {
            return status switch
            {
                SuccessFactorStatus.NotStarted => "bg-secondary",
                SuccessFactorStatus.InProgress => "bg-primary",
                SuccessFactorStatus.OnTrack => "bg-success",
                SuccessFactorStatus.AtRisk => "bg-warning",
                SuccessFactorStatus.OffTrack => "bg-danger",
                SuccessFactorStatus.Completed => "bg-info",
                SuccessFactorStatus.Cancelled => "bg-dark",
                _ => "bg-secondary",
            };
        }

        public static string GetStatusBadgeClass(this IndicatorStatus status)
        {
            return status switch
            {
                IndicatorStatus.Draft => "bg-secondary",
                IndicatorStatus.Active => "bg-primary",
                IndicatorStatus.UnderReview => "bg-info",
                IndicatorStatus.Approved => "bg-success",
                IndicatorStatus.OnTarget => "bg-success",
                IndicatorStatus.AtRisk => "bg-warning",
                IndicatorStatus.BelowTarget => "bg-danger",
                IndicatorStatus.Archived => "bg-dark",
                IndicatorStatus.Deprecated => "bg-dark",
                _ => "bg-secondary",
            };
        }

        public static string GetProgressBarColorClass(this decimal progress)
        {
            return progress switch
            {
                < 25 => "bg-danger",
                < 50 => "bg-warning",
                < 75 => "bg-info",
                _ => "bg-success",
            };
        }
    }
}
