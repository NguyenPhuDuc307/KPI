namespace KPISolution.Infrastructure.Routing
{
    /// <summary>
    /// Route parameter transformer that ensures case-insensitive routing
    /// </summary>
    public class IgnoreCaseParameterTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        /// Transforms the specified value of a route parameter or a query string parameter for URL generation
        /// </summary>
        /// <param name="value">The value to transform</param>
        /// <returns>The transformed value</returns>
        public string? TransformOutbound(object? value)
        {
            // Keep the value unchanged
            if (value == null) return null;

            string? stringValue = value.ToString();

            // Handle special cases for KPI types
            if (stringValue == "KeyResultIndicator")
                return "KeyResultIndicator";

            return stringValue;
        }
    }
}
