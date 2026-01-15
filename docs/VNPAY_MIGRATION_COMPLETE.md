# ✅ VNPay Migration Complete

## 📋 Tóm tắt thay đổi

### Backend (Spring Boot) ✅
1. **Configuration**
   - `VNPayConfig.java` - VNPay configuration với TMN_CODE, SECURE_HASH
   - `application-dev.properties` - Development config với PostgreSQL
   - `application-pord.properties` - Production config placeholders

2. **DTOs**
   - `VNPayPaymentRequest.java` - Request DTO cho create payment
   - `VNPayPaymentResponse.java` - Response DTO với payment URL
   - `VNPayReturnResponse.java` - Response DTO cho callback

3. **Service Layer**
   - `VNPayService.java` - Business logic
     - `createPayment()` - Tạo VNPay payment URL
     - `handleVNPayReturn()` - Xử lý callback từ VNPay
     - Verify signature với HMAC SHA512

4. **Controller**
   - `VNPayController.java` - REST API endpoints
     - `POST /api/vnpay/create-payment` - Tạo payment URL
     - `GET /api/vnpay/vnpay-return` - Callback từ VNPay (redirect về frontend)
     - `GET /api/vnpay/vnpay-ipn` - IPN notification

5. **Utilities**
   - `VNPayUtil.java` - Helper methods cho HMAC, URL building, formatting

### Frontend (React/TypeScript) ✅
1. **Payment Component Updated**
   - File: `DOMICARE_FRONTEND/src/pages/ALL/ProductDetail/components/Payment/Payment.tsx`
   - Thay đổi:
     ```typescript
     // OLD: Gọi Vercel Edge Function
     axios.post('/api/create-payment', ...)
     
     // NEW: Gọi Backend Spring Boot
     axiosClient.post('/vnpay/create-payment', ...)
     ```
   - Parse response: `response.data.data.paymentUrl`

2. **Payment Result Page**
   - File: `DOMICARE_FRONTEND/src/pages/ALL/PaymentResult/PaymentResult.tsx`
   - Route: `/payment` (đã có sẵn)
   - Nhận query params: `?status=success&orderInfo=...&amount=...`

## 🧪 Testing End-to-End

### 1. Start Backend
```bash
cd /Users/anhnon/DOMICARE/DomiCare
mvn spring-boot:run -Dspring-boot.run.profiles=dev
```
**Expected**: Server running on http://localhost:8080

### 2. Start Frontend
```bash
cd /Users/anhnon/DOMICARE/DOMICARE_FRONTEND
npm run dev
```
**Expected**: Frontend running on http://localhost:5173

### 3. Test Payment Flow

#### Step A: Create Payment (Backend Test)
```bash
curl -X POST http://localhost:8080/api/vnpay/create-payment \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 100000,
    "orderInfo": "Test payment for booking",
    "orderId": "TEST'$(date +%s)'"
  }'
```

**Expected Response:**
```json
{
  "status": 200,
  "data": {
    "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?..."
  }
}
```

#### Step B: Test from Frontend
1. Open http://localhost:5173
2. Navigate to Product Detail page
3. Click "Đặt dịch vụ" → Booking confirmation
4. Click "Đặt cọc ngay" button
5. **Expected**: 
   - Backend API called: `POST http://localhost:8080/api/vnpay/create-payment`
   - New tab opens with VNPay payment page

#### Step C: Complete Payment on VNPay
1. Use VNPay test card:
   - **Card Number**: `9704198526191432198`
   - **Card Holder**: `NGUYEN VAN A`
   - **Expiry**: `07/15`
   - **OTP**: `123456`

2. Submit payment

3. **Expected Flow**:
   ```
   VNPay → Backend callback: GET /api/vnpay/vnpay-return?vnp_Amount=...
   Backend verifies signature
   Backend redirects: http://localhost:5173/payment?status=success&orderInfo=...&amount=...
   Frontend displays: "Thanh toán thành công! 🎉"
   ```

## 🔄 Migration Checklist

- [x] Backend VNPay configuration setup
- [x] Backend DTOs created
- [x] Backend Service layer implemented
- [x] Backend Controller with 3 endpoints
- [x] Backend HMAC SHA512 signature verification
- [x] Frontend Payment component updated to call backend
- [x] Frontend axios client integration
- [x] Frontend PaymentResult page ready
- [x] Backend tested with curl ✅
- [ ] Frontend tested in browser
- [ ] End-to-end payment flow tested
- [ ] Remove `/api` folder after verification

## 📁 Files to Remove After Verification

```bash
cd /Users/anhnon/DOMICARE/DOMICARE_FRONTEND

# Remove Vercel Edge Function folder
rm -rf api/

# Verify no code references /api/vnpay or /api/create-payment
grep -r "/api/create-payment" src/
grep -r "/api/vnpay" src/
```

**Expected**: No matches found (all updated to use axiosClient)

## 🔧 Configuration Notes

### Development Environment
- **Backend**: http://localhost:8080
- **Frontend**: http://localhost:5173
- **Database**: PostgreSQL on AivenCloud
- **VNPay**: Sandbox environment

### Production Checklist
1. Update `application-pord.properties`:
   ```properties
   vnpay.tmn.code=${VNPAY_TMN_CODE}
   vnpay.secure.hash=${VNPAY_SECURE_HASH}
   vnpay.url=https://vnpayment.vn/paymentv2/vpcpay.html
   vnpay.return.url=https://your-backend.com/api/vnpay/vnpay-return
   vnpay.frontend.url=https://your-frontend.com
   ```

2. Environment Variables:
   ```bash
   export VNPAY_TMN_CODE=your_production_tmn_code
   export VNPAY_SECURE_HASH=your_production_secure_hash
   ```

## 📊 API Endpoints Summary

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/vnpay/create-payment` | Create VNPay payment URL |
| GET | `/api/vnpay/vnpay-return` | Handle VNPay callback (redirects to frontend) |
| GET | `/api/vnpay/vnpay-ipn` | IPN notification from VNPay |

## 🎯 Response Format

### Success Response
```json
{
  "status": 200,
  "error": null,
  "message": "Success",
  "data": {
    "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?..."
  }
}
```

### Error Response
```json
{
  "status": 500,
  "error": "Error message",
  "message": "Failed to create payment",
  "data": null
}
```

## 🔐 Security

1. **Signature Verification**: HMAC SHA512 với SECURE_HASH
2. **Environment Variables**: TMN_CODE và SECURE_HASH không hardcode
3. **HTTPS**: Production phải dùng HTTPS cho return URL
4. **CORS**: Backend đã config cho phép frontend origin

## ✅ Migration Status

**Status**: ✅ **COMPLETE - Ready for Testing**

**Next Steps**:
1. Start backend và frontend
2. Test payment flow từ browser
3. Verify VNPay callback redirect về frontend
4. Remove `/api` folder khi mọi thứ hoạt động
