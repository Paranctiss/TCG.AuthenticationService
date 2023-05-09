using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Domain;
using TCG.CatalogService.Application.Keycloak.DTO.Request;

namespace TCG.AuthenticationService.Application.Keycloak.Command;

public record CreateUserCommand(UserRegistration UserRegistration) : IRequest;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IKeycloakRepository _keycloakService;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(IKeycloakRepository keycloakService, ILogger<CreateUserCommandHandler> logger, IUserRepository userRepository, IMapper mapper)
    {
        _keycloakService = keycloakService;
        _logger = logger;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var accessToken = await _keycloakService.GetAdminAccessTokenAsync();

            await _userRepository.ExecuteInTransactionAsync(async () =>
            {
                request.UserRegistration.UserStateId = 1;
                await _keycloakService.CreateUserAsync(accessToken, request.UserRegistration);
                var userSub = await _keycloakService.GetUserIdAsync(accessToken, request.UserRegistration.Username);
                request.UserRegistration.Sub = Guid.Parse(userSub);
                await _userRepository.AddAsync(_mapper.Map<User>(request.UserRegistration), cancellationToken);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while adding user");
            throw;
        }
        
        return Unit.Value;
    }
}