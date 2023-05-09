using TCG.AuthenticationService.Domain;
using TCG.Common.Contracts;

namespace TCG.AuthenticationService.Application.Contracts;

public interface ICountryRepository : IRepository<Domain.Country>
{
    
}