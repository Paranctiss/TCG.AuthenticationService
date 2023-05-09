using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using TCG.AuthenticationService.Domain;
using TCG.AuthenticationService.Persistence.ExternalsApi.KeycloakExternalApi.ModelsKeycloakExternalApi;

namespace TCG.AuthenticationService.Persistence.DependencyInjection;

public static class AddMapping
{
    public static IServiceCollection AddMapper(this IServiceCollection services, string configName)
    {
        var config = new TypeAdapterConfig();
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}