using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Features.Categories.Commands.CreateCategory;
using TheGourmet.Application.Features.Categories.Commands.DeleteCategory;
using TheGourmet.Application.Features.Categories.Commands.UpdateCategory;
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
    
    // update category 
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<ActionResult<UpdateCategoryResponse>> UpdateCategory([FromBody] UpdateCategoryCommand command,
        [FromRoute] Guid id)
    {
        command.Id = id;
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    // delete category (soft delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory([FromRoute]DeleteCategoryCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}