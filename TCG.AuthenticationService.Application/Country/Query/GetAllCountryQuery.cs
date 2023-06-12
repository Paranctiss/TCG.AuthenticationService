using MediatR;
using Microsoft.Extensions.Logging;
using TCG.AuthenticationService.Application.Contracts;
using TCG.AuthenticationService.Application.Keycloak.Query;
using TCG.AuthenticationService.Domain;
using TCG.CatalogService.Application.Keycloak.DTO.Request;

namespace TCG.AuthenticationService.Application.Country.Query;

public record GetAllCountryQuery() : IRequest<IEnumerable<Domain.Country>>;

public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, IEnumerable<Domain.Country>>
{
    private readonly ICountryRepository _countryRepository;
    private readonly ILogger _logger;
    
    public GetAllCountryQueryHandler(ICountryRepository countryRepository, ILogger<GetAllCountryQueryHandler> logger)
    {
        _countryRepository = countryRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Country>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var countries = await _countryRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Seri Log is Working");
            return countries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error while getting user");
            throw;
        }
    }
}