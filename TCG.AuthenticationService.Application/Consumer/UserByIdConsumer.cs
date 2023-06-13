using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCG.AuthenticationService.Application.Contracts;
using TCG.Common.MassTransit.Messages;

namespace TCG.AuthenticationService.Application.Consumer
{
    public class UserByIdConsumer : IConsumer<UserById>
    {
        private readonly IUserRepository _userRepository;

        public UserByIdConsumer(IUserRepository repository)
        {
            _userRepository= repository;
        }

        public async Task Consume(ConsumeContext<UserById> context)
        {
            var message = context.Message;

            var user = await _userRepository.GetByUserId(message.idUser);
            var response = new UserByIdResponse(user.Id, user.Username);
            await context.RespondAsync(response);
        }

    }
}
