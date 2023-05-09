using MediatR;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Domain;
using TCG.CatalogService.Application.Keycloak.DTO.Request;

namespace TCG.AuthenticationService.Application.Keycloak.Query;

public record UserInfoQuery(TokenRequest token) : IRequest<User>;

public class UserInfoQueryHandler : IRequestHandler<UserInfoQuery, User>
{
    private readonly IKeycloakRepository _keycloakService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;
    
    public UserInfoQueryHandler(IKeycloakRepository keycloakService, ILogger<UserInfoQueryHandler> logger, IUserRepository userRepository)
    {
        _keycloakService = keycloakService;
        _userRepository = userRepository;
        _logger = logger;
    }


    public async Task<User> Handle(UserInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userSub = await _keycloakService.GetUserInfoAsync(request.token.Token);
            var userInfo = await _userRepository.GetSub(userSub, cancellationToken);
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while getting user");
            throw;
        }
    }
}