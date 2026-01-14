using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Application.Features.Categories.Commands.CreateCategory;
using TheGourmet.Application.Features.Categories.Commands.DeleteCategory;
using TheGourmet.Application.Features.Categories.Commands.UpdateCategory;
using TheGourmet.Application.Features.Categories.Queries.GetAllCategories;
using TheGourmet.Application.Features.Categories.Queries.GetCategoryTree;
using TheGourmet.Application.Features.Categories.Results;

namespace TheGourmet.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new category")]
    public async Task<IActionResult> Post([FromBody] CreateCategoryCommand categoryCommand)
    {
        var result =  await mediator.Send(categoryCommand);
        return Ok(result);
    }
    
    // get all categories endpoint
    [HttpGet]
    [SwaggerOperation(Summary = "Get all categories")]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
    {
        var query = new GetAllCategoriesQuery();
        var result = await mediator.Send(query);
        
        return Ok(result);
    }
    
    // update category 
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    [SwaggerOperation(Summary = "Update a category by id")]
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
    [SwaggerOperation(Summary = "Delete a category by id")]
    public async Task<ActionResult> DeleteCategory([FromRoute]DeleteCategoryCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    // get category tree 
    [HttpGet("tree")]
    [SwaggerOperation(Summary = "Get category tree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        var query = new GetCategoryTreeQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}