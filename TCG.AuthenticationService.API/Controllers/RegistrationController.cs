using MediatR;
using Microsoft.AspNetCore.Mvc;
using TCG.AuthenticationService.Application.Keycloak.Command;
using TCG.AuthenticationService.Application.Keycloak.Query;
using TCG.AuthenticationService.Domain;
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

    [HttpGet("profile/{idUser}")]
    public async Task<IActionResult> GetProfileInfos(int idUser, [FromHeader] string authorization = "")
    {
        User CurrentUserInfos = new User();

        if(authorization != "")
        {
            try
            {
                string token = authorization.Substring("Bearer ".Length);
                CurrentUserInfos = await _mediator.Send(new UserInfoQuery(token));
            }
            catch (Exception e) { }
            finally
            {
                CurrentUserInfos.Id = 0;
            }
               
           
        }
        else
        {
            CurrentUserInfos.Id = 0;
        }
            
            var userProfileInfos = await _mediator.Send(new GetUserProfileQuery(idUser));

        if(CurrentUserInfos.Id == idUser)
        {
            userProfileInfos.IsOwner = true;
        }
        else
        {
            userProfileInfos.IsOwner = false;
        }

        return Ok(userProfileInfos);
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