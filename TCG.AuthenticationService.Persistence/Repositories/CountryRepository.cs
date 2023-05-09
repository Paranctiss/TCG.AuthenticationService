using Microsoft.EntityFrameworkCore;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Domain;
using TCG.Common.MySqlDb;

namespace TCG.AuthenticationService.Persistence.Repositories;

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ServiceDbContext dbContext) : base(dbContext)
    {
    }
}