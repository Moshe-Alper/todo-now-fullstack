using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoApi.Data;

namespace TodoApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Get Cosmos DB connection string from environment variable or User Secrets
            var cosmosConnectionString = Configuration["CosmosDb:ConnectionString"] 
                ?? Configuration.GetConnectionString("CosmosDb")
                ?? Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

            if (string.IsNullOrEmpty(cosmosConnectionString))
            {
                throw new InvalidOperationException(
                    "Cosmos DB connection string is not configured. " +
                    "Please set it in User Secrets, appsettings.json, or environment variable COSMOSDB_CONNECTION_STRING");
            }

            // Register CosmosClient as singleton
            services.AddSingleton<CosmosClient>(serviceProvider =>
            {
                return new CosmosClient(cosmosConnectionString);
            });

            // Register CosmosDbService
            services.AddScoped<ICosmosDbService, CosmosDbService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Cosmos DB database and container
            InitializeCosmosDb(app.ApplicationServices).GetAwaiter().GetResult();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private async Task InitializeCosmosDb(IServiceProvider serviceProvider)
        {
            var cosmosClient = serviceProvider.GetRequiredService<CosmosClient>();
            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync("TodoDb");
            await database.Database.CreateContainerIfNotExistsAsync("Items", "/userId");
        }
    }
}

