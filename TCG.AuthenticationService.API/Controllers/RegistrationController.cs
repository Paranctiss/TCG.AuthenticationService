using MediatR;
using Microsoft.AspNetCore.Mvc;
using TCG.AuthenticationService.Application.Keycloak.Command;
using TCG.AuthenticationService.Application.Keycloak.Query;
using TCG.CatalogService.Application.Keycloak.DTO.Request;
using TCG.CatalogService.Application.Keycloak.DTO.Response;
using TCG.Common.Middlewares.MiddlewareException;

namespace TCG.AuthenticationService.Controllers;

[ApiController]
[Route("[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(IMediator mediator, ILogger<RegistrationController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] UserLogin userLogin)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var accessToken = await _mediator.Send(new AuthenticateQuery(userLogin));
            return Ok(new AuthenticationResponse { AccessToken = accessToken });
        }
        catch (Exception ex)
        {
            return StatusCode(401, "Invalid credentials.");
        }
    }
    
    [HttpGet("user-info")]
    public async Task<IActionResult> UserInfo()
    {
        string authorizationHeader = HttpContext.Request.Headers["Authorization"];

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        string token = authorizationHeader.Substring("Bearer ".Length);
        try
        {
            var userInfo = await _mediator.Send(new UserInfoQuery(token));
            return Ok(new { user = userInfo });
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "User not found in database");
            return StatusCode(404, "User not found in databse.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing your request.");
            return StatusCode(500, "An unexpected error occurred while processing your request.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegistration userRegistration)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _mediator.Send(new CreateUserCommand(userRegistration));
            return Ok();
        }
        catch (UserAlreadyExistsException ex)
        {
            return new StatusCodeResult(409);
        }
        catch (Exception ex)
        {
            return new StatusCodeResult(500);
        }
    }
}