using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ticket_service.src.Tickets.API.Startup;

public static class SwaggerConfiguration
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(setup =>
        {
            // Regular API Document
            setup.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ticket Service API",
                Version = "v1",
                Description = "Regular relational database operations"
            });

            // Neo4j API Document  
            setup.SwaggerDoc("neo4j", new OpenApiInfo
            {
                Title = "Ticket Service Neo4j API",
                Version = "v1",
                Description = "Neo4j graph database operations"
            });

            // Configure document filters to separate controllers
            setup.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "v1")
                {
                    // Include only non-Neo4j controllers (controllers not in NAIS folder)
                    return !apiDesc.RelativePath?.StartsWith("api/neo4j") == true;
                }
                else if (docName == "neo4j")
                {
                    // Include only Neo4j controllers (controllers in NAIS folder with /api/neo4j route)
                    return apiDesc.RelativePath?.StartsWith("api/neo4j") == true;
                }
                return false;
            });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Description = "Put **ONLY** your JWT Bearer token in the text box below!",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
        return services;
    }
}