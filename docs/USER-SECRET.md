# Xem tất cả secrets
dotnet user-secrets list --project src/TheGourmet.Api

# Hoặc xem một secret cụ thể
dotnet user-secrets list --project src/TheGourmet.Api | grep "MailSettings:Password"

# Thêm hoặc cập nhật một secret
dotnet user-secrets set "MailSettings:Password" "NewSecurePassword123!" --project src/TheGourmet.Api