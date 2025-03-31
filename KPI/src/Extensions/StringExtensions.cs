using System.Text.RegularExpressions;

namespace KPISolution.Extensions
{
    /// <summary>
    /// Extension methods for string operations
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits a camel case or pascal case string into separate words
        /// </summary>
        /// <param name="input">The camel case or pascal case string to split</param>
        /// <returns>A string with spaces between words</returns>
        public static string SplitCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Add a space before each uppercase letter that is not at the beginning
            // and is not followed by another uppercase letter
            var result = Regex.Replace(input, "(?<!^)(?<![A-Z])([A-Z])", " $1");

            // Handle abbreviations (consecutive uppercase letters)
            result = Regex.Replace(result, "([A-Z])([A-Z])(?=[a-z])", "$1 $2");

            return result;
        }

        /// <summary>
        /// Converts a string to title case (first letter of each word capitalized)
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <returns>Title cased string</returns>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var textInfo = System.Globalization.CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input.ToLower());
        }

        /// <summary>
        /// Truncates a string to a specified length and adds an ellipsis if truncated
        /// </summary>
        /// <param name="input">The string to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="addEllipsis">Whether to add an ellipsis (...) if truncated</param>
        /// <returns>Truncated string</returns>
        public static string Truncate(this string input, int maxLength, bool addEllipsis = true)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
                return input;

            var truncated = input.Substring(0, maxLength).Trim();
            return addEllipsis ? truncated + "..." : truncated;
        }

        /// <summary>
        /// Converts a string to a URL-friendly slug
        /// </summary>
        /// <param name="input">The string to convert</param>
        /// <returns>URL-friendly slug</returns>
        public static string ToSlug(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Replace spaces with hyphens
            var slug = Regex.Replace(input.ToLower(), @"\s+", "-");

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens
            slug = Regex.Replace(slug, @"-{2,}", "-");

            // Trim hyphens from start and end
            return slug.Trim('-');
        }

        /// <summary>
        /// Checks if a string contains another string (case-insensitive)
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="toCheck">String to check for</param>
        /// <returns>True if contained, false otherwise</returns>
        public static bool ContainsIgnoreCase(this string source, string toCheck)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(toCheck))
                return false;

            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}