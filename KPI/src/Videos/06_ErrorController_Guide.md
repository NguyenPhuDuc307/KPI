# Video Hướng Dẫn: ErrorController

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về ErrorController trong hệ thống quản lý KPI. ErrorController là thành phần quan trọng, xử lý và hiển thị các lỗi trong hệ thống một cách thân thiện với người dùng, đồng thời cung cấp thông tin hữu ích cho việc gỡ lỗi.

## 2. Cấu Trúc ErrorController

ErrorController có cấu trúc đơn giản nhưng hiệu quả:

```csharp
[AllowAnonymous]
public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    // Các action methods...
}
```

**Đặc điểm quan trọng**:

-   Thuộc tính `[AllowAnonymous]` cho phép tất cả người dùng (kể cả chưa đăng nhập) đều có thể xem trang lỗi
-   Sử dụng `ILogger` để ghi nhật ký lỗi

## 3. Các Action Method Chính

### 3.1. Trang Lỗi Mặc Định

```csharp
[Route("Error")]
public IActionResult Index()
{
    var errorViewModel = new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        ErrorTitle = "Đã xảy ra lỗi",
        ErrorMessage = "Hệ thống đã gặp lỗi không xác định.",
        ShowRequestId = true
    };

    return View(errorViewModel);
}
```

-   **URL**: `/Error`
-   **Mục đích**: Trang lỗi mặc định khi không xác định được loại lỗi cụ thể
-   **Thông tin hiển thị**: RequestId để phục vụ việc gỡ lỗi

### 3.2. Lỗi 404 - Không Tìm Thấy

```csharp
[Route("Error/404")]
public IActionResult NotFound()
{
    var errorViewModel = new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        ErrorTitle = "Không tìm thấy trang",
        ErrorMessage = "Trang bạn đang tìm kiếm không tồn tại hoặc đã bị di chuyển.",
        ShowRequestId = false
    };

    _logger.LogWarning("404 error for path: {Path}", HttpContext.Request.Path);

    return View("Error", errorViewModel);
}
```

-   **URL**: `/Error/404`
-   **Mục đích**: Hiển thị khi người dùng truy cập đường dẫn không tồn tại
-   **Thông tin bổ sung**: Ghi nhật ký đường dẫn gây ra lỗi

### 3.3. Lỗi 403 - Truy Cập Bị Từ Chối

```csharp
[Route("Error/403")]
public IActionResult Forbidden()
{
    var errorViewModel = new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        ErrorTitle = "Truy cập bị từ chối",
        ErrorMessage = "Bạn không có quyền truy cập trang này.",
        ShowRequestId = false
    };

    _logger.LogWarning("403 error for user: {User}, path: {Path}",
        User.Identity?.Name ?? "unknown", HttpContext.Request.Path);

    return View("Error", errorViewModel);
}
```

-   **URL**: `/Error/403`
-   **Mục đích**: Hiển thị khi người dùng không có quyền truy cập
-   **Thông tin bổ sung**: Ghi nhật ký người dùng gặp lỗi phân quyền

### 3.4. Lỗi 500 - Lỗi Máy Chủ

```csharp
[Route("Error/500")]
public IActionResult ServerError(string errorId)
{
    var errorViewModel = new ErrorViewModel
    {
        RequestId = errorId ?? (Activity.Current?.Id ?? HttpContext.TraceIdentifier),
        ErrorTitle = "Lỗi máy chủ",
        ErrorMessage = "Đã xảy ra lỗi trong quá trình xử lý yêu cầu của bạn. Vui lòng thử lại sau.",
        ShowRequestId = true
    };

    return View("Error", errorViewModel);
}
```

-   **URL**: `/Error/500`
-   **Mục đích**: Hiển thị khi xảy ra lỗi nội bộ từ máy chủ
-   **Tham số**: `errorId` - ID của lỗi đã được ghi nhật ký

### 3.5. Xử Lý Chi Tiết Lỗi Exception

```csharp
[Route("Error/Exception")]
[ApiExplorerSettings(IgnoreApi = true)]
public IActionResult Exception(Exception exception)
{
    var errorId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

    _logger.LogError(exception, "Unhandled exception with ID {ErrorId}", errorId);

    var errorViewModel = new ErrorViewModel
    {
        RequestId = errorId,
        ErrorTitle = "Lỗi hệ thống",
        ErrorMessage = app.Environment.IsDevelopment()
            ? exception.Message
            : "Đã xảy ra lỗi nghiêm trọng. Vui lòng liên hệ quản trị viên.",
        ShowRequestId = true,
        ExceptionDetails = app.Environment.IsDevelopment() ? exception.StackTrace : null
    };

    return View("Error", errorViewModel);
}
```

-   **URL**: `/Error/Exception`
-   **Mục đích**: Xử lý các ngoại lệ chưa được bắt trong ứng dụng
-   **Đặc điểm**: Hiển thị thông tin chi tiết trong môi trường phát triển

## 4. ErrorViewModel

ErrorViewModel là đối tượng chứa thông tin lỗi để hiển thị cho người dùng:

```csharp
public class ErrorViewModel
{
    public string RequestId { get; set; }
    public string ErrorTitle { get; set; }
    public string ErrorMessage { get; set; }
    public bool ShowRequestId { get; set; }
    public string ExceptionDetails { get; set; }

    public bool HasExceptionDetails => !string.IsNullOrEmpty(ExceptionDetails);
}
```

## 5. Cấu Hình Trong Program.cs

ErrorController hoạt động nhờ vào cấu hình trong file Program.cs:

```csharp
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    // Configure production error handling
    app.UseExceptionHandler("/Error/Exception");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");

    // The default HSTS value is 30 days
    app.UseHsts();
}

// Add global error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();

        // Handle 404 errors for non-existent endpoints if no response has been sent yet
        if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
        {
            // Log the 404 error
            Log.Warning("404 error occurred for path: {Path}", context.Request.Path);

            // Rewrite path to error handler
            context.Request.Path = "/Error/404";
            await next();
        }
    }
    catch (Exception ex)
    {
        // Log the exception
        Log.Error(ex, "Unhandled exception occurred");

        // Re-throw to be handled by the exception handler middleware
        throw;
    }
});
```

## 6. View Lỗi

File Error.cshtml chứa giao diện hiển thị lỗi:

```html
@model ErrorViewModel @{ ViewData["Title"] = Model.ErrorTitle ?? "Lỗi"; }

<div class="error-container">
    <h1 class="text-danger">@Model.ErrorTitle</h1>
    <h2 class="text-danger">@Model.ErrorMessage</h2>

    @if (Model.ShowRequestId) {
    <p><strong>Request ID:</strong> <code>@Model.RequestId</code></p>
    } @if (Model.HasExceptionDetails) {
    <div class="card mt-4">
        <div class="card-header">
            <h5>Chi tiết ngoại lệ</h5>
        </div>
        <div class="card-body">
            <pre class="stacktrace">@Model.ExceptionDetails</pre>
        </div>
    </div>
    }

    <div class="mt-4">
        <a href="/" class="btn btn-primary">Quay về trang chủ</a>
    </div>
</div>
```

## 7. Xử Lý Lỗi Trong Controller Khác

Các controller khác trong hệ thống sử dụng try-catch để bắt lỗi và chuyển hướng đến ErrorController:

```csharp
[HttpGet]
public async Task<IActionResult> Details(Guid? id)
{
    try
    {
        if (id == null)
            return RedirectToAction("NotFound", "Error");

        var kpi = await _unitOfWork.KPIs.GetByIdAsync(id.Value);

        if (kpi == null)
            return RedirectToAction("NotFound", "Error");

        // Xử lý logic...
        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while retrieving KPI details for ID: {KpiId}", id);
        return RedirectToAction("Exception", "Error", new { exception = ex });
    }
}
```

## 8. Quy Trình Xử Lý Lỗi

1. **Lỗi xảy ra** trong controller hoặc service
2. **Try-catch trong controller** bắt lỗi và ghi nhật ký
3. **Chuyển hướng** đến ErrorController với thông tin lỗi
4. **ErrorController** xử lý và hiển thị trang lỗi phù hợp
5. **Thông tin lỗi** được ghi nhật ký chi tiết
6. **Người dùng** nhìn thấy thông báo lỗi thân thiện

## 9. Kỹ Thuật Gỡ Lỗi Với ErrorController

ErrorController cung cấp nhiều thông tin hữu ích cho gỡ lỗi:

-   **RequestId**: ID duy nhất cho mỗi request, dùng để tìm log
-   **Exception Details**: Chi tiết lỗi (chỉ trong môi trường phát triển)
-   **Log File**: Lỗi được ghi vào file log/kpi-.txt

## 10. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. Minh họa lỗi 404 - truy cập URL không tồn tại
2. Minh họa lỗi 403 - truy cập trang không có quyền
3. Minh họa lỗi 500 - lỗi từ máy chủ
4. Cách đọc log để gỡ lỗi
5. Cách sử dụng RequestId để theo dõi lỗi

## 11. Best Practices Xử Lý Lỗi

-   **Thông báo thân thiện**: Không hiển thị thông tin kỹ thuật cho người dùng
-   **Ghi nhật ký đầy đủ**: Ghi lại mọi thông tin cần thiết để gỡ lỗi
-   **RequestId**: Luôn tạo ID cho mỗi lỗi để dễ dàng tra cứu
-   **Quy trinh xử lý**: Có quy trình rõ ràng để phản hồi với người dùng
-   **Phân loại lỗi**: Phân biệt rõ các loại lỗi (404, 403, 500, etc.)

## 12. Tóm Tắt

ErrorController là thành phần quan trọng trong hệ thống quản lý KPI:

-   Xử lý và hiển thị lỗi một cách thân thiện với người dùng
-   Cung cấp thông tin hữu ích cho việc gỡ lỗi
-   Phân loại và xử lý các loại lỗi khác nhau
-   Ghi nhật ký chi tiết để phục vụ việc khắc phục sự cố
-   Đảm bảo trải nghiệm người dùng tốt ngay cả khi có lỗi xảy ra

## 13. Kết Luận

Qua loạt video hướng dẫn về các controller trong hệ thống quản lý KPI, chúng ta đã tìm hiểu từ các controller cốt lõi như KpiController, đến các controller hỗ trợ như ErrorController. Hy vọng series này giúp bạn hiểu rõ hơn về cấu trúc và cách hoạt động của hệ thống, từ đó có thể sử dụng, bảo trì và phát triển hệ thống một cách hiệu quả.
