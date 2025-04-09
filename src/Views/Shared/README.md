# Cấu trúc thư mục Shared

Thư mục này chứa các partial view được tổ chức theo chức năng để dễ quản lý và bảo trì.

## Cấu trúc thư mục

```
/Shared
├── /Layout                 # Các thành phần liên quan đến layout và cấu trúc trang
│   ├── _Layout.cshtml      # Layout chính của ứng dụng
│   ├── _Breadcrumb.cshtml  # Navigation breadcrumb
│   ├── _PageHeader.cshtml  # Header cho mỗi trang
│   ├── _PageTemplate.cshtml # Template trang cơ bản
│   └── _PageTitle.cshtml   # Tiêu đề trang
│
├── /Widgets                # Các widget được sử dụng trong dashboard
│   ├── _AlertWidget.cshtml # Widget hiển thị cảnh báo
│   ├── _CalendarWidget.cshtml # Widget lịch
│   ├── _ChartWidget.cshtml # Widget biểu đồ
│   ├── _CsfProgressWidget.cshtml # Widget tiến độ CSF
│   ├── _EmptyWidget.cshtml # Widget trống
│   ├── _KpiCardWidget.cshtml # Widget card KPI/Indicator
│   ├── _KpiTableWidget.cshtml # Widget bảng KPI/Indicator
│   ├── _StatisticsWidget.cshtml # Widget thống kê
│   ├── _TableWidget.cshtml # Widget bảng dữ liệu
│   └── _TextWidget.cshtml  # Widget văn bản
│
├── /Tree                   # Các thành phần hiển thị cấu trúc dạng cây
│   ├── _DepartmentHierarchyPartial.cshtml # Hiển thị cấu trúc phòng ban
│   ├── _ObjectiveTreeNodePartial.cshtml # Hiển thị cấu trúc mục tiêu
│   └── _TreeNodePartial.cshtml # Thành phần cơ bản của cấu trúc cây
│
├── /Modals                 # Các modal dialog
│   ├── _ProgressUpdateModal.cshtml # Modal cập nhật tiến độ
│   └── _QuickProgressUpdate.cshtml # Thành phần cập nhật tiến độ nhanh
│
├── /Common                 # Các thành phần dùng chung
│   ├── _Alert.cshtml       # Hiển thị thông báo
│   ├── _ValidationScriptsPartial.cshtml # Script validation
│   └── Error.cshtml        # Trang lỗi
│
└── /Authentication         # Các thành phần liên quan đến xác thực
    └── _LoginPartial.cshtml # Partial view đăng nhập
```

## Quy ước đặt tên

1. Tất cả file partial view đều bắt đầu bằng dấu gạch dưới (\_)
2. Tên file nên phản ánh chức năng của nó
3. Widget nên có hậu tố "Widget" (ví dụ: \_ChartWidget.cshtml)
