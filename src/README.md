# Cấu trúc ViewModels

Đây là cấu trúc thư mục ViewModels của dự án KPI Solution, được tổ chức lại để dễ bảo trì và mở rộng.

## Cấu trúc thư mục

```
Models/
└── ViewModels/
    ├── Common/                # ViewModels dùng chung cho toàn bộ ứng dụng
    │   └── ErrorViewModel.cs  # ViewModel cho trang lỗi
    │
    ├── Dashboard/             # ViewModels cho các dashboard
    │   └── Widgets/           # ViewModels cho các widget trên dashboard
    │
    ├── Department/            # ViewModels liên quan đến phòng ban
    │
    ├── Hierarchy/             # ViewModels cho cấu trúc phân cấp
    │   └── IndicatorHierarchyViewModel.cs # Cấu trúc phân cấp chỉ số
    │
    ├── Indicator/             # ViewModels liên quan đến chỉ số
    │   ├── PerformanceIndicator/ # KPI và PI
    │   ├── ResultIndicator/   # KRI và RI
    │   └── ListModel/         # Mô hình danh sách chỉ số
    │
    ├── Measurement/           # ViewModels cho việc đo lường
    │   ├── AbstractMeasurementViewModel.cs # Lớp cơ sở trừu tượng
    │   ├── MeasurementViewModel.cs  # Hiển thị đo lường
    │   ├── MeasurementListViewModel.cs # Danh sách đo lường
    │   └── ...                # Các ViewModel đo lường khác
    │
    ├── Objective/             # ViewModels cho mục tiêu
    │
    ├── Organization/          # ViewModels cho tổ chức
    │
    ├── SuccessFactor/         # ViewModels cho yếu tố thành công (SF, CSF)
    │
    └── Users/                 # ViewModels cho người dùng
```

## Nguyên tắc tổ chức mới

1. **Phân tách theo chức năng**: Mỗi thư mục chứa các ViewModel liên quan đến một chức năng hoặc tính năng cụ thể.
2. **Nhất quán về đặt tên**: Tên lớp theo mẫu `TênĐốiTượng + ViewModel`.
3. **Lớp cơ sở chung**: Sử dụng các lớp trừu tượng cơ sở để tái sử dụng code.
4. **Phân cấp rõ ràng**: Không lồng các thư mục quá sâu, duy trì cấu trúc phẳng hơn.

## Thay đổi chính

1. **Di chuyển Hierarchy**: Di chuyển từ `Indicator/Hierarchy` lên thư mục gốc để tránh lồng ghép quá nhiều.
2. **Gộp Measurement**: Các ViewModel đo lường được di chuyển từ `Indicator/Measurement` vào thư mục `Measurement`.
3. **Sử dụng kế thừa**: Các class đo lường cụ thể kế thừa từ `AbstractMeasurementViewModel`.

## Cách sử dụng

```csharp
// Đã khai báo trong GlobalUsings.cs
global using KPISolution.Models.ViewModels;
global using KPISolution.Models.ViewModels.Common;
global using KPISolution.Models.ViewModels.SuccessFactor;
global using KPISolution.Models.ViewModels.Indicator;
global using KPISolution.Models.ViewModels.Indicator.PerformanceIndicator;
global using KPISolution.Models.ViewModels.Indicator.ResultIndicator;
global using KPISolution.Models.ViewModels.Indicator.ListModel;
global using KPISolution.Models.ViewModels.Hierarchy;
global using KPISolution.Models.ViewModels.Measurement;
global using KPISolution.Models.ViewModels.Organization;
global using KPISolution.Models.ViewModels.Dashboard;
global using KPISolution.Models.ViewModels.Department;
global using KPISolution.Models.ViewModels.Objective;
global using KPISolution.Models.ViewModels.Users;
```
