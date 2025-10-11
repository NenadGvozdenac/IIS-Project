using Microsoft.Net.Http.Headers;

namespace scouting_service.src.Scoutings.API.Startup;

public static class CorsConfiguration
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services, string corsPolicy)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: corsPolicy,
                builder =>
                {
                    builder.WithOrigins(ParseCorsOrigins())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
        return services;
    }

    private static string[] ParseCorsOrigins()
    {
        var corsOrigins = new[] {
            "http://localhost:5173",   // Frontend Vite dev server  
            "https://localhost:5173",  // Frontend Vite dev server (HTTPS)
            "http://localhost:5002",   // Scouting service HTTP
            "https://localhost:5003",  // Scouting service HTTPS
            "http://localhost:3000",   // Common frontend port
            "https://localhost:3000"   // Common frontend port (HTTPS)
        };
        var corsOriginsPath = Environment.GetEnvironmentVariable("EXPLORER_CORS_ORIGINS");
        if (File.Exists(corsOriginsPath))
        {
            corsOrigins = File.ReadAllLines(corsOriginsPath);
        }

        return corsOrigins;
    }
}