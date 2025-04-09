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
        _unitOfWork = unitOfWork;
        _logger = logger;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Get counts for dashboard cards
            var performanceIndicators = await _unitOfWork.PerformanceIndicators.GetAllAsync();
            var successFactors = await _unitOfWork.SuccessFactors.GetAllAsync();
            var objectives = await _unitOfWork.Objectives.GetAllAsync();
            var departments = await _unitOfWork.Departments.GetAllAsync();

            ViewBag.PerformanceIndicatorCount = performanceIndicators.Count();
            ViewBag.SuccessFactorCount = successFactors.Count();
            ViewBag.ObjectiveCount = objectives.Count();
            ViewBag.DepartmentCount = departments.Count();

            // Get indicator status counts
            ViewBag.IndicatorsOnTarget = performanceIndicators.Count(p => p.Status == IndicatorStatus.OnTarget) +
                                       (await _unitOfWork.ResultIndicators.GetAllAsync(r => r.Status == IndicatorStatus.OnTarget)).Count();
            ViewBag.IndicatorsAtRisk = performanceIndicators.Count(p => p.Status == IndicatorStatus.AtRisk) +
                                     (await _unitOfWork.ResultIndicators.GetAllAsync(r => r.Status == IndicatorStatus.AtRisk)).Count();
            ViewBag.IndicatorsBelowTarget = performanceIndicators.Count(p => p.Status == IndicatorStatus.BelowTarget) +
                                          (await _unitOfWork.ResultIndicators.GetAllAsync(r => r.Status == IndicatorStatus.BelowTarget)).Count();
            ViewBag.IndicatorsNotMeasured = performanceIndicators.Count(p => p.Status == IndicatorStatus.Draft) +
                                          (await _unitOfWork.ResultIndicators.GetAllAsync(r => r.Status == IndicatorStatus.Draft)).Count();

            // Get upcoming success factors
            var upcomingSuccessFactors = successFactors
                .Where(sf => sf.TargetDate > DateTime.Now)
                .OrderBy(sf => sf.TargetDate)
                .Take(3)
                .Select(sf => new
                {
                    sf.Code,
                    sf.Name,
                    DaysRemaining = (int)(sf.TargetDate - DateTime.Now).TotalDays,
                    ProgressPercentage = CalculateProgressPercentage(sf),
                    StatusCssClass = GetStatusCssClass(CalculateProgressPercentage(sf))
                })
                .ToList();

            ViewBag.UpcomingSuccessFactors = upcomingSuccessFactors;

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            return View("Error");
        }
    }

    private int CalculateProgressPercentage(SuccessFactor sf)
    {
        var totalDays = (sf.TargetDate - sf.StartDate).TotalDays;
        var daysPassed = (DateTime.Now - sf.StartDate).TotalDays;

        if (totalDays <= 0)
            return 100;

        var percentage = (int)((daysPassed / totalDays) * 100);
        return Math.Min(percentage, 100);
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
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature =
            HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        var errorViewModel = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = HttpContext.Response?.StatusCode ?? 500,
            Path = exceptionHandlerPathFeature?.Path,
            ExceptionMessage = exceptionHandlerPathFeature?.Error?.Message,
            Title = "Đã xảy ra lỗi",
            Message = "Chúng tôi đang tìm cách khắc phục. Vui lòng thử lại sau.",
            IsProduction = !_environment.IsDevelopment()
        };

        _logger.LogError("Error occurred: {ErrorMessage} on path: {Path}",
            errorViewModel.ExceptionMessage,
            errorViewModel.Path);

        return View("~/Views/Shared/Error.cshtml", errorViewModel);
    }

    protected virtual void SetupPageTemplate()
    {
        ViewData["Icon"] = "bi-house-fill";
        ViewData["Title"] = "Dashboard";
        ViewData["Subtitle"] = "Tổng quan về hiệu suất";
        ViewData["ActiveMenu"] = "Home";
    }
}
