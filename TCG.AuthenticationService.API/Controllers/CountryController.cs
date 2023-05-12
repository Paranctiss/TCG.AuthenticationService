using MediatR;
using Microsoft.AspNetCore.Mvc;
using TCG.AuthenticationService.Application.Country.Query;
using TCG.AuthenticationService.Application.Keycloak.Command;
using TCG.AuthenticationService.Application.Keycloak.Query;
using TCG.CatalogService.Application.Keycloak.DTO.Request;
using TCG.CatalogService.Application.Keycloak.DTO.Response;
using TCG.Common.Middlewares.MiddlewareException;

namespace TCG.AuthenticationService.Controllers;

[ApiController]
[Route("[controller]")]
public class CountryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CountryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<IActionResult> Register(CancellationToken cancellationToken)
    {
        try
        {
            var countries = await _mediator.Send(new GetAllCountryQuery(), cancellationToken);
            return Ok(countries);
        }
        catch (Exception ex)
        {
            return new StatusCodeResult(500);
        }
    }
}