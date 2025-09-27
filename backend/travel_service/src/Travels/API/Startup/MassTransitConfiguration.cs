using MassTransit;
using travel_service.src.Travels.Core.Application.Saga.Consumers;

namespace travel_service.src.Travels.API.Startup;

public static class MassTransitConfiguration
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Add consumers
            x.AddConsumer<DeleteTravelDataConsumer>();
            x.AddConsumer<RollbackTravelDataConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672";
                
                cfg.Host(rabbitMqHost);

                // Configure consumers
                cfg.ReceiveEndpoint("delete-travel-data", e =>
                {
                    e.ConfigureConsumer<DeleteTravelDataConsumer>(context);
                });

                cfg.ReceiveEndpoint("rollback-travel-data", e =>
                {
                    e.ConfigureConsumer<RollbackTravelDataConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}