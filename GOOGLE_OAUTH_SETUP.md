# Cấu hình Google OAuth cho KPI Solution

## Bước 1: Tạo Project trên Google Cloud Console

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo một project mới hoặc chọn project hiện có
3. Kích hoạt Google+ API (nếu cần thiết)

## Bước 2: Tạo OAuth 2.0 Client Credentials

1. Trong Google Cloud Console, điều hướng đến **APIs & Services** → **Credentials**
2. Nhấp **Create Credentials** → **OAuth client ID**
3. Chọn **Web application** làm Application type
4. Cấu hình như sau:

### Authorized JavaScript origins (cho development):
```
http://localhost:5000
https://localhost:5001
http://localhost:7112
https://localhost:7113
```

### Authorized redirect URIs (cho development):
```
http://localhost:5000/signin-google
https://localhost:5001/signin-google
http://localhost:7112/signin-google
https://localhost:7113/signin-google
```

### Cho production environment:
```
https://your-domain.com/signin-google
```

## Bước 3: Cấu hình User Secrets (Khuyến nghị cho Development)

Thay vì lưu trực tiếp trong appsettings.json, sử dụng User Secrets:

```bash
cd /Users/ducnp/Projects/KPI/src
dotnet user-secrets set "Authentication:Google:ClientId" "your-google-client-id.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-google-client-secret"
```

## Bước 4: Cấu hình cho Production

Trong production, lưu trữ credentials an toàn bằng:

### Azure Key Vault (khuyến nghị):
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/GoogleClientId/)",
      "ClientSecret": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/GoogleClientSecret/)"
    }
  }
}
```

### Environment Variables:
```bash
export Authentication__Google__ClientId="your-google-client-id"
export Authentication__Google__ClientSecret="your-google-client-secret"
```

## Bước 5: Kiểm tra OAuth Consent Screen

1. Trong Google Cloud Console, điều hướng đến **OAuth consent screen**
2. Cấu hình thông tin ứng dụng:
   - Application name: "KPI Management System"
   - User support email: your-email@domain.com
   - Developer contact information: your-email@domain.com
3. Thêm scopes cần thiết:
   - email
   - profile
   - openid

## Bước 6: Thử nghiệm

1. Chạy ứng dụng: `dotnet run`
2. Truy cập trang đăng nhập: `https://localhost:7113/Identity/Account/Login`
3. Kiểm tra nút "Google" xuất hiện
4. Thử đăng nhập bằng Google

## Bảo mật và Best Practices

### 1. Bảo vệ Client Secret
- Không bao giờ commit client secret vào source control
- Sử dụng Azure Key Vault cho production
- Sử dụng User Secrets cho development

### 2. Cấu hình HTTPS
- Luôn sử dụng HTTPS trong production
- Google OAuth yêu cầu HTTPS cho callback URLs

### 3. Kiểm tra Domain
- Chỉ thêm các domain tin cậy vào Authorized origins
- Regularly review authorized domains

### 4. Monitoring
- Theo dõi login attempts qua Google Console
- Set up alerts cho suspicious activities

## Troubleshooting

### Error: redirect_uri_mismatch
- Kiểm tra redirect URI trong Google Console khớp chính xác với callback path
- Đảm bảo protocol (http/https) và port đúng

### Error: invalid_client
- Kiểm tra ClientId và ClientSecret
- Đảm bảo credentials được cấu hình đúng

### Google button không hiển thị
- Kiểm tra cấu hình trong Program.cs
- Đảm bảo Google authentication được thêm vào DI container

## Environment-specific Configuration

### Development (appsettings.Development.json):
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "dev-client-id.apps.googleusercontent.com",
      "ClientSecret": "dev-client-secret"
    }
  }
}
```

### Production (appsettings.json or environment variables):
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "prod-client-id.apps.googleusercontent.com",
      "ClientSecret": "prod-client-secret"
    }
  }
}
```

## Tích hợp với Existing Users

Hệ thống đã được cấu hình để:
- Tự động tạo user mới khi đăng nhập lần đầu bằng Google
- Liên kết Google account với existing email account
- Cho phép user có multiple login providers

## Next Steps

1. Cấu hình Google OAuth credentials
2. Test trên development environment
3. Deploy và test trên staging/production
4. Monitor authentication logs
5. Set up backup authentication methods
