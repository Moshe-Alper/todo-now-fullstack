using Microsoft.Azure.Cosmos;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

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
