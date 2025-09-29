using MassTransit;
using match_service.src.Matches.Core.Application.Saga;
using match_service.src.Matches.Core.Application.Saga.Consumers;

namespace match_service.src.Matches.API.Startup;

public static class MassTransitConfiguration
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            // Add saga
            x.AddSagaStateMachine<DeleteTeamMemberSaga, DeleteTeamMemberSagaState>()
                .InMemoryRepository(); // For development; use Entity Framework or another persistent store in production

            // Add consumers
            x.AddConsumer<DeleteMatchDataConsumer>();
            x.AddConsumer<DeleteInfluxEventsConsumer>();
            x.AddConsumer<DeleteTeamMemberConsumer>();
            x.AddConsumer<RollbackMatchDataConsumer>();
            x.AddConsumer<RollbackInfluxEventsConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@rabbitmq:5672";
                
                cfg.Host(rabbitMqHost);

                // Configure saga endpoint
                cfg.ReceiveEndpoint("team-member-deletion-saga", e =>
                {
                    e.ConfigureSaga<DeleteTeamMemberSagaState>(context);
                });

                // Configure consumers
                cfg.ReceiveEndpoint("delete-match-data", e =>
                {
                    e.ConfigureConsumer<DeleteMatchDataConsumer>(context);
                });

                cfg.ReceiveEndpoint("delete-influx-events", e =>
                {
                    e.ConfigureConsumer<DeleteInfluxEventsConsumer>(context);
                });

                cfg.ReceiveEndpoint("delete-team-member", e =>
                {
                    e.ConfigureConsumer<DeleteTeamMemberConsumer>(context);
                });

                cfg.ReceiveEndpoint("rollback-match-data", e =>
                {
                    e.ConfigureConsumer<RollbackMatchDataConsumer>(context);
                });

                cfg.ReceiveEndpoint("rollback-influx-events", e =>
                {
                    e.ConfigureConsumer<RollbackInfluxEventsConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}