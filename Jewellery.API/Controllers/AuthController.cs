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

        if (result.Code == 1 && result.Data != null)
        {
            string token = null;

            if (result.Data is IDictionary<string, object> dataDict)
            {
                if (dataDict.TryGetValue("token", out var tokenValue))
                {
                    token = tokenValue?.ToString();
                }
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddDays(1),
                    Path = "/"
                });
            }
        }

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
    [HttpPost("ConvertPassword")]
    public async Task<IActionResult> ConvertPassword([FromBody] ConvertPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
