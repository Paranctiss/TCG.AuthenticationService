using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace TCG.CatalogService.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return services.AddMediatR(assembly);
    }
}