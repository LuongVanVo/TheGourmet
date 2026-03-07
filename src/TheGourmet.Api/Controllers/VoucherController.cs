using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TheGourmet.Application.Features.Vouchers.Commands.CreateVoucher;
using TheGourmet.Application.Features.Vouchers.Commands.ToggleStatusVoucher;
using TheGourmet.Application.Features.Vouchers.Commands.UpdateVoucher;
using TheGourmet.Application.Features.Vouchers.Queries.GetVouchers;

namespace TheGourmet.Api.Controllers;

[Route("api/{controller}")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly IMediator _mediator;

    public VoucherController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // Create voucher
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Add a new voucher",
        Description = "Add a new voucher to the system. Only accessible by Admin.")]
    public async Task<IActionResult> CreateVoucher([FromBody] CreateVoucherCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // Update voucher
    [Authorize(Roles = "Admin")]
    [HttpPatch]
    [SwaggerOperation(Summary = "Update info of a voucher",
        Description = "Update info of a voucher. Only accessible by Admin.")]
    public async Task<IActionResult> UpdateVoucher([FromBody] UpdateVoucherCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // Toggle status of a voucher
    [Authorize(Roles = "Admin")]
    [HttpPatch("toggle-status")]
    [SwaggerOperation(Summary = "Toggle status of a voucher",
        Description = "Toggle status of a voucher. Only accessible by Admin.")]
    public async Task<IActionResult> ToggleStatusVoucher([FromBody] ToggleStatusVoucherCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    // Get vouchers with pagination and search
    [Authorize]
    [HttpGet]
    [SwaggerOperation(Summary = "Get vouchers with pagination and search",
        Description = "Get vouchers with pagination and search. Accessible by all authenticated users.")]
    public async Task<IActionResult> GetVouchers([FromQuery] GetVouchersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}