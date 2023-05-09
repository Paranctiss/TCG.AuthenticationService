using TCG.AuthenticationService.Domain;
using TCG.Common.Contracts;

namespace TCG.AuthenticationService.Application.Contracts;

public interface IUserRepository : IRepository<User>
{
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);
    Task<User> GetSub(Guid guid, CancellationToken cancellationToken);
}