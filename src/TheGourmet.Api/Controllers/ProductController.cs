using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.Features.Products.Commands.CreateProduct;
using TheGourmet.Application.Features.Products.Queries.GetProductsWithPagination;
using TheGourmet.Application.Features.Products.Results;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    // Get products with pagination
    [HttpGet]
    public async Task<ActionResult<PaginatedList<GetProductsWithPaginationResponse>>> GetProducts(
        [FromQuery] GetProductsWithPaginationQuery query)
    {
        return await mediator.Send(query);
    }
}