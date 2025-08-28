using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ticket_service.src.Tickets.API.Startup;

public static class AuthConfiguration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        ConfigureAuthentication(services);
        ConfigureAuthorizationPolicies(services);
        return services;
    }

    public static void ConfigureAuthentication(IServiceCollection services)
    {
        var key = "Pq5s9XJ68zQWJY8h2Nx2Q9sYyQJdJf2zEwRZp9LrXUs=";
        var issuer = "iis";
        var audience = "iis";

        var roleClaimType = "userRole";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    TryAllIssuerSigningKeys = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    RoleClaimType = roleClaimType,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Tried JWT: " + context.Request.Headers["Authorization"]);
                        Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                        Console.WriteLine("Error: " + context.Exception.StackTrace);

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
    }

    private static void ConfigureAuthorizationPolicies(IServiceCollection services) =>
        services.AddAuthorizationBuilder()
            .AddPolicy("adminPolicy", policy => policy.RequireRole("admin"));
}