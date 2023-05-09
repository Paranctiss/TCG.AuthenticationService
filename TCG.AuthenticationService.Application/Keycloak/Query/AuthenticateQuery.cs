using MediatR;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.CatalogService.Application.Keycloak.DTO.Request;

namespace TCG.AuthenticationService.Application.Keycloak.Query;

public record AuthenticateQuery(UserLogin UserLogin) : IRequest<string>;
public class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, string>
{
    private readonly IKeycloakRepository _keycloakService;
    private readonly ILogger _logger;
    
    public AuthenticateQueryHandler(IKeycloakRepository keycloakService, ILogger<AuthenticateQueryHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<string> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var accessToken = await _keycloakService.AuthenticateUserAsync(request.UserLogin);
            return accessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while authenticate user");
            throw;
        }
    }
}
