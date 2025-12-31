using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Features.Categories.Commands.CreateCategory;
using TheGourmet.Application.Features.Categories.Queries.GetAllCategories;
using TheGourmet.Application.Features.Categories.Results;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategoryCommand categoryCommand)
    {
        var result =  await mediator.Send(categoryCommand);
        return Ok(result);
    }
    
    // get all categories endpoint
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await mediator.Send(query);
        
        return Ok(result);
    }
}