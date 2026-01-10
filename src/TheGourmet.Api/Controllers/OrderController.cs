using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Api.Helper;
using TheGourmet.Application.Features.Orders.Commands.CreateOrder;
using TheGourmet.Application.Features.Orders.Queries.GetOrdersByUserId;

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
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("user")]
    public async Task<IActionResult> GetOrdersByUserId()
    {
        var query = new GetOrdersByUserIdQuery
        {
            UserId = User.GetCurrentUserId()
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}