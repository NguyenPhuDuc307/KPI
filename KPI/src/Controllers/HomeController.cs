using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace KPISolution.Controllers;

[Authorize]
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
    {
        this._logger = logger;
        this._environment = environment;
    }

    public IActionResult Index()
    {
        this.SetupPageTemplate(
            title: "Dashboard",
            subtitle: "Tổng quan hiệu suất và chỉ số KPI",
            icon: "bi-speedometer2",
            showButtons: false);

        return View();
    }

    public IActionResult Privacy()
    {
        this.SetupPageTemplate(
            title: "Chính sách bảo mật",
            subtitle: "Thông tin về cách chúng tôi bảo vệ dữ liệu của bạn",
            icon: "bi-shield-lock");

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
}
