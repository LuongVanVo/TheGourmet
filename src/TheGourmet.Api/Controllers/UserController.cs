using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheGourmet.Application.Features.Auth.Queries.GetUserProfile;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICookieService _cookieService;
        private readonly IMediator _mediator;
        public UserController(ICookieService cookieService, IMediator mediator)
        {
            _cookieService = cookieService;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub); 

            var query = new GetUserProfileQuery
            {
                UserId = userId ?? string.Empty,
            };
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}