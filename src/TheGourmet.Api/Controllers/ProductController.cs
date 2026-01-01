using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Common.Models;
using TheGourmet.Application.Features.Products.Commands.CreateProduct;
using TheGourmet.Application.Features.Products.Commands.ToggleActiveProduct;
using TheGourmet.Application.Features.Products.Commands.UpdateProduct;
using TheGourmet.Application.Features.Products.Queries.GetProductById;
using TheGourmet.Application.Features.Products.Queries.GetProductsWithPagination;
using TheGourmet.Application.Features.Products.Results;
using TheGourmet.Domain.Entities;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductCommand command)
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
    
    // Get product by id
    [HttpGet("{id}")]
    public async Task<ActionResult<GetProductByIdResponse>> GetProductById([FromRoute] GetProductByIdQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
    // update info product 
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductCommand command, [FromRoute] Guid id)
    {
        command.Id = id;
        var result = await mediator.Send(command);
        return Ok(result);
    }

    // Toggler product status (active/inactive)
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/active")]
    public async Task<ActionResult> ToggleProductStatus([FromRoute] ToggleActiveProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}