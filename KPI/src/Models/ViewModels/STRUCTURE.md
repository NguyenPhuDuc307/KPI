# Kế hoạch tái cấu trúc ViewModels

Cấu trúc ViewModels hiện tại đang có một số vấn đề về tổ chức và phân cấp làm cho việc bảo trì và mở rộng trở nên khó khăn. Tài liệu này đề xuất một kế hoạch tái cấu trúc để cải thiện tình hình.

## Vấn đề hiện tại

1. **Lồng ghép quá nhiều**: Một số namespace có cấu trúc quá sâu, ví dụ: `KPISolution.Models.ViewModels.Indicator.Measurement`.
2. **Trùng lặp logic**: Code xử lý đo lường tồn tại ở nhiều nơi và không có lớp cơ sở chung.
3. **Thiếu tính nhất quán**: Một số ViewModel có cấu trúc và quy ước đặt tên khác nhau.

## Cấu trúc đề xuất

```
Models/
└── ViewModels/
    ├── Common/                # ViewModels dùng chung
    │   └── ErrorViewModel.cs  # ViewModel cho trang lỗi
    │
    ├── Dashboard/             # ViewModels cho dashboard
    │
    ├── Department/            # ViewModels phòng ban
    │
    ├── Hierarchy/             # ViewModels cho cấu trúc phân cấp
    │
    ├── Indicator/             # ViewModels chỉ số
    │   ├── PerformanceIndicator/ # KPI và PI
    │   ├── ResultIndicator/   # KRI và RI
    │   └── ListModel/         # Mô hình danh sách
    │
    ├── Measurement/           # ViewModels cho đo lường
    │
    ├── Objective/             # ViewModels mục tiêu
    │
    ├── Organization/          # ViewModels tổ chức
    │
    ├── SuccessFactor/         # ViewModels yếu tố thành công
    │
    └── Users/                 # ViewModels người dùng
```

## Kế hoạch triển khai

Việc chuyển đổi sẽ được thực hiện qua 3 giai đoạn:

### Giai đoạn 1: Chuẩn bị cấu trúc mới (Hiện tại)

1. Tạo thư mục cho cấu trúc mới: `Common`, `Hierarchy`, và `Measurement`
2. Tạo các lớp cơ sở và trừu tượng trong thư mục mới
3. Tạo tài liệu hướng dẫn cấu trúc mới (file này)

### Giai đoạn 2: Tạo các lớp trung gian (Transition)

1. Tạo các lớp kế thừa trong thư mục mới kế thừa từ các lớp cũ
2. Cập nhật GlobalUsings.cs để sử dụng cả namepace cũ và mới
3. Bắt đầu sử dụng các lớp mới trong code mới

### Giai đoạn 3: Hoàn tất chuyển đổi

1. Cập nhật toàn bộ code để sử dụng namespace mới
2. Di chuyển logic từ lớp cũ sang lớp mới
3. Loại bỏ namespace cũ và các lớp trùng lặp

## Lợi ích của cấu trúc mới

1. **Cấu trúc rõ ràng hơn**: Mỗi thư mục đại diện cho một khía cạnh rõ ràng của ứng dụng
2. **Giảm lồng ghép**: Cấu trúc phẳng hơn giúp dễ điều hướng
3. **Code sạch hơn**: Sử dụng kế thừa và các lớp cơ sở để giảm trùng lặp
4. **Dễ bảo trì**: Dễ dàng thêm/sửa/xóa các ViewModel mà không ảnh hưởng đến phần khác

## Thời gian triển khai

-   Giai đoạn 1: Hoàn thành trong ngày
-   Giai đoạn 2: 2-3 ngày
-   Giai đoạn 3: 1 tuần

## Ghi chú

Cấu trúc này phù hợp với mô hình phân tách trách nhiệm rõ ràng (SRP) và đảm bảo dễ dàng mở rộng trong tương lai.
