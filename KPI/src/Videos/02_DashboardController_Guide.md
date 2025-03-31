# Video Hướng Dẫn: DashboardController

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về DashboardController trong hệ thống quản lý KPI. DashboardController là thành phần trung tâm của hệ thống, cung cấp các bảng điều khiển tổng quan trực quan để theo dõi hiệu suất KPI trong tổ chức.

## 2. Cấu Trúc DashboardController

DashboardController quản lý nhiều loại bảng điều khiển khác nhau:

-   Dashboard tổng quan cho lãnh đạo (Executive)
-   Dashboard theo phòng ban (Department)
-   Dashboard tùy chỉnh cho người dùng (Custom)

```csharp
[Authorize]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Các action methods...
}
```

## 3. Executive Dashboard

Dashboard cho cấp quản lý cao cấp, cung cấp cái nhìn tổng quan về toàn bộ tổ chức:

```csharp
[HttpGet]
[Authorize(Roles = "Executive,Administrator")]
public async Task<IActionResult> Executive()
{
    // Tạo viewmodel và lấy dữ liệu...
}
```

-   **URL**: `/Dashboard/Executive`
-   **Đối tượng sử dụng**: Lãnh đạo cấp cao, quản trị viên
-   **Chức năng chính**:
    -   Hiển thị tổng quan về KRI (Key Result Indicators)
    -   Trạng thái các CSF (Critical Success Factors)
    -   Hiệu suất theo phòng ban
    -   Phân bố trạng thái KPI

## 4. Department Dashboard

Dashboard theo từng phòng ban, tập trung vào KPI của một phòng ban cụ thể:

```csharp
[HttpGet]
public async Task<IActionResult> Department(Guid id)
{
    // Lấy thông tin phòng ban và KPI liên quan...
}
```

-   **URL**: `/Dashboard/Department/{id}`
-   **Đối tượng sử dụng**: Trưởng phòng, nhân viên trong phòng
-   **Chức năng chính**:
    -   Hiển thị các KPI của phòng ban
    -   Tổng quan về mức độ hoàn thành
    -   Liên kết đến các CSF liên quan
    -   Thống kê theo trạng thái KPI

## 5. Custom Dashboard

Dashboard cá nhân hóa cho từng người dùng:

```csharp
[HttpGet]
public async Task<IActionResult> Custom(Guid? id)
{
    // Lấy thông tin dashboard tùy chỉnh...
}
```

-   **URL**: `/Dashboard/Custom/{id}`
-   **Đối tượng sử dụng**: Tất cả người dùng
-   **Chức năng chính**:
    -   Tùy chỉnh các KPI hiển thị
    -   Sắp xếp bố cục theo ý muốn
    -   Lưu nhiều dashboard khác nhau

## 6. Tạo Và Quản Lý Custom Dashboard

### 6.1. Tạo Mới Dashboard

```csharp
[HttpGet]
public IActionResult Create()
{
    // Hiển thị form tạo dashboard...
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(CustomDashboardViewModel viewModel)
{
    // Xử lý tạo dashboard...
}
```

-   **URL**: `/Dashboard/Create`
-   **Chức năng**: Tạo dashboard tùy chỉnh mới với tên, mô tả và thiết lập ban đầu

### 6.2. Chỉnh Sửa Dashboard

```csharp
[HttpGet]
public async Task<IActionResult> Edit(Guid id)
{
    // Hiển thị form chỉnh sửa...
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Guid id, CustomDashboardViewModel viewModel)
{
    // Xử lý cập nhật dashboard...
}
```

-   **URL**: `/Dashboard/Edit/{id}`
-   **Chức năng**: Chỉnh sửa thông tin cơ bản của dashboard

### 6.3. Xóa Dashboard

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(Guid id)
{
    // Xử lý xóa dashboard...
}
```

-   **URL**: `/Dashboard/Delete/{id}`
-   **Chức năng**: Xóa dashboard tùy chỉnh

## 7. Tùy Chỉnh Bố Cục Dashboard

### 7.1. Lưu Layout

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SaveLayout(Guid id, [FromBody] DashboardLayoutViewModel layout)
{
    // Lưu cấu hình bố cục...
}
```

-   **Chức năng**: Lưu vị trí các widget trên dashboard
-   **Công nghệ**: Sử dụng AJAX để lưu bố cục không cần tải lại trang

### 7.2. Thêm Item Vào Dashboard

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddItem(Guid id, [FromBody] DashboardItemViewModel item)
{
    // Thêm widget mới vào dashboard...
}
```

-   **Chức năng**: Thêm widget KPI, biểu đồ hoặc chỉ số khác vào dashboard

### 7.3. Xóa Item Khỏi Dashboard

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RemoveItem(Guid id, Guid itemId)
{
    // Xóa widget khỏi dashboard...
}
```

-   **Chức năng**: Xóa widget không cần thiết khỏi dashboard

## 8. Kiểm Tra Quyền Truy Cập

Dashboard Controller có hệ thống phân quyền phức tạp:

```csharp
private async Task<bool> UserHasAccessToDepartment(ClaimsPrincipal user, Guid departmentId)
{
    // Kiểm tra người dùng có quyền với phòng ban...
}
```

-   **Mục đích**: Đảm bảo người dùng chỉ xem được dashboard thuộc quyền hạn của họ
-   **Cơ chế**: Dựa trên vai trò, phòng ban và quyền cụ thể

## 9. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. Xem Executive Dashboard
2. Truy cập Department Dashboard của phòng Marketing
3. Tạo Custom Dashboard mới
4. Thêm widget KPI vào dashboard
5. Sắp xếp bố cục dashboard
6. Lưu và chia sẻ dashboard

## 10. Tổ Chức Dữ Liệu

DashboardController làm việc với nhiều loại dữ liệu khác nhau:

-   **ExecutiveDashboardViewModel**: Chứa dữ liệu tổng quan cho lãnh đạo
-   **DepartmentDashboardViewModel**: Dữ liệu theo phòng ban
-   **CustomDashboardViewModel**: Cấu hình dashboard tùy chỉnh
-   **DashboardLayoutViewModel**: Lưu thông tin bố cục
-   **DashboardItemViewModel**: Mô tả các widget trên dashboard

## 11. Tóm Tắt

DashboardController là thành phần trung tâm của hệ thống quản lý KPI:

-   Cung cấp nhiều góc nhìn khác nhau về hiệu suất
-   Cho phép tùy chỉnh theo nhu cầu của từng người dùng
-   Hỗ trợ đa dạng đối tượng người dùng từ lãnh đạo đến nhân viên

## 12. Tiếp Theo

Trong video tiếp theo, chúng ta sẽ tìm hiểu về CsfController, nơi quản lý các Yếu tố thành công then chốt (Critical Success Factors) - nền tảng để định hướng các KPI trong tổ chức.
