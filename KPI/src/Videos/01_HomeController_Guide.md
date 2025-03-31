# Video Hướng Dẫn: HomeController

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về HomeController trong hệ thống quản lý KPI. Trong video này, chúng ta sẽ tìm hiểu về trang chủ, cách hoạt động và cách nó điều hướng người dùng trong hệ thống.

## 2. Cấu Trúc HomeController

HomeController là controller cơ bản nhất trong hệ thống, phục vụ các chức năng:

-   Hiển thị trang chủ hệ thống
-   Hiển thị trang chính sách bảo mật
-   Xử lý lỗi cơ bản

```csharp
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
```

## 3. Trang Chủ - Index Action

Trang chủ là điểm khởi đầu của người dùng khi truy cập hệ thống:

-   **URL**: `/Home/Index` hoặc chỉ đơn giản là `/`
-   **Chức năng**: Hiển thị thông tin tổng quan về hệ thống KPI
-   **Quyền truy cập**: Tất cả người dùng đều có thể xem trang chủ

## 4. Trang Privacy - Privacy Action

Trang Privacy hiển thị chính sách bảo mật của hệ thống:

-   **URL**: `/Home/Privacy`
-   **Chức năng**: Cung cấp thông tin về chính sách bảo mật, quyền riêng tư
-   **Quyền truy cập**: Tất cả người dùng đều có thể xem

## 5. Xử Lý Lỗi - Error Action

Action Error xử lý và hiển thị các lỗi trong hệ thống:

-   **URL**: `/Home/Error`
-   **Chức năng**: Hiển thị thông tin về lỗi xảy ra với RequestId
-   **Lưu ý**: Action này có thuộc tính `[ResponseCache]` để đảm bảo không cache thông tin lỗi

## 6. Layout Và Navigation

Giao diện người dùng sử dụng:

-   Layout chung trong `_Layout.cshtml`
-   Thanh điều hướng với các liên kết đến trang chủ và các phần khác
-   Tích hợp với \_LoginPartial để hiển thị thông tin đăng nhập/đăng xuất

## 7. Kết Nối Với Các Controller Khác

HomeController còn đóng vai trò là điểm kết nối với các controller khác:

-   Dashboard - Bảng điều khiển tổng quan
-   CSF - Quản lý yếu tố thành công quan trọng
-   KPI - Quản lý các chỉ số hiệu suất chính
-   Và các controller khác...

## 8. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. Truy cập trang chủ
2. Xem thanh điều hướng
3. Kiểm tra trang Privacy
4. Minh họa cách xử lý lỗi đơn giản

## 9. Tóm Tắt

HomeController tuy đơn giản nhưng đóng vai trò quan trọng:

-   Là điểm khởi đầu của người dùng trong hệ thống
-   Cung cấp thông tin cơ bản về hệ thống
-   Xử lý lỗi và điều hướng người dùng

## 10. Tiếp Theo

Trong video tiếp theo, chúng ta sẽ tìm hiểu về DashboardController, nơi cung cấp các bảng điều khiển tổng quan về hiệu suất KPI trong toàn bộ tổ chức.
