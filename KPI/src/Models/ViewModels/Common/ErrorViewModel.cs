namespace KPISolution.Models.ViewModels
{
    /// <summary>
    /// View model for error pages
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the request ID
        /// </summary>
        public string? RequestId { get; init; }

        /// <summary>
        /// Gets a value indicating whether request ID should be shown
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

        /// <summary>
        /// Gets or sets the HTTP status code
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// Gets or sets the error title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the original path that caused the error
        /// </summary>
        public string? Path { get; init; }

        /// <summary>
        /// Gets or sets the original query string that caused the error
        /// </summary>
        public string? QueryString { get; init; }

        /// <summary>
        /// Gets or sets the exception message (only shown in Development)
        /// </summary>
        public string? ExceptionMessage { get; init; }

        /// <summary>
        /// Gets or sets the detailed error message
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// Gets or sets the stack trace (only shown in Development)
        /// </summary>
        public string? StackTrace { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the application is running in Production mode
        /// </summary>
        public bool IsProduction { get; init; }
    }
}