# OAuth2 Debug Guide

## Các bước để debug OAuth2 login

### 1. Kiểm tra Environment Variables

Backend đã được config với:
- `app.frontend.url=http://localhost:5173` (dev)
- `spring.security.oauth2.client.registration.google.redirect-uri=http://localhost:8080/auth/callback`

Frontend đã được config với:
- `VITE_REDIRECT_URI=http://localhost:8080/auth/callback`
- `VITE_GOOGLE_CLIENT_ID=471569164757-0ge2390vio22e6evqoapc0873vou661l.apps.googleusercontent.com`

### 2. Flow OAuth2 hiện tại

```
User click "Login with Google" 
  ↓
Frontend tạo URL: https://accounts.google.com/o/oauth2/v2/auth?client_id=...&redirect_uri=http://localhost:8080/auth/callback&response_type=code&scope=...
  ↓
Browser redirect to Google
  ↓
User login and authorize
  ↓
Google redirect to: http://localhost:8080/auth/callback?code=xxx
  ↓
Backend OAuth2Controller nhận code và:
  - Exchange code for access_token với Google
  - Lấy user info từ Google
  - Tạo/login user trong DB
  - Tạo JWT tokens
  - Redirect về: http://localhost:5173/login?access_token=xxx&refresh_token=yyy
  ↓
Frontend LoginGoogle component:
  - Parse access_token và refresh_token từ URL
  - Lưu vào localStorage
  - Gọi getMe() để lấy profile
```

### 3. Các điểm cần check khi debug

#### A. Check Backend Logs
Khi user login, backend sẽ log:
```
=== OAuth2 Callback Debug ===
Redirect URI from config: http://localhost:8080/auth/callback
Frontend URL from config: http://localhost:5173
Client ID: 471569164757-0ge2390vio22e6evqoapc0873vou661l.apps.googleusercontent.com
Authorization code received: YES
OAuth2 Success - Redirecting to: http://localhost:5173/login?access_token=...
```

Nếu không thấy log này → Backend không nhận được request từ Google

#### B. Check Browser Console
Mở Developer Tools (F12) → Console tab:
- Có error CORS?
- Có error 401/403?
- URL có query params `access_token` và `refresh_token` không?

#### C. Check Browser Network Tab
Mở Developer Tools (F12) → Network tab:
1. Xem request đến `http://localhost:8080/auth/callback?code=...`
   - Status code phải là 302 (redirect)
   - Response Headers phải có `Location: http://localhost:5173/login?access_token=...`
2. Xem request tiếp theo đến `http://localhost:5173/login?access_token=...`
   - URL này phải có query params

### 4. Test OAuth URL Generation

Mở file `test-oauth-url.html` trong browser để test URL generation:
```bash
open test-oauth-url.html
```

Kiểm tra:
- Client ID đúng?
- Redirect URI = `http://localhost:8080/auth/callback`?
- Scope đúng?

### 5. Kiểm tra Google Console

Vào https://console.cloud.google.com/apis/credentials
- Chọn OAuth 2.0 Client ID
- Kiểm tra "Authorized redirect URIs" có:
  - `http://localhost:8080/auth/callback`
  - `https://domicare-web-v1-5.onrender.com/auth/callback` (production)

### 6. Test từng bước

#### Bước 1: Chạy Backend
```bash
cd DomiCare
mvn spring-boot:run
```

Hoặc với Docker:
```bash
docker-compose up backend
```

Kiểm tra backend đang chạy:
```bash
curl http://localhost:8080/actuator/health
```

#### Bước 2: Chạy Frontend
```bash
cd DOMICARE_FRONTEND
npm run dev
```

Mở browser: http://localhost:5173

#### Bước 3: Test OAuth Flow
1. Click "Login with Google"
2. Mở F12 → Console + Network tabs
3. Login với Google account
4. Xem logs và network requests

### 7. Common Issues

#### Issue 1: redirect_uri_mismatch
**Triệu chứng:** Google trả về error "redirect_uri_mismatch"
**Nguyên nhân:** 
- Frontend gửi redirect_uri khác với backend config
- Hoặc Google Console không có URI này

**Fix:**
- Kiểm tra `.env` file có `VITE_REDIRECT_URI=http://localhost:8080/auth/callback`
- Kiểm tra `application-dev.properties` có `redirect-uri=http://localhost:8080/auth/callback`
- Thêm URI vào Google Console

#### Issue 2: Backend không redirect về frontend
**Triệu chứng:** Browser bị stuck ở backend URL
**Nguyên nhân:** Backend không có `frontendUrl` config

**Fix:**
- Kiểm tra `application-dev.properties` có `app.frontend.url=http://localhost:5173`
- Restart backend

#### Issue 3: Frontend không nhận được tokens
**Triệu chứng:** User redirect về `/login` nhưng không login
**Nguyên nhân:** 
- URL không có query params
- Hoặc useEffect không chạy

**Debug:**
- Mở F12 Console → check URL có `?access_token=` không?
- Check localStorage có `access_token` không?
- Check LoginGoogle component có log errors?

#### Issue 4: CORS Error
**Triệu chứng:** Console show "CORS policy" error
**Nguyên nhân:** Backend CORS không cho phép frontend origin

**Fix:**
- Kiểm tra CorsConfiguration.java
- Kiểm tra SecurityConfiguration.java permitAll cho `/auth/**`

### 8. Debug Commands

```bash
# Check backend env
cd DomiCare
grep "app.frontend.url" src/main/resources/application-dev.properties

# Check frontend env
cd DOMICARE_FRONTEND
cat .env | grep VITE_REDIRECT_URI

# Test backend OAuth endpoint
curl http://localhost:8080/auth/callback?code=test
# Nên trả về error hoặc redirect, không phải 404

# Check backend logs
cd DomiCare
# Xem logs trong terminal chạy spring-boot
```

### 9. Nếu vẫn lỗi

Hãy cung cấp:
1. Backend logs (từ lúc bấm "Login with Google")
2. Browser Console logs (F12 → Console)
3. Browser Network logs (F12 → Network → request đến `/auth/callback`)
4. Screenshot của error message

## Lưu ý

- **Đừng commit Google credentials vào Git!** Đã có trong `.gitignore`
- Dev environment: Chỉ test với `http://localhost` URIs
- Production: Nhớ update URIs trong Google Console và environment variables
