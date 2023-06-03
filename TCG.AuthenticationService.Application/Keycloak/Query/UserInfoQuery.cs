using MediatR;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Domain;
using TCG.CatalogService.Application.Keycloak.DTO.Request;
using TCG.Common.Middlewares.MiddlewareException;

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

            // Check if the user exists in the database
            if (userInfo == null)
            {
                // User does not exist in the database
                throw new NotFoundException("User does not exist in the database");
            }

            return userInfo;
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "User not found in database");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting user");
            throw;
        }
    }
}
