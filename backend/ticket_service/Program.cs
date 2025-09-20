using Microsoft.AspNetCore.Rewrite;
using ticket_service.src.Tickets.API.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);

const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();
builder.Services.ConfigureApplication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket Service API v1");
        c.SwaggerEndpoint("/swagger/neo4j/swagger.json", "Ticket Service Neo4j API v1");
        c.DefaultModelsExpandDepth(-1); // Hide models section
    });
}

app.UseRouting();
app.UseCors(corsPolicy);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();