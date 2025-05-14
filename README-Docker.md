# Hướng dẫn chạy ứng dụng KPI bằng Docker

## Yêu cầu

-   Docker và Docker Compose đã được cài đặt
-   Cổng 8080, 8443 và 1433 không bị sử dụng bởi ứng dụng khác

## Các bước chạy ứng dụng

1. Mở terminal hoặc command prompt
2. Di chuyển đến thư mục chứa mã nguồn (thư mục chứa tập tin docker-compose.yml)
3. Chạy lệnh sau để khởi động ứng dụng:

```bash
docker-compose up -d
```

4. Sau khi khởi động hoàn tất, truy cập ứng dụng tại:

    - http://localhost:8080 (HTTP)
    - https://localhost:8443 (HTTPS)

5. Để dừng ứng dụng, chạy lệnh:

```bash
docker-compose down
```

## Cấu trúc Docker

-   **kpi-app**: Dịch vụ chính chạy ứng dụng ASP.NET Core
-   **db**: Dịch vụ Azure SQL Edge lưu trữ dữ liệu (tương thích với Mac M1/ARM)

## Cài đặt bổ sung

### Xem logs

```bash
# Xem logs của ứng dụng
docker-compose logs kpi-app

# Xem logs của database
docker-compose logs db

# Xem logs theo thời gian thực
docker-compose logs -f
```

### Truy cập vào container

```bash
# Truy cập vào container ứng dụng
docker-compose exec kpi-app /bin/bash

# Truy cập vào container SQL Server
docker-compose exec db /bin/bash
```

### Kết nối đến SQL Server

-   Server: localhost,1433
-   Tài khoản: sa
-   Mật khẩu: Password.1
-   Database: KPI (Azure SQL Edge)

## Xử lý sự cố

1. **Không thể kết nối đến ứng dụng**: Kiểm tra logs bằng lệnh `docker-compose logs kpi-app`
2. **Lỗi kết nối đến database**: Kiểm tra logs bằng lệnh `docker-compose logs db`
3. **Khởi động lại các dịch vụ**: `docker-compose restart kpi-app` hoặc `docker-compose restart db`

## Bảo mật

-   Đây là cấu hình cho môi trường phát triển/thử nghiệm.
-   Sử dụng Azure SQL Edge thay vì SQL Server chính thức để tương thích với Mac M1/ARM.
-   Trong môi trường sản xuất, bạn nên thay đổi các thông tin đăng nhập mặc định và cấu hình mạng phù hợp.
