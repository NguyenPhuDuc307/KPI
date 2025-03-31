using System.Security.Claims;

namespace KPISolution.Extensions
{
    /// <summary>
    /// Extension methods for claims principal
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the user ID from claims
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal</param>
        /// <returns>The user ID as a string</returns>
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var nameIdentifierClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            return nameIdentifierClaim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets the username from claims
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal</param>
        /// <returns>The username as a string</returns>
        public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            var nameClaim = claimsPrincipal.FindFirst(ClaimTypes.Name);
            return nameClaim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets the user's email from claims
        /// </summary>
        /// <param name="claimsPrincipal">The claims principal</param>
        /// <returns>The email as a string</returns>
        public static string GetUserEmail(this ClaimsPrincipal claimsPrincipal)
        {
            var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value ?? string.Empty;
        }
    }
} 