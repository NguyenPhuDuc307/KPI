-- Tạo user kpiuser với mysql_native_password plugin
-- Xóa user cũ nếu tồn tại
DROP USER IF EXISTS 'kpiuser'@'%';

-- Tạo user mới với mysql_native_password
CREATE USER 'kpiuser'@'%' IDENTIFIED WITH mysql_native_password BY 'kpipassword';

-- Cấp quyền cho user
GRANT ALL PRIVILEGES ON kpi.* TO 'kpiuser'@'%';

-- Đảm bảo root user cũng sử dụng mysql_native_password
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'rootpassword';
ALTER USER 'root'@'%' IDENTIFIED WITH mysql_native_password BY 'rootpassword';

-- Flush privileges để áp dụng thay đổi
FLUSH PRIVILEGES;

-- Hiển thị thông tin user để verify
SELECT user, host, plugin FROM mysql.user WHERE user IN ('root', 'kpiuser');
