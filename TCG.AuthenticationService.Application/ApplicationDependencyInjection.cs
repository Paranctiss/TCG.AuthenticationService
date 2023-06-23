using System.Reflection;
using GreenPipes;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCG.AuthenticationService.Application.Consumer;
using TCG.Common.Settings;

namespace TCG.CatalogService.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        return services.AddMediatR(assembly);
    }

    public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection serviceCollection)
    {
        //Config masstransit to rabbitmq
        serviceCollection.AddMassTransit(configure =>
        {
            configure.AddConsumer<UserByIdConsumer>();
            configure.AddConsumer<UserByTokenConsumer>();
            configure.UsingRabbitMq((context, configurator) =>
            {
                var config = context.GetService<IConfiguration>();
                //On récupère le nom de la table Catalog
                ////On recupère la config de seeting json pour rabbitMQ
                var rabbitMQSettings = config.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                configurator.Host(rabbitMQSettings.Host);
                configurator.ConfigureEndpoints(context);
                //Defnir comment les queues sont crées dans rabbit
                configurator.ReceiveEndpoint("authservice", e =>
                {
                    e.UseMessageRetry(r => r.Interval(2, 3000));
                    e.ConfigureConsumer<UserByIdConsumer>(context);
                });
                configurator.ReceiveEndpoint("authservice2", e =>
                {
                    e.UseMessageRetry(r => r.Interval(2, 3000));
                    e.ConfigureConsumer<UserByTokenConsumer>(context);
                });
            });
        });
        //Start rabbitmq bus pour exanges
        serviceCollection.AddMassTransitHostedService();
        return serviceCollection;
    }
}