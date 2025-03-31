# Video Hướng Dẫn: CsfController

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về CsfController trong hệ thống quản lý KPI. CsfController đóng vai trò cực kỳ quan trọng, quản lý các Yếu tố thành công then chốt (Critical Success Factors - CSF) - những yếu tố quyết định thành công của tổ chức và định hướng cho việc xây dựng các KPI.

## 2. Cấu Trúc CsfController

CsfController quản lý mọi khía cạnh liên quan đến CSF:

```csharp
[Authorize]
public class CsfController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CsfController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public CsfController(
        IUnitOfWork unitOfWork,
        ILogger<CsfController> logger,
        IMapper mapper,
        IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    // Các action methods...
}
```

## 3. Hiển Thị Danh Sách CSF

Hiển thị danh sách các CSF với các tùy chọn lọc và sắp xếp:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
public async Task<IActionResult> Index(CsfFilterViewModel filter, int page = 1)
{
    // Lấy danh sách CSF với phân trang và lọc...
}
```

-   **URL**: `/CSF/Index` hoặc `/CSF`
-   **Quyền truy cập**: Người dùng có quyền xem CSF
-   **Chức năng chính**:
    -   Hiển thị danh sách CSF có phân trang
    -   Lọc theo nhiều tiêu chí (tên, mã, danh mục, ưu tiên, trạng thái)
    -   Sắp xếp kết quả theo các trường khác nhau
    -   Hiển thị trạng thái và tiến độ CSF

## 4. Xem Chi Tiết CSF

Hiển thị thông tin chi tiết về một CSF cụ thể:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewCsfs)]
public async Task<IActionResult> Details(Guid? id)
{
    // Lấy chi tiết CSF và dữ liệu liên quan...
}
```

-   **URL**: `/CSF/Details/{id}`
-   **Quyền truy cập**: Người dùng có quyền xem CSF
-   **Chức năng chính**:
    -   Hiển thị thông tin chi tiết về CSF
    -   Danh sách KPI liên kết
    -   Lịch sử cập nhật tiến độ
    -   Biểu đồ tiến độ và thời gian còn lại

## 5. Tạo Mới CSF

Tạo mới một Yếu tố thành công then chốt:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> Create()
{
    // Hiển thị form tạo CSF...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> Create(CreateCsfViewModel viewModel)
{
    // Xử lý tạo CSF...
}
```

-   **URL**: `/CSF/Create`
-   **Quyền truy cập**: Người dùng có quyền quản lý CSF
-   **Thông tin cần nhập**:
    -   Tên và mã CSF
    -   Mô tả chi tiết
    -   Danh mục CSF
    -   Mức độ ưu tiên
    -   Phòng ban chịu trách nhiệm
    -   Mục tiêu kinh doanh liên quan
    -   Ngày bắt đầu và ngày mục tiêu
    -   Mức độ rủi ro

## 6. Chỉnh Sửa CSF

Chỉnh sửa thông tin của CSF đã tạo:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> Edit(Guid? id)
{
    // Hiển thị form chỉnh sửa CSF...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> Edit(Guid id, EditCsfViewModel viewModel)
{
    // Xử lý cập nhật CSF...
}
```

-   **URL**: `/CSF/Edit/{id}`
-   **Quyền truy cập**: Người dùng có quyền quản lý CSF
-   **Chức năng**: Cập nhật mọi thông tin của CSF ngoại trừ mã CSF

## 7. Xóa CSF

Xóa một CSF khỏi hệ thống:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteCsfs)]
public async Task<IActionResult> Delete(Guid? id)
{
    // Hiển thị xác nhận xóa...
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteCsfs)]
public async Task<IActionResult> DeleteConfirmed(Guid id)
{
    // Xử lý xóa CSF...
}
```

-   **URL**: `/CSF/Delete/{id}`
-   **Quyền truy cập**: Người dùng có quyền xóa CSF
-   **Lưu ý**: Xóa CSF sẽ ảnh hưởng đến các KPI liên kết

## 8. Cập Nhật Tiến Độ CSF

Cập nhật tiến độ thực hiện của CSF:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> UpdateProgress(Guid? id)
{
    // Hiển thị form cập nhật tiến độ...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageCsfs)]
public async Task<IActionResult> UpdateProgress(CsfProgressUpdateViewModel viewModel)
{
    // Xử lý cập nhật tiến độ...
}
```

-   **URL**: `/CSF/UpdateProgress/{id}`
-   **Quyền truy cập**: Người dùng có quyền quản lý CSF
-   **Thông tin cập nhật**:
    -   Phần trăm hoàn thành hiện tại
    -   Ghi chú về tiến độ
    -   Vấn đề cần chú ý
    -   Hành động khắc phục
    -   Đánh dấu cần chú ý hay không

## 9. Các Hàm Hỗ Trợ

CsfController có nhiều hàm hỗ trợ để xử lý logic phức tạp:

### 9.1. Lấy Dữ Liệu Cho Dropdown Lists

```csharp
private async Task<List<SelectListItem>> GetDepartmentSelectList()
{
    // Lấy danh sách phòng ban cho dropdown
}

private async Task<List<SelectListItem>> GetBusinessObjectiveSelectList()
{
    // Lấy danh sách mục tiêu kinh doanh cho dropdown
}

private async Task<List<SelectListItem>> GetKpiSelectList()
{
    // Lấy danh sách KPI cho dropdown
}
```

### 9.2. Sắp Xếp Dữ Liệu

```csharp
private IQueryable<CriticalSuccessFactor> ApplySorting(
    IQueryable<CriticalSuccessFactor> query,
    string? sortBy,
    string? sortDirection)
{
    // Sắp xếp dữ liệu theo các tiêu chí
}
```

### 9.3. Tính Toán Trạng Thái

```csharp
private bool IsOnTrack(int progressPercentage, DateTime startDate, DateTime targetDate)
{
    // Tính toán CSF có đúng tiến độ không
}

private int CalculateTimeElapsedPercentage(DateTime startDate, DateTime targetDate)
{
    // Tính phần trăm thời gian đã trôi qua
}
```

### 9.4. Định Dạng CSS Cho Trạng Thái

```csharp
private string GetStatusCssClass(CSFStatus status)
{
    // Trả về CSS class cho mỗi trạng thái
}

private string GetRiskLevelCssClass(RiskLevel riskLevel)
{
    // Trả về CSS class cho mỗi mức độ rủi ro
}

private string GetProgressCssClass(int progressPercentage)
{
    // Trả về CSS class dựa trên tiến độ
}
```

## 10. Phân Quyền Trong CSF Controller

CsfController sử dụng phân quyền dựa trên policy:

-   **CanViewCsfs**: Cho phép xem danh sách và chi tiết CSF
-   **CanManageCsfs**: Cho phép tạo, chỉnh sửa và cập nhật tiến độ CSF
-   **CanDeleteCsfs**: Cho phép xóa CSF

## 11. Mối Liên Hệ Giữa CSF và KPI

CSF là nền tảng để xây dựng KPI:

-   Mỗi CSF có thể liên kết với nhiều KPI
-   KPI được thiết kế để đo lường tiến độ thực hiện các CSF
-   Tiến độ CSF một phần dựa vào mức độ hoàn thành của các KPI liên kết

## 12. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. Xem danh sách CSF
2. Lọc CSF theo danh mục và trạng thái
3. Xem chi tiết một CSF
4. Tạo mới CSF cho phòng Marketing
5. Cập nhật tiến độ CSF
6. Xem mối liên hệ giữa CSF và các KPI

## 13. Tổ Chức Dữ Liệu

CsfController làm việc với nhiều loại dữ liệu:

-   **CsfListViewModel**: Hiển thị danh sách CSF với bộ lọc
-   **CsfDetailsViewModel**: Chi tiết về một CSF
-   **CreateCsfViewModel**: Dữ liệu để tạo CSF mới
-   **EditCsfViewModel**: Dữ liệu để cập nhật CSF
-   **CsfProgressUpdateViewModel**: Cập nhật tiến độ CSF

## 14. Tóm Tắt

CsfController là thành phần then chốt trong hệ thống quản lý KPI:

-   Quản lý các Yếu tố thành công then chốt của tổ chức
-   Thiết lập nền tảng cho việc xây dựng và đo lường KPI
-   Theo dõi tiến độ thực hiện mục tiêu chiến lược
-   Cung cấp cái nhìn tổng quan về trạng thái các yếu tố quyết định thành công

## 15. Tiếp Theo

Trong video tiếp theo, chúng ta sẽ tìm hiểu về KpiController, nơi quản lý các Chỉ số hiệu suất chính (Key Performance Indicators) được thiết kế để đo lường hiệu quả hoạt động của tổ chức trong việc đạt được các Yếu tố thành công then chốt.
