using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Features.Carts.Commands.AddItemToCart;
using TheGourmet.Application.Features.Carts.Commands.ClearItemToCart;
using TheGourmet.Application.Features.Carts.Commands.UpdateQuantityProductInCart;
using TheGourmet.Application.Features.Carts.Queries.GetCart;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetCurrentUserId();
        var query = new GetCartQuery(userId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItemToCart([FromBody] AddItemToCartCommand command)
    {
        command.UserId = GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
    
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token");
    
        return Guid.Parse(userIdClaim);
    }
    
    // update quantity product item in cart
    [HttpPatch("items/{productId}")]
    public async Task<IActionResult> UpdateItemProductQuantity([FromRoute] Guid productId, [FromBody] UpdateQuantityProductInCartCommand command)
    {
        command.ProductId = productId;
        command.UserId = GetCurrentUserId();

        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // remove product item from cart
    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart([FromRoute] Guid productId)
    {
        var command = new UpdateQuantityProductInCartCommand
        {
            UserId = GetCurrentUserId(),
            ProductId = productId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // clear cart 
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var command = new ClearItemToCartCommand
        {
            UserId = GetCurrentUserId()
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}