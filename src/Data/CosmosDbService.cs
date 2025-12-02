using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using TodoApi.Models;
using System.Net;

namespace TodoApi.Data
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Todo>> GetTodosAsync(string userId);
        Task<Todo> GetTodoByIdAsync(string id, string userId);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task<Todo> UpdateTodoAsync(string id, Todo todo);
        Task DeleteTodoAsync(string id, string userId);
    }

    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;
        private const string DatabaseName = "TodoDb";
        private const string ContainerName = "Items";

        public CosmosDbService(CosmosClient cosmosClient, IConfiguration configuration)
        {
            _container = cosmosClient.GetContainer(DatabaseName, ContainerName);
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync(string userId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
                .WithParameter("@userId", userId);

            var iterator = _container.GetItemQueryIterator<Todo>(query);
            var results = new List<Todo>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<Todo> GetTodoByIdAsync(string id, string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<Todo>(id, new PartitionKey(userId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            var response = await _container.CreateItemAsync(todo, new PartitionKey(todo.UserId));
            return response.Resource;
        }

        public async Task<Todo> UpdateTodoAsync(string id, Todo todo)
        {
            var response = await _container.UpsertItemAsync(todo, new PartitionKey(todo.UserId));
            return response.Resource;
        }

        public async Task DeleteTodoAsync(string id, string userId)
        {
            await _container.DeleteItemAsync<Todo>(id, new PartitionKey(userId));
        }
    }
}

