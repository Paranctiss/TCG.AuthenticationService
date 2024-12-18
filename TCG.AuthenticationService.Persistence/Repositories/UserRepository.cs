using Microsoft.EntityFrameworkCore;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Domain;
using TCG.Common.MySqlDb;

namespace TCG.AuthenticationService.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    protected readonly ServiceDbContext _dbContext;

    public UserRepository(ServiceDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User> GetSub(Guid guid, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.Where(x => x.Sub == guid).FirstAsync();
    }
    
    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        using (var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

}