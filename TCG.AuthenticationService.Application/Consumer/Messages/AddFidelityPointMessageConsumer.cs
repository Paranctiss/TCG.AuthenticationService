using MassTransit;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.Common.MassTransit.Events;
using TCG.Common.MassTransit.Messages;

namespace TCG.AuthenticationService.Application.Consumer.Messages;

public class AddFidelityPointMessageConsumer : IConsumer<AddFidelityPointMessage>
{
    private readonly ILogger<AddFidelityPointMessageConsumer> _logger;
    private readonly IUserRepository _userRepository;

    public AddFidelityPointMessageConsumer(ILogger<AddFidelityPointMessageConsumer> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<AddFidelityPointMessage> context)
    {
        var userPoint = await _userRepository.GetByUserId(context.Message.UserId);

        if (userPoint.FidelityPoint >= 10)
        {
            await context.Publish(new FidelityPointFailedEvent(
                CorrelationId: context.Message.CorrelationId,
                ErrorMessage: "Seuil de fidelité atteint",
                UserId: context.Message.UserId)
            ); 
        }
        else
        {
            await context.Publish(new FidelityPointCompletedEvent(
                CorrelationId: context.Message.CorrelationId)
            );
            userPoint.FidelityPoint += 5;
            await _userRepository.UpdateAsync(userPoint, context.CancellationToken);
        }
    }
}