namespace scouting_service.src.Scoutings.API.Startup;

public static class ApplicationStartup
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        SetupDatabases(services);

        return services;
    }

    private static void SetupDatabases(IServiceCollection services)
    {
    }
}