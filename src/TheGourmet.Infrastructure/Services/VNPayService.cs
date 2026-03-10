using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TheGourmet.Application.DTOs.Payment;
using TheGourmet.Application.Interfaces;
using TheGourmet.Infrastructure.Payments;

namespace TheGourmet.Infrastructure.Services;

public class VNPayService : IVNPayService
{
    private readonly IConfiguration _configuration;
    public VNPayService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string CreatePaymentUrl(PaymentInformationModel model, string ipAddress)
    {
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var pay = new VNPayLibrary();
        
        pay.AddRequestData("vnp_Version", _configuration["VNPay:Version"] ?? string.Empty);
        pay.AddRequestData("vnp_Command", _configuration["VNPay:Command"] ?? string.Empty);
        pay.AddRequestData("vnp_TmnCode", _configuration["VNPay:TmnCode"] ?? string.Empty);
        pay.AddRequestData("vnp_Amount", ((long)model.Amount * 100).ToString());
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _configuration["VNPay:CurrCode"] ?? string.Empty);
        pay.AddRequestData("vnp_IpAddr", ipAddress);
        pay.AddRequestData("vnp_Locale", _configuration["VNPay:Locale"] ?? string.Empty);
        pay.AddRequestData("vnp_OrderInfo", $"{model.Name} thanh toan don hang {model.OrderId}");
        pay.AddRequestData("vnp_OrderType", model.OrderType);
        pay.AddRequestData("vnp_ReturnUrl", _configuration["VNPay:ReturnUrl"] ?? string.Empty);
        pay.AddRequestData("vnp_TxnRef", model.TransactionId);
        
        return pay.CreateRequestUrl(_configuration["VNPay:BaseUrl"] ?? string.Empty, _configuration["VNPay:HashSecret"] ?? string.Empty);
    }

    public PaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var pay = new VNPayLibrary();
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                pay.AddResponseData(key, value.ToString());
            }
        }

        var vnp_OrderId = pay.GetResponseData("vnp_TxnRef");
        var vnp_TransactionId = pay.GetResponseData("vnp_TransactionNo");
        var vnp_Securehash = collections.FirstOrDefault(x => x.Key == "vnp_SecureHash").Value;
        var vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
        
        // Truyền HashSecret từ settings vào hàm Validate
        bool checkSignature = pay.ValidateSignature(vnp_Securehash, _configuration["VNPay:HashSecret"] ?? string.Empty);

        if (!checkSignature)
        {
            return new PaymentResponseModel { Success = false };
        }

        return new PaymentResponseModel
        {
            Success = vnp_ResponseCode == "00",
            PaymentMethod = "VNPAY",
            OrderDescription = pay.GetResponseData("vnp_OrderInfo"),
            OrderId = vnp_OrderId,
            TransactionId = vnp_TransactionId,
            Token = vnp_Securehash,
            VnPayResponseCode = vnp_ResponseCode
        };
    }
}