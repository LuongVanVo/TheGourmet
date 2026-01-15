# Google OAuth2 Setup Guide

## ✅ OAuth2 Logic đã được khớp!

Backend và Frontend đã được đồng bộ với flow sau:
1. User click "Login with Google" ở Frontend
2. Frontend redirect đến Google Authorization URL
3. User đăng nhập và đồng ý quyền
4. Google callback về Backend: `/auth/callback`
5. Backend xử lý code, lấy token từ Google, tạo user, tạo JWT
6. Backend redirect về Frontend với access_token & refresh_token
7. Frontend lưu token và đăng nhập user

## 📋 URIs để đăng ký trên Google Cloud Console

### Development Environment
- **Authorized JavaScript origins:**
  ```
  http://localhost:5173
  http://localhost:8080
  ```

- **Authorized redirect URIs:**
  ```
  http://localhost:8080/auth/callback
  ```

### Production Environment  
- **Authorized JavaScript origins:**
  ```
  https://domicare-frontend.vercel.app
  https://domi-care-multi.vercel.app
  https://domicare-web-v1-5.onrender.com
  ```

- **Authorized redirect URIs:**
  ```
  https://domicare-web-v1-5.onrender.com/auth/callback
  ```

## 🔧 Cách đăng ký trên Google Cloud Console

### Bước 1: Tạo Project
1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo project mới hoặc chọn project hiện có
3. Enable **Google+ API** trong "APIs & Services"

### Bước 2: Tạo OAuth 2.0 Credentials
1. Vào **APIs & Services** > **Credentials**
2. Click **Create Credentials** > **OAuth client ID**
3. Chọn **Application type**: **Web application**
4. Đặt tên: `DomiCare OAuth Client`

### Bước 3: Cấu hình URIs
5. Thêm **Authorized JavaScript origins** như trên
6. Thêm **Authorized redirect URIs** như trên
7. Click **Create**

### Bước 4: Lấy Credentials
8. Copy **Client ID** và **Client Secret**
9. Cập nhật vào file `.env`:

**Backend** (`DomiCare/.env`):
```bash
GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=your-client-secret
```

**Frontend** (`DOMICARE_FRONTEND/.env`):
```bash
VITE_GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com
VITE_CLIENT_SECRECT=your-client-secret
VITE_GOOGLE_URL=https://accounts.google.com/o/oauth2/v2/auth
VITE_REDIRECT_URI=http://localhost:8080/auth/callback
```

**Frontend Production** (`DOMICARE_FRONTEND/.env.production`):
```bash
VITE_GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com
VITE_CLIENT_SECRECT=your-client-secret
VITE_GOOGLE_URL=https://accounts.google.com/o/oauth2/v2/auth
VITE_REDIRECT_URI=https://domicare-web-v1-5.onrender.com/auth/callback
```

### Bước 5: OAuth Consent Screen
1. Vào **OAuth consent screen**
2. Chọn **External** (cho testing) hoặc **Internal** (nếu có workspace)
3. Điền thông tin:
   - **App name**: DomiCare
   - **User support email**: your-email@gmail.com
   - **Developer contact**: your-email@gmail.com
4. Thêm scopes:
   - `userinfo.email`
   - `userinfo.profile`
5. Thêm test users (nếu ở External & Testing mode)

## 🧪 Testing
1. Khởi động Backend: `mvn spring-boot:run` (port 8080)
2. Khởi động Frontend: `npm run dev` (port 5173)
3. Truy cập: `http://localhost:5173/login`
4. Click nút "Login with Google"
5. Sau khi đăng nhập thành công, bạn sẽ được redirect về `/login` với token trong URL

## 🚀 Production Deployment Checklist
- [ ] Cập nhật GOOGLE_CLIENT_ID và GOOGLE_CLIENT_SECRET trên production
- [ ] Verify redirect URI trên Google Console khớp với production backend URL
- [ ] Publish OAuth consent screen (nếu cần public access)
- [ ] Test OAuth flow trên production environment
- [ ] Remove test users (nếu publish app)

## 🔐 Security Notes
- ⚠️ **KHÔNG** commit Client Secret vào Git
- ⚠️ Luôn sử dụng environment variables
- ⚠️ Kiểm tra redirect URI để tránh open redirect vulnerability
- ✅ Backend validate email domain nếu cần
- ✅ Implement rate limiting cho OAuth endpoint

## 📞 Support
Nếu gặp lỗi:
- Check console logs (F12) trên frontend
- Check backend logs
- Verify URLs khớp chính xác (không có trailing slash)
- Verify Client ID và Secret đúng
- Kiểm tra OAuth consent screen settings
