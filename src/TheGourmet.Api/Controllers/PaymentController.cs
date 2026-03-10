using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Api.Helper;
using TheGourmet.Application.Features.Payments.Commands.CreatePaymentUrl;
using TheGourmet.Application.Features.Payments.Commands.ProcessVnPayIpn;
using TheGourmet.Application.Features.Payments.Queries.GetAllPayments;
using TheGourmet.Application.Features.Payments.Queries.GetPaymentByOrderId;
using TheGourmet.Application.Features.Payments.Queries.GetVnPayReturnResult;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("create-url/{orderId}")]
    [SwaggerOperation(Summary = "Create a payment URL for the specified order.",
        Description = "Generates a payment URL that the user can use to complete the payment for the specified order.")]
    public async Task<IActionResult> CreatePaymentUrl(Guid orderId)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        var command = new CreatePaymentUrlCommand
        {
            OrderId = orderId,
            IpAddress = ipAddress,
            UserId = User.GetCurrentUserId()
        };

        var paymentUrl = await _mediator.Send(command);

        return Ok(new
        {
            success = true,
            paymentUrl
        });
    }

    [AllowAnonymous]
    [HttpGet("vnpay-ipn")]
    [SwaggerOperation(Summary = "Handle VnPay IPN notifications.",
        Description = "Processes Instant Payment Notifications (IPN) sent by VnPay to update the payment status of orders accordingly.")]
    public async Task<IActionResult> VnPayIpn()
    {
        var command = new ProcessVnPayIpnCommand
        {
            QueryData = Request.Query
        };

        var responseJson = await _mediator.Send(command);

        return Content(responseJson, "application/json");
    }

    [AllowAnonymous]
    [HttpGet("vnpay-return")]
    [SwaggerOperation(Summary = "Handle VnPay return URL.",
        Description =
            "Handles the return URL that VnPay redirects to after payment completion, allowing the application to display the appropriate success or failure message to the user.")]
    public async Task<IActionResult> VnPayReturn()
    {
        var query = new GetVnPayReturnResultQuery
        {
            QueryData = Request.Query
        };

        var paymentResult = await _mediator.Send(query);

        if (paymentResult.Success)
        {
            return Ok(new
            {
                Success = true,
                Message = "Payment successful. Thank you for your purchase!",
                TransactionId = paymentResult.TransactionId,
                OrderId = paymentResult.OrderId,
                VnPayResponseCode = paymentResult.VnPayResponseCode
            });
        }

        return BadRequest(new
        {
            Success = false,
            Message = "Payment failed. Please try again or contact support if the issue persists.",
            TransactionId = paymentResult.TransactionId,
            OrderId = paymentResult.OrderId,
            VnPayResponseCode =  paymentResult.VnPayResponseCode
        });
    }

    [Authorize]
    [HttpGet("order/{orderId}")]
    [SwaggerOperation(Summary = "Get payment information by order ID.",
        Description = "Retrieves the payment information associated with a specific order, allowing users to view the payment status and details for their orders.")]
    public async Task<IActionResult> GetPaymetnByOrderid(Guid orderId)
    {
        var query = new GetPaymentByOrderIdQuery
        {
            OrderId = orderId,
            UserId = User.GetCurrentUserId()
        };

        var payment = await _mediator.Send(query);

        return Ok(new
        {
            Success = true,
            Data = payment
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("all")]
    [SwaggerOperation(Summary = "Get all payments (Admin only).",
        Description =
            "Allows administrators to retrieve a list of all payments made in the system, providing insights into payment history and status for all orders.")]
    public async Task<IActionResult> GetAllPayments()
    {
        var query = new GetAllPaymentsQuery();
        var payments = await _mediator.Send(query);

        var totalRevenue = payments
            .Where(p => p.Status == "Success")
            .Sum(p => p.Amount);

        return Ok(new
        {
            Success = true,
            TotalTransactions = payments.Count(),
            TotalRevenue = totalRevenue,
            Data = payments
        });
    }
}