using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace KPISolution.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;

    public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger, IWebHostEnvironment environment)
    {
        this._unitOfWork = unitOfWork;
        this._logger = logger;
        this._environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Get counts for dashboard cards
            var performanceIndicators = await this._unitOfWork.PerformanceIndicators.GetAllAsync();
            var successFactors = await this._unitOfWork.SuccessFactors.GetAllAsync();
            var objectives = await this._unitOfWork.Objectives.GetAllAsync();
            var departments = await this._unitOfWork.Departments.GetAllAsync();
            var resultIndicators = await this._unitOfWork.ResultIndicators.GetAllAsync();

            this.ViewBag.PerformanceIndicatorCount = performanceIndicators.Count();
            this.ViewBag.ResultIndicatorCount = resultIndicators.Count();
            this.ViewBag.TotalIndicatorCount = this.ViewBag.PerformanceIndicatorCount + this.ViewBag.ResultIndicatorCount;
            this.ViewBag.SuccessFactorCount = successFactors.Count();
            this.ViewBag.ObjectiveCount = objectives.Count();
            this.ViewBag.DepartmentCount = departments.Count();

            // Get indicator status counts
            this.ViewBag.IndicatorsOnTarget = performanceIndicators.Count(p => p.Status == IndicatorStatus.OnTarget) +
                                              resultIndicators.Count(r => r.Status == IndicatorStatus.OnTarget);
            this.ViewBag.IndicatorsAtRisk = performanceIndicators.Count(p => p.Status == IndicatorStatus.AtRisk) +
                                            resultIndicators.Count(r => r.Status == IndicatorStatus.AtRisk);
            this.ViewBag.IndicatorsBelowTarget = performanceIndicators.Count(p => p.Status == IndicatorStatus.BelowTarget) +
                                                 resultIndicators.Count(r => r.Status == IndicatorStatus.BelowTarget);
            this.ViewBag.IndicatorsNotMeasured = performanceIndicators.Count(p => p.Status == IndicatorStatus.Draft) +
                                                 resultIndicators.Count(r => r.Status == IndicatorStatus.Draft);

            // Get upcoming success factors using TargetDate only for filtering
            var upcomingSuccessFactors = successFactors
                .Where(sf => sf.TargetDate.ToUniversalTime() > DateTime.UtcNow) // Lấy những SF có TargetDate trong tương lai
                .OrderBy(sf => sf.TargetDate) // Sắp xếp theo TargetDate sớm nhất
                .Take(5)
                .Select(sf => new
                {
                    sf.Id,
                    sf.Code,
                    sf.Name,
                    // Calculate remaining days based on UtcNow
                    DaysRemaining = (int)(sf.TargetDate.ToUniversalTime() - DateTime.UtcNow).TotalDays,
                    ProgressPercentage = this.CalculateProgressPercentage(sf),
                    StatusCssClass = this.GetStatusCssClass(this.CalculateProgressPercentage(sf))
                })
                .ToList();

            this._logger.LogInformation("Found {Count} upcoming success factors", upcomingSuccessFactors.Count);
            this.ViewBag.UpcomingSuccessFactors = upcomingSuccessFactors;

            return this.View();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error loading dashboard data");
            return this.View("Error");
        }
    }

    private int CalculateProgressPercentage(SuccessFactor sf)
    {
        // Corrected: StartDate and TargetDate are not nullable
        if (sf.StartDate >= sf.TargetDate)
            return 0;

        // Use UtcNow for progress calculation as well
        var totalDays = (sf.TargetDate.ToUniversalTime() - sf.StartDate.ToUniversalTime()).TotalDays;
        var daysPassed = (DateTime.UtcNow - sf.StartDate.ToUniversalTime()).TotalDays;

        if (totalDays <= 0) return 100;
        if (daysPassed < 0) return 0; // If current time is before start date

        var percentage = (int)((daysPassed / totalDays) * 100);
        return Math.Clamp(percentage, 0, 100); // Use Clamp for safety
    }

    private string GetStatusCssClass(int progressPercentage)
    {
        return progressPercentage switch
        {
            >= 80 => "success",
            >= 60 => "warning",
            >= 40 => "info",
            _ => "danger"
        };
    }

    public IActionResult Privacy()
    {
        return this.View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier,
            StatusCode = this.HttpContext.Response?.StatusCode ?? 500,
            Path = exceptionHandlerPathFeature?.Path,
            ExceptionMessage = exceptionHandlerPathFeature?.Error?.Message,
            Title = "Đã xảy ra lỗi",
            Message = "Chúng tôi đang tìm cách khắc phục. Vui lòng thử lại sau.",
            IsProduction = !this._environment.IsDevelopment()
        };

        this._logger.LogError("Error occurred: {ErrorMessage} on path: {Path}",
            errorViewModel.ExceptionMessage,
            errorViewModel.Path);

        return this.View("~/Views/Shared/Error.cshtml", errorViewModel);
    }

    protected virtual void SetupPageTemplate()
    {
        this.ViewData["Icon"] = "bi-house-fill";
        this.ViewData["Title"] = "Dashboard";
        this.ViewData["Subtitle"] = "Tổng quan về hiệu suất";
        this.ViewData["ActiveMenu"] = "Home";
    }
}
