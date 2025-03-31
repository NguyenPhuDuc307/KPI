# Video Hướng Dẫn: KpiController

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về KpiController trong hệ thống quản lý KPI. KpiController là thành phần cốt lõi, quản lý các Chỉ số hiệu suất chính (Key Performance Indicators - KPI) - công cụ đo lường hiệu quả hoạt động của tổ chức trong việc thực hiện các Yếu tố thành công then chốt (CSF).

## 2. Cấu Trúc KpiController

KpiController quản lý mọi khía cạnh của KPI, từ tạo mới, xem, cập nhật đến phân tích:

```csharp
[Authorize]
public class KpiController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KpiController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public KpiController(
        IUnitOfWork unitOfWork,
        ILogger<KpiController> logger,
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

## 3. Hiển Thị Danh Sách KPI

Hiển thị danh sách KPI với các tùy chọn lọc và sắp xếp:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
public async Task<IActionResult> Index(KpiFilterViewModel filter, int page = 1)
{
    // Lấy danh sách KPI với phân trang và lọc...
}
```

-   **URL**: `/KPI/Index` hoặc `/KPI`
-   **Quyền truy cập**: Người dùng có quyền xem KPI
-   **Chức năng chính**:
    -   Hiển thị danh sách KPI có phân trang
    -   Lọc theo nhiều tiêu chí (tên, mã, phòng ban, trạng thái, loại KPI)
    -   Sắp xếp kết quả theo các trường khác nhau
    -   Hiển thị trạng thái hiệu suất của từng KPI

## 4. Xem Chi Tiết KPI

Hiển thị thông tin chi tiết về một KPI cụ thể:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
public async Task<IActionResult> Details(Guid? id)
{
    // Lấy chi tiết KPI và dữ liệu liên quan...
}
```

-   **URL**: `/KPI/Details/{id}`
-   **Quyền truy cập**: Người dùng có quyền xem KPI
-   **Chức năng chính**:
    -   Hiển thị thông tin chi tiết về KPI
    -   Biểu đồ hiệu suất theo thời gian
    -   Danh sách CSF liên kết
    -   So sánh với giá trị mục tiêu
    -   Lịch sử cập nhật giá trị

## 5. Tạo Mới KPI

Tạo mới một Chỉ số hiệu suất chính:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> Create()
{
    // Hiển thị form tạo KPI...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> Create(CreateKpiViewModel viewModel)
{
    // Xử lý tạo KPI...
}
```

-   **URL**: `/KPI/Create`
-   **Quyền truy cập**: Người dùng có quyền quản lý KPI
-   **Thông tin cần nhập**:
    -   Tên và mã KPI
    -   Mô tả chi tiết và cách tính
    -   Đơn vị đo lường
    -   Tần suất đo lường (hàng ngày, hàng tuần, hàng tháng)
    -   Giá trị mục tiêu
    -   Giá trị ngưỡng (thấp, trung bình, cao)
    -   Phòng ban chịu trách nhiệm
    -   CSF liên quan
    -   Nguồn dữ liệu
    -   Người chịu trách nhiệm

## 6. Chỉnh Sửa KPI

Chỉnh sửa thông tin của KPI đã tạo:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> Edit(Guid? id)
{
    // Hiển thị form chỉnh sửa KPI...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> Edit(Guid id, EditKpiViewModel viewModel)
{
    // Xử lý cập nhật KPI...
}
```

-   **URL**: `/KPI/Edit/{id}`
-   **Quyền truy cập**: Người dùng có quyền quản lý KPI
-   **Chức năng**: Cập nhật mọi thông tin của KPI ngoại trừ mã KPI

## 7. Xóa KPI

Xóa một KPI khỏi hệ thống:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
public async Task<IActionResult> Delete(Guid? id)
{
    // Hiển thị xác nhận xóa...
}

[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanDeleteKpis)]
public async Task<IActionResult> DeleteConfirmed(Guid id)
{
    // Xử lý xóa KPI...
}
```

-   **URL**: `/KPI/Delete/{id}`
-   **Quyền truy cập**: Người dùng có quyền xóa KPI
-   **Lưu ý**: Xóa KPI sẽ ảnh hưởng đến dữ liệu lịch sử và các CSF liên kết

## 8. Cập Nhật Giá Trị KPI

Cập nhật giá trị đo lường mới cho KPI:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanUpdateKpiValues)]
public async Task<IActionResult> UpdateValue(Guid? id)
{
    // Hiển thị form cập nhật giá trị...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanUpdateKpiValues)]
public async Task<IActionResult> UpdateValue(KpiValueUpdateViewModel viewModel)
{
    // Xử lý cập nhật giá trị...
}
```

-   **URL**: `/KPI/UpdateValue/{id}`
-   **Quyền truy cập**: Người dùng có quyền cập nhật giá trị KPI
-   **Thông tin cập nhật**:
    -   Giá trị đo lường mới
    -   Ngày đo lường
    -   Ghi chú và phân tích
    -   Đánh dấu có cần chú ý không

## 9. Phân Tích Xu Hướng KPI

Hiển thị phân tích chi tiết về xu hướng của một KPI:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
public async Task<IActionResult> Trend(Guid? id, DateTime? startDate, DateTime? endDate)
{
    // Hiển thị phân tích xu hướng...
}
```

-   **URL**: `/KPI/Trend/{id}`
-   **Quyền truy cập**: Người dùng có quyền xem KPI
-   **Chức năng chính**:
    -   Biểu đồ giá trị theo thời gian
    -   Phân tích tốc độ tăng/giảm
    -   So sánh với kỳ vọng
    -   Dự báo giá trị tương lai (nếu có)

## 10. So Sánh KPI

So sánh nhiều KPI với nhau:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanViewKpis)]
public async Task<IActionResult> Compare(List<Guid> ids)
{
    // Hiển thị so sánh giữa các KPI...
}
```

-   **URL**: `/KPI/Compare`
-   **Quyền truy cập**: Người dùng có quyền xem KPI
-   **Chức năng chính**:
    -   So sánh hiệu suất của nhiều KPI trên cùng biểu đồ
    -   Phân tích tương quan giữa các KPI

## 11. Báo Cáo KPI

Tạo báo cáo về KPI:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanGenerateReports)]
public async Task<IActionResult> Report(KpiReportViewModel viewModel)
{
    // Tạo báo cáo về KPI...
}
```

-   **URL**: `/KPI/Report`
-   **Quyền truy cập**: Người dùng có quyền tạo báo cáo
-   **Loại báo cáo**:
    -   Báo cáo theo phòng ban
    -   Báo cáo theo CSF
    -   Báo cáo tổng hợp

## 12. Liên Kết KPI với CSF

Quản lý mối liên hệ giữa KPI và CSF:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> LinkCsf(Guid id)
{
    // Hiển thị form liên kết CSF...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanManageKpis)]
public async Task<IActionResult> LinkCsf(KpiCsfLinkViewModel viewModel)
{
    // Xử lý liên kết CSF với KPI...
}
```

-   **URL**: `/KPI/LinkCsf/{id}`
-   **Quyền truy cập**: Người dùng có quyền quản lý KPI
-   **Chức năng**: Thiết lập mối liên hệ giữa KPI và CSF, xác định mức độ ảnh hưởng

## 13. Nhập Dữ Liệu Hàng Loạt

Nhập dữ liệu cho nhiều KPI cùng lúc:

```csharp
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanUpdateKpiValues)]
public IActionResult BatchUpdate()
{
    // Hiển thị form nhập liệu hàng loạt...
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Policy = KpiAuthorizationPolicies.PolicyNames.CanUpdateKpiValues)]
public async Task<IActionResult> BatchUpdate(KpiBatchUpdateViewModel viewModel)
{
    // Xử lý cập nhật hàng loạt...
}
```

-   **URL**: `/KPI/BatchUpdate`
-   **Quyền truy cập**: Người dùng có quyền cập nhật giá trị KPI
-   **Chức năng**: Cập nhật giá trị cho nhiều KPI trong cùng một biểu mẫu

## 14. Các Hàm Hỗ Trợ

KpiController có nhiều hàm hỗ trợ để xử lý logic phức tạp:

### 14.1. Tính Toán Trạng Thái

```csharp
private KpiStatus CalculateStatus(double currentValue, double targetValue, double threshold)
{
    // Tính toán trạng thái KPI dựa trên giá trị hiện tại so với mục tiêu
}
```

### 14.2. Tính Toán Xu Hướng

```csharp
private TrendDirection CalculateTrend(List<KpiValue> values)
{
    // Phân tích xu hướng tăng/giảm dựa trên các giá trị gần đây
}
```

### 14.3. Tính Toán Dự Báo

```csharp
private List<KpiPrediction> GeneratePredictions(List<KpiValue> values, int daysToForecast)
{
    // Dự báo giá trị KPI trong tương lai
}
```

## 15. Phân Quyền Trong KPI Controller

KpiController sử dụng phân quyền dựa trên policy:

-   **CanViewKpis**: Cho phép xem danh sách và chi tiết KPI
-   **CanManageKpis**: Cho phép tạo và chỉnh sửa KPI
-   **CanUpdateKpiValues**: Cho phép cập nhật giá trị đo lường cho KPI
-   **CanDeleteKpis**: Cho phép xóa KPI
-   **CanGenerateReports**: Cho phép tạo báo cáo về KPI

## 16. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. Xem danh sách KPI
2. Lọc KPI theo phòng ban và trạng thái
3. Xem chi tiết một KPI
4. Tạo mới KPI cho phòng Kinh doanh
5. Cập nhật giá trị KPI
6. Xem biểu đồ xu hướng
7. Liên kết KPI với CSF

## 17. Tổ Chức Dữ Liệu

KpiController làm việc với nhiều loại dữ liệu:

-   **KpiListViewModel**: Hiển thị danh sách KPI với bộ lọc
-   **KpiDetailsViewModel**: Chi tiết về một KPI
-   **CreateKpiViewModel**: Dữ liệu để tạo KPI mới
-   **EditKpiViewModel**: Dữ liệu để cập nhật KPI
-   **KpiValueUpdateViewModel**: Cập nhật giá trị đo lường KPI
-   **KpiTrendViewModel**: Phân tích xu hướng KPI
-   **KpiCompareViewModel**: So sánh các KPI
-   **KpiReportViewModel**: Tạo báo cáo KPI

## 18. Tóm Tắt

KpiController là trung tâm của hệ thống quản lý KPI:

-   Quản lý toàn diện các Chỉ số hiệu suất chính của tổ chức
-   Cung cấp công cụ theo dõi, cập nhật và phân tích KPI
-   Kết nối KPI với CSF để đảm bảo tính liên kết chiến lược
-   Cung cấp cái nhìn đa chiều về hiệu suất tổ chức

## 19. Tiếp Theo

Trong video tiếp theo, chúng ta sẽ tìm hiểu về các controller chuyên biệt cho các loại chỉ số đo lường khác: PiController (Performance Indicators), KriController (Key Result Indicators), và RiController (Result Indicators).
