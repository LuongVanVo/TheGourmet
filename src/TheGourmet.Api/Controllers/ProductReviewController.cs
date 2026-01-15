using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Api.Helper;
using TheGourmet.Application.Features.ProductReviews.Commands.CreateReview;
using TheGourmet.Application.Features.ProductReviews.Queries.GetProductReviews;

namespace TheGourmet.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }
    // get review by product id
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProductReviewsByProductId([FromRoute] GetProductReviewsQuery command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // create review
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewCommand command)
    {
        command.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}