using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Api.Helper;
using TheGourmet.Application.Features.Orders.Commands.CancelOrder;
using TheGourmet.Application.Features.Orders.Commands.ConfirmReceipt;
using TheGourmet.Application.Features.Orders.Commands.CreateOrder;
using TheGourmet.Application.Features.Orders.Queries.GetCancelReasons;
using TheGourmet.Application.Features.Orders.Queries.GetOrderPreview;
using TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;
using TheGourmet.Domain.Enums;

namespace TheGourmet.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new order for the current user.")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("user")]
    [SwaggerOperation(Summary = "Get orders of the current user, optionally filtered by status.")]
    public async Task<IActionResult> GetOrdersByUserId([FromQuery] OrderStatus? status)
    {
        var query = new GetOrdersByUserIdQuery
        {
            UserId = User.GetCurrentUserId(),
            Status = status
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    // Cancel order 
    [HttpPatch("cancel/{orderId}")]
    [SwaggerOperation(Summary = "Cancel an order by ID for the current user.")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderCommand command, [FromRoute]Guid orderId)
    {
        command.UserId = User.GetCurrentUserId();
        command.OrderId = orderId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // Preview order before create order
    [HttpPost("preview")]
    public async Task<IActionResult> PreviewOrder([FromBody] GetOrderPreviewQuery command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // Get all Cancel Order reason
    [HttpGet("cancel-reasons")]
    public async Task<IActionResult> GetAllOrderReason()
    {
        var result = await _mediator.Send(new GetCancelReasonsQuery());
        return Ok(result);
    } 
    
    // Confirm receipt of order
    [HttpPatch("confirm-receipt/{orderId}")]
    [SwaggerOperation(Summary = "Confirm receipt of an order by ID for the current user.")]
    public async Task<IActionResult> ConfirmReceipt([FromRoute]Guid orderId)
    {
        var command = new ConfirmReceiptCommand();
        command.UserId = User.GetCurrentUserId();
        command.Id = orderId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}