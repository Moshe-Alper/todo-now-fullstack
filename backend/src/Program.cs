using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow localhost
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Production: Allow configured frontend URL or any origin (for flexibility)
            // To restrict to specific URL, set "Cors:AllowedOrigins" in appsettings.json or Azure App Settings
            var corsSection = builder.Configuration.GetSection("Cors:AllowedOrigins");
            var allowedOrigins = new List<string>();
            
            if (corsSection.Exists())
            {
                foreach (var child in corsSection.GetChildren())
                {
                    var value = child.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        allowedOrigins.Add(value);
                    }
                }
            }
            
            if (allowedOrigins.Count > 0)
            {
                policy.WithOrigins(allowedOrigins.ToArray())
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            }
            else
            {
                // Fallback: Allow any origin (less secure, but works for testing)
                // Remove this and configure specific origins for production
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
        }
    });
});

// Configure Cosmos DB
var cosmosConnectionString = builder.Configuration["CosmosDb:ConnectionString"] 
    ?? throw new InvalidOperationException(
        "Cosmos DB connection string is not configured. " +
        "Please set it using: dotnet user-secrets set \"CosmosDb:ConnectionString\" \"AccountEndpoint=...;AccountKey=...\"");

// Register CosmosClient as singleton
builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    return new CosmosClient(cosmosConnectionString);
});

// Register TodoService
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Use CORS before routing
app.UseCors("AllowAngularDev");

app.UseRouting();
app.MapControllers();

// Initialize Cosmos DB database and container on startup
await InitializeCosmosDbAsync(app.Services);

app.Run();

static async Task InitializeCosmosDbAsync(IServiceProvider serviceProvider)
{
    var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    
    const string databaseName = "TodoDb";
    const string containerName = "Items";
    const string partitionKeyPath = "/userId";

    try
    {
        // Create database if it doesn't exist
        var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
        logger.LogInformation("Database '{DatabaseName}' is ready", databaseName);

        // Create container if it doesn't exist
        await database.Database.CreateContainerIfNotExistsAsync(containerName, partitionKeyPath);
        logger.LogInformation("Container '{ContainerName}' with partition key '{PartitionKey}' is ready", 
            containerName, partitionKeyPath);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing Cosmos DB");
        throw;
    }
}
