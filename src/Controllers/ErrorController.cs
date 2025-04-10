using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for handling errors and exceptions
    /// </summary>
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorController"/> class
        /// </summary>
        /// <param name="logger">The logger instance</param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Handle HTTP status code errors
        /// </summary>
        /// <param name="statusCode">HTTP status code</param>
        /// <returns>View with error details</returns>
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = this.HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            var viewModel = new ErrorViewModel
            {
                StatusCode = statusCode,
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
                Path = statusCodeResult?.OriginalPath,
                QueryString = statusCodeResult?.OriginalQueryString,
            };

            switch (statusCode)
            {
                case 404:
                    viewModel.Title = "Page not found";
                    viewModel.Message = "The requested page could not be found.";
                    this._logger.LogWarning("404 error occurred. Path: {Path}, QueryString: {QueryString}",
                        viewModel.Path, viewModel.QueryString);
                    break;
                case 500:
                    viewModel.Title = "Server error";
                    viewModel.Message = "An internal server error occurred. Please try again later.";
                    this._logger.LogError("500 error occurred. Path: {Path}, QueryString: {QueryString}",
                        viewModel.Path, viewModel.QueryString);
                    break;
                default:
                    viewModel.Title = "Error";
                    viewModel.Message = "An error occurred while processing your request.";
                    this._logger.LogWarning("{StatusCode} error occurred. Path: {Path}, QueryString: {QueryString}",
                        statusCode, viewModel.Path, viewModel.QueryString);
                    break;
            }

            // Determine if request is from localhost
            var host = this.HttpContext.Request.Host.Value;
            this.ViewData["IsLocal"] = host != null && (host.StartsWith("localhost") || host.StartsWith("127.0.0.1"));

            return this.View("Error", viewModel);
        }

        /// <summary>
        /// Handles exceptions
        /// </summary>
        /// <returns>View with error details</returns>
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            var viewModel = new ErrorViewModel
            {
                StatusCode = 500,
                Title = "Error",
                Message = "An unexpected error occurred. Please try again later.",
                RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
                Path = exceptionHandlerPathFeature?.Path,
                ExceptionMessage = exception?.Message,
            };

            this._logger.LogError(exception, "Unhandled exception occurred. Path: {Path}", viewModel.Path);

            // Determine if request is from localhost
            var host = this.HttpContext.Request.Host.Value;
            this.ViewData["IsLocal"] = host != null && (host.StartsWith("localhost") || host.StartsWith("127.0.0.1"));

            return this.View(viewModel);
        }
    }
}
