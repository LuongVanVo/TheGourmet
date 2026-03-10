using Microsoft.AspNetCore.Http;
using TheGourmet.Application.DTOs.Payment;

namespace TheGourmet.Application.Interfaces;

public interface IVNPayService
{
    string CreatePaymentUrl(PaymentInformationModel model, string ipAddress);
    PaymentResponseModel PaymentExecute(IQueryCollection query);
}