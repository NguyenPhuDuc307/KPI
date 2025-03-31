using System.Diagnostics;
using KPISolution.Extensions;
using KPISolution.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace KPISolution.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        SetupPageTemplate(
            title: "KPI Management System",
            subtitle: "Hệ thống quản lý hiệu suất công việc",
            icon: "bi-speedometer2",
            showButtons: false);

        return View();
    }

    public IActionResult Privacy()
    {
        SetupPageTemplate(
            title: "Chính sách bảo mật",
            subtitle: "Thông tin về cách chúng tôi bảo vệ dữ liệu của bạn",
            icon: "bi-shield-lock");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
