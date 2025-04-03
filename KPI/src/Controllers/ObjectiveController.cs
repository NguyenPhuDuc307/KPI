using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace KPISolution.Controllers
{
    [Authorize]
    public class ObjectiveController : Controller
    {
        // Tất cả các request đến Objective sẽ được chuyển hướng sang BusinessObjective
        public IActionResult Index()
        {
            return RedirectToAction("Index", "BusinessObjective");
        }

        public IActionResult Details(int id)
        {
            // Chuyển đổi int id sang Guid (BusinessObjectiveController sử dụng Guid)
            var guidId = Guid.NewGuid(); // Lý tưởng là nên có cách chuyển đổi id thực tế, nhưng vì không có dữ liệu thực nên tạm thời dùng Guid mới
            return RedirectToAction("Details", "BusinessObjective", new { id = guidId });
        }

        public IActionResult Create()
        {
            return RedirectToAction("Create", "BusinessObjective");
        }

        public IActionResult Edit(int id)
        {
            // Chuyển đổi int id sang Guid
            var guidId = Guid.NewGuid();
            return RedirectToAction("Edit", "BusinessObjective", new { id = guidId });
        }

        public IActionResult Delete(int id)
        {
            // Chuyển đổi int id sang Guid
            var guidId = Guid.NewGuid();
            return RedirectToAction("Delete", "BusinessObjective", new { id = guidId });
        }
    }
}