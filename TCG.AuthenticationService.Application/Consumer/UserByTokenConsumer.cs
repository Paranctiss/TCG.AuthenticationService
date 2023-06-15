using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Application.Keycloak.Query;
using TCG.Common.MassTransit.Messages;
using TCG.Common.Middlewares.MiddlewareException;

namespace TCG.AuthenticationService.Application.Consumer;

public class UserByTokenConsumer : IConsumer<UserByToken>
{
    private readonly IUserRepository _userRepository; 
    private readonly IKeycloakRepository _keycloakService;


    public UserByTokenConsumer(IUserRepository repository, IKeycloakRepository keycloakService)
    {
        _userRepository = repository;
        _keycloakService = keycloakService;
    }

    public async Task Consume(ConsumeContext<UserByToken> context)
    {
        var message = context.Message;
        if (message != null)
        {
            string token = message.Token;
        

            var userSub = await _keycloakService.GetUserInfoAsync(token);
            var userInfo = await _userRepository.GetSub(userSub, message.cancellationToken);

            if (userInfo == null)
            {
                // User does not exist in the database
                throw new NotFoundException("User does not exist in the database");
            }

            var response = new UserByTokenResponse(userInfo.Id);
            await context.RespondAsync(response);
        }
    }
}
