# Video Hướng Dẫn: Các Controller Chỉ Số Khác (PI, KRI, RI)

## 1. Giới Thiệu

Chào mừng bạn đến với video hướng dẫn về các controller chỉ số khác trong hệ thống quản lý KPI. Trong video này, chúng ta sẽ tìm hiểu về ba controller chuyên biệt cho các loại chỉ số đo lường khác nhau:

-   **PiController**: Quản lý Chỉ số hiệu suất (Performance Indicators)
-   **KriController**: Quản lý Chỉ số kết quả chính (Key Result Indicators)
-   **RiController**: Quản lý Chỉ số kết quả (Result Indicators)

Các controller này có cấu trúc và chức năng tương tự KpiController nhưng được điều chỉnh cho từng loại chỉ số cụ thể.

## 2. Phân Biệt Các Loại Chỉ Số

Trước khi đi vào chi tiết từng controller, chúng ta cần hiểu rõ sự khác biệt giữa các loại chỉ số:

1. **KPI (Key Performance Indicators)**:

    - Đo lường hiệu suất của các hoạt động then chốt
    - Phi tài chính, đo lường thường xuyên
    - Ví dụ: Tỷ lệ hoàn thành đơn hàng đúng hạn

2. **PI (Performance Indicators)**:

    - Đo lường hiệu suất của các hoạt động cụ thể
    - Mức độ chi tiết hơn KPI
    - Ví dụ: Thời gian xử lý một loại đơn hàng cụ thể

3. **KRI (Key Result Indicators)**:

    - Đo lường kết quả then chốt của tổ chức
    - Tổng hợp, thường liên quan đến tài chính
    - Dành cho hội đồng quản trị
    - Ví dụ: Lợi nhuận theo quý

4. **RI (Result Indicators)**:
    - Đo lường kết quả của các hoạt động
    - Được đo lường theo tuần/tháng/quý
    - Ví dụ: Số lượng khách hàng mới

## 3. PiController

PiController quản lý các chỉ số hiệu suất (Performance Indicators), tập trung vào các đo lường chi tiết hơn so với KPI:

```csharp
[Authorize]
public class PiController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PiController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public PiController(
        IUnitOfWork unitOfWork,
        ILogger<PiController> logger,
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

### 3.1 Đặc Điểm Của PiController

-   **Loại chỉ số**: Quản lý Performance Indicators
-   **Mục đích**: Theo dõi hiệu suất của các hoạt động cụ thể
-   **Phân quyền**: Tương tự KPI nhưng thường mở rộng cho nhiều người dùng hơn
-   **Tần suất cập nhật**: Thường xuyên, nhiều lần trong ngày hoặc hàng ngày

### 3.2 Các Chức Năng Chính

PiController có các chức năng tương tự KpiController:

-   **Index**: Hiển thị danh sách PI với bộ lọc
-   **Details**: Xem chi tiết một PI
-   **Create/Edit/Delete**: Quản lý thông tin PI
-   **UpdateValue**: Cập nhật giá trị đo lường

### 3.3 Điểm Khác Biệt So Với KpiController

-   **Đo lường chi tiết**: Tập trung vào các đo lường cụ thể hơn
-   **Tần suất cao hơn**: Thường được cập nhật thường xuyên hơn KPI
-   **Tính liên kết KPI**: Mỗi PI thường liên kết với một KPI cụ thể
-   **Phòng ban cụ thể**: Thường thuộc về một phòng ban hoặc bộ phận cụ thể

## 4. KriController

KriController quản lý các chỉ số kết quả chính (Key Result Indicators), tập trung vào các kết quả tổng thể của tổ chức:

```csharp
[Authorize]
public class KriController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<KriController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public KriController(
        IUnitOfWork unitOfWork,
        ILogger<KriController> logger,
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

### 4.1 Đặc Điểm Của KriController

-   **Loại chỉ số**: Quản lý Key Result Indicators
-   **Mục đích**: Đo lường kết quả then chốt của tổ chức
-   **Phân quyền**: Thường giới hạn cho cấp quản lý cao và hội đồng quản trị
-   **Tần suất cập nhật**: Định kỳ, thường hàng tháng hoặc hàng quý

### 4.2 Các Chức Năng Chính

KriController có các chức năng tiêu chuẩn:

-   **Index**: Hiển thị danh sách KRI với bộ lọc
-   **Details**: Xem chi tiết một KRI
-   **Create/Edit/Delete**: Quản lý thông tin KRI
-   **UpdateValue**: Cập nhật giá trị đo lường

### 4.3 Điểm Khác Biệt So Với Các Controller Khác

-   **Báo cáo cấp cao**: Tập trung vào báo cáo cho cấp quản lý cao
-   **Biểu đồ xu hướng dài hạn**: Thường hiển thị dữ liệu trong khoảng thời gian dài
-   **Liên kết chiến lược**: Thường liên kết trực tiếp với mục tiêu chiến lược
-   **Quyền truy cập hạn chế**: Chỉ người dùng cấp cao mới có quyền cập nhật

## 5. RiController

RiController quản lý các chỉ số kết quả (Result Indicators), tập trung vào kết quả cụ thể:

```csharp
[Authorize]
public class RiController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RiController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;

    public RiController(
        IUnitOfWork unitOfWork,
        ILogger<RiController> logger,
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

### 5.1 Đặc Điểm Của RiController

-   **Loại chỉ số**: Quản lý Result Indicators
-   **Mục đích**: Đo lường kết quả của các hoạt động
-   **Phân quyền**: Cấp quản lý trung gian và trưởng phòng ban
-   **Tần suất cập nhật**: Thường hàng tuần hoặc hàng tháng

### 5.2 Các Chức Năng Chính

RiController có các chức năng tương tự các controller khác:

-   **Index**: Hiển thị danh sách RI
-   **Details**: Xem chi tiết một RI
-   **Create/Edit/Delete**: Quản lý thông tin RI
-   **UpdateValue**: Cập nhật giá trị đo lường

### 5.3 Điểm Khác Biệt So Với Các Controller Khác

-   **Kết quả phòng ban**: Tập trung vào kết quả của từng phòng ban
-   **Liên kết với KRI**: Thường nhiều RI sẽ góp phần tạo nên một KRI
-   **Báo cáo trung gian**: Phục vụ cho báo cáo cấp quản lý trung gian
-   **Mức phân tích**: Chi tiết hơn KRI nhưng tổng hợp hơn PI

## 6. Mối Quan Hệ Giữa Các Loại Chỉ Số

Hệ thống quản lý KPI sử dụng mô hình phân cấp chỉ số:

-   **KRI**: Ở cấp cao nhất, phục vụ ban lãnh đạo
-   **RI**: Ở cấp trung gian, phục vụ quản lý
-   **KPI**: Chỉ số hiệu suất then chốt, liên kết với CSF
-   **PI**: Ở cấp chi tiết nhất, đo lường hoạt động cụ thể

![Mô hình phân cấp chỉ số](diagram/indicator_hierarchy.png)

## 7. Ví Dụ Minh Họa

### 7.1 Ví Dụ Về KRI

```csharp
// Doanh thu quý
var quarterlyRevenue = new Kri
{
    Name = "Doanh thu quý",
    Code = "KRI001",
    Description = "Tổng doanh thu trong quý hiện tại",
    Unit = "Triệu đồng",
    TargetValue = 1000,
    Frequency = MeasurementFrequency.Quarterly
};
```

### 7.2 Ví Dụ Về RI

```csharp
// Doanh thu theo phòng ban
var departmentRevenue = new Ri
{
    Name = "Doanh thu phòng Kinh doanh",
    Code = "RI001",
    Description = "Tổng doanh thu của phòng Kinh doanh trong tháng",
    Unit = "Triệu đồng",
    TargetValue = 250,
    Frequency = MeasurementFrequency.Monthly,
    Department = "Kinh doanh"
};
```

### 7.3 Ví Dụ Về KPI

```csharp
// Tỷ lệ đơn hàng hoàn thành
var orderCompletionRate = new Kpi
{
    Name = "Tỷ lệ đơn hàng hoàn thành đúng hạn",
    Code = "KPI001",
    Description = "Phần trăm đơn hàng được hoàn thành và giao đúng hạn",
    Unit = "%",
    TargetValue = 95,
    Frequency = MeasurementFrequency.Weekly,
    Department = "Vận hành"
};
```

### 7.4 Ví Dụ Về PI

```csharp
// Thời gian xử lý đơn hàng
var orderProcessingTime = new Pi
{
    Name = "Thời gian xử lý đơn hàng trực tuyến",
    Code = "PI001",
    Description = "Thời gian trung bình để xử lý đơn hàng trực tuyến",
    Unit = "Phút",
    TargetValue = 15,
    Frequency = MeasurementFrequency.Daily,
    Department = "Vận hành",
    LinkedKpiId = orderCompletionRate.Id
};
```

## 8. Demo Thực Tế

(Phần này sẽ bao gồm demo trực tiếp trên giao diện, với các bước cụ thể)

1. **PiController**:

    - Xem danh sách PI
    - Tạo mới PI liên kết với KPI
    - Cập nhật giá trị PI

2. **KriController**:

    - Xem dashboard KRI
    - Hiển thị xu hướng KRI theo quý
    - Phân tích so với mục tiêu

3. **RiController**:
    - Lọc RI theo phòng ban
    - Cập nhật giá trị RI hàng tuần
    - Xem báo cáo RI theo phòng ban

## 9. Tổ Chức Dữ Liệu Chung

Tất cả các controller đều sử dụng cấu trúc tương tự:

-   Các lớp ViewModels cho từng chức năng
-   Phân quyền dựa trên policy
-   Cơ chế lọc và sắp xếp
-   Repository pattern thông qua UnitOfWork

## 10. Tóm Tắt

Hệ thống quản lý KPI sử dụng nhiều loại chỉ số đo lường:

-   **KRI**: Chỉ số kết quả then chốt dành cho lãnh đạo cao cấp
-   **RI**: Chỉ số kết quả dành cho quản lý cấp trung
-   **KPI**: Chỉ số hiệu suất then chốt gắn với CSF
-   **PI**: Chỉ số hiệu suất chi tiết dành cho cấp quản lý vận hành

Mỗi loại chỉ số có controller riêng để quản lý, với các đặc điểm phù hợp với mục đích sử dụng và đối tượng người dùng cụ thể.

## 11. Tiếp Theo

Trong video tiếp theo, chúng ta sẽ tìm hiểu về ErrorController - controller xử lý lỗi trong hệ thống quản lý KPI.
