# Tổng quan hệ thống KPI Solution

## 1. Tổng quan dự án

KPI Solution là một hệ thống quản lý chỉ số hiệu suất (KPI) và chỉ số kết quả (KRI) toàn diện, được xây dựng trên nền tảng ASP.NET Core. Hệ thống giúp tổ chức theo dõi, đánh giá và quản lý hiệu suất thông qua việc thiết lập các mục tiêu, yếu tố thành công, và các chỉ số đo lường.

## 2. Kiến trúc hệ thống

-   **Nền tảng:** ASP.NET Core MVC
-   **Cơ sở dữ liệu:** Microsoft SQL Server/Azure SQL Edge
-   **Pattern:** Repository, Unit of Work
-   **Authentication:** ASP.NET Core Identity với hỗ trợ xác thực 2 yếu tố
-   **Docker:** Hỗ trợ triển khai bằng Docker

## 3. Mô hình dữ liệu chính

### 3.1. Chỉ số (Indicators)

-   **IndicatorBase:** Lớp cơ sở cho tất cả các chỉ số, cung cấp các thuộc tính chung như tên, mã, mô tả, phòng ban, người phụ trách, trạng thái, v.v.
-   **PerformanceIndicator (KPI/PI):** Các chỉ số đo lường hiệu suất, dùng để đánh giá tiến độ đạt mục tiêu.
-   **ResultIndicator (KRI/RI):** Các chỉ số đo lường kết quả, dùng để đánh giá kết quả đạt được.

### 3.2. Yếu tố thành công (Success Factors)

-   **SuccessFactor (SF):** Các yếu tố góp phần vào việc đạt được mục tiêu.
-   **CriticalSuccessFactor (CSF):** Các yếu tố then chốt, quan trọng đặc biệt cho việc đạt mục tiêu.

### 3.3. Tổ chức (Organization)

-   **Department:** Phòng ban trong tổ chức.
-   **Objective:** Mục tiêu kinh doanh.

### 3.4. Đo lường (Measurement)

-   **Measurement:** Các giá trị đo lường cho chỉ số theo thời gian, bao gồm giá trị thực tế, ngày đo, trạng thái, ghi chú, v.v.

## 4. Chức năng chính

### 4.1. Quản lý mục tiêu (Objectives)

-   Tạo, cập nhật, xem và xóa các mục tiêu
-   Theo dõi tiến độ mục tiêu
-   Gán mục tiêu cho phòng ban
-   Phân quyền quản lý mục tiêu

### 4.2. Quản lý yếu tố thành công (Success Factors)

-   Quản lý SF và CSF
-   Gán SF cho mục tiêu
-   Phân cấp SF (cha-con)
-   Cập nhật tiến độ
-   Lọc, tìm kiếm và phân trang

### 4.3. Quản lý chỉ số hiệu suất (KPI/PI)

-   Tạo, cập nhật, xem và xóa chỉ số
-   Gán chỉ số cho phòng ban
-   Thiết lập giá trị mục tiêu, đơn vị đo, tần suất đo lường
-   Định cấu hình cảnh báo

### 4.4. Quản lý chỉ số kết quả (KRI/RI)

-   Tạo, cập nhật, xem và xóa chỉ số kết quả
-   Theo dõi mức độ rủi ro
-   Gán chỉ số cho phòng ban
-   Cấu hình ngưỡng cảnh báo

### 4.5. Đo lường hiệu suất

-   Nhập giá trị đo lường cho các chỉ số
-   Lịch sử đo lường
-   Biểu đồ theo dõi xu hướng
-   Báo cáo theo thời gian và phòng ban
-   Xuất dữ liệu sang Excel

### 4.6. Dashboard và báo cáo

-   Dashboard tổng quan
-   Báo cáo theo phòng ban
-   Báo cáo theo loại chỉ số
-   Các widget hiển thị trạng thái và tiến độ

### 4.7. Quản lý người dùng và phân quyền

-   Đăng ký, đăng nhập, quản lý hồ sơ người dùng
-   Xác thực hai yếu tố
-   Phân quyền dựa trên vai trò
-   Ủy quyền dựa trên chính sách

## 5. Luồng xử lý chính

### 5.1. Thiết lập mục tiêu và yếu tố thành công

1. Người dùng tạo mục tiêu kinh doanh (Objective)
2. Xác định các yếu tố thành công (SF) để đạt mục tiêu
3. Xác định các yếu tố thành công then chốt (CSF)
4. Gán mục tiêu và SF cho phòng ban, người phụ trách

### 5.2. Thiết lập và đo lường chỉ số

1. Thiết lập các chỉ số hiệu suất (KPI/PI) và chỉ số kết quả (KRI/RI)
2. Cấu hình thông số: giá trị mục tiêu, đơn vị đo, tần suất đo lường, v.v.
3. Thực hiện đo lường theo chu kỳ đã định
4. Hệ thống tự động cập nhật trạng thái dựa trên giá trị đo lường
5. Vẽ biểu đồ xu hướng và cảnh báo nếu cần

### 5.3. Theo dõi và báo cáo

1. Dashboard hiển thị tổng quan trạng thái chỉ số
2. Xem chi tiết lịch sử đo lường từng chỉ số
3. Tạo báo cáo theo thời gian, phòng ban, loại chỉ số
4. Xuất dữ liệu sang Excel để phân tích sâu hơn

## 6. Yêu cầu hệ thống

### 6.1. Phần cứng

-   .NET Core Runtime
-   SQL Server hoặc Azure SQL Edge
-   Docker (nếu triển khai container)

### 6.2. Bảo mật

-   Xác thực người dùng qua ASP.NET Core Identity
-   Hỗ trợ xác thực hai yếu tố
-   Phân quyền dựa trên vai trò và chính sách
-   Bảo vệ cookie bằng HttpOnly và Secure policy

### 6.3. Triển khai

-   Hỗ trợ triển khai bằng Docker
-   Cấu hình cho môi trường phát triển và sản xuất
-   Ghi log chi tiết bằng Serilog

## 7. Công nghệ sử dụng

-   ASP.NET Core MVC
-   Entity Framework Core
-   AutoMapper
-   Bootstrap UI
-   Serilog (ghi log)
-   ClosedXML (xuất Excel)
-   Chart.js (vẽ biểu đồ)
