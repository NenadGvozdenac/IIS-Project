using Microsoft.AspNetCore.Rewrite;
using scouting_service.src.Scoutings.API.Startup;
using scouting_service.src.Elasticsearch.Services;
using Nest;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.ConfigureSwagger(builder.Configuration);

// Elasticsearch configuration
var elasticsearchUrl = builder.Configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
    .DefaultIndex("players")
    .ThrowExceptions();

builder.Services.AddSingleton<IElasticClient>(sp => new ElasticClient(settings));
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddScoped<IPlayerSearchService, PlayerSearchService>();
builder.Services.AddScoped<ISessionSearchService, SessionSearchService>();
builder.Services.AddScoped<IDataSyncService, DataSyncService>();

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
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(corsPolicy);
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();