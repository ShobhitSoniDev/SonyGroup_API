using Jewellery.API.Controllers;
using Jewellery.Application.Auth;
using Jewellery.Application.Master.Commands;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly JwtTokenService _jwtService;
    private readonly IMediator _mediator;

    public AuthController(JwtTokenService jwtService, IMediator mediator)
    {
        _jwtService = jwtService;
        _mediator = mediator;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
