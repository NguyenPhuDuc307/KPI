namespace KPISolution.Controllers
{
    /// <summary>
    /// Controller for the landing page
    /// </summary>
    [AllowAnonymous] // Cho phép truy cập mà không cần xác thực
    public class LandingController : Controller
    {
        private readonly ILogger<LandingController> _logger;

        public LandingController(ILogger<LandingController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Display the landing page
        /// </summary>
        public IActionResult Index()
        {
            this._logger.LogInformation("Landing page accessed - Public marketing page displayed");
            return this.View();
        }
    }
}