using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using Flix.Application;
using Flix.Application.Services;
using Flix.Infrastructure.Repositories;
using Flix.Domain.Repositories;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

string? cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosDb__Endpoint");
string? cosmosKey = Environment.GetEnvironmentVariable("CosmosDb__Key");
string? cosmosDatabase = Environment.GetEnvironmentVariable("CosmosDb__DatabaseName");
string? cosmosContainer = Environment.GetEnvironmentVariable("CosmosDb__ContainerName");

CosmosClient cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);

builder.Services.AddSingleton(cosmosClient);
builder.Services.AddScoped<IMovieRepository>(sp =>
    new CosmosMovieRepository(cosmosClient, cosmosDatabase, cosmosContainer));
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Build().Run();
