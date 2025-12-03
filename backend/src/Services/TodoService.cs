using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using TodoApi.Models;

namespace TodoApi.Services
{
    /// <summary>
    /// Repository service for Todo operations using Azure Cosmos DB
    /// All operations are scoped to userId using partition key
    /// </summary>
    public class TodoService : ITodoService
    {
        private readonly Container _container;
        private readonly ILogger<TodoService> _logger;
        private const string DatabaseName = "TodoDb";
        private const string ContainerName = "Items";

        public TodoService(CosmosClient cosmosClient, ILogger<TodoService> logger)
        {
            _container = cosmosClient.GetContainer(DatabaseName, ContainerName);
            _logger = logger;
        }

        public async Task<IEnumerable<Todo>> GetTodosAsync(string userId)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId ORDER BY c._ts DESC")
                    .WithParameter("@userId", userId);

                var iterator = _container.GetItemQueryIterator<Todo>(query);
                var results = new List<Todo>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }

                _logger.LogInformation("Retrieved {Count} todos for user {UserId}", results.Count, userId);
                return results;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Error retrieving todos for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Todo?> GetTodoByIdAsync(string id, string userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<Todo>(
                    id, 
                    new PartitionKey(userId));
                
                _logger.LogInformation("Retrieved todo {TodoId} for user {UserId}", id, userId);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Todo {TodoId} not found for user {UserId}", id, userId);
                return null;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Error retrieving todo {TodoId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            try
            {
                if (string.IsNullOrEmpty(todo.UserId))
                {
                    throw new ArgumentException("UserId is required", nameof(todo));
                }

                var response = await _container.CreateItemAsync(
                    todo, 
                    new PartitionKey(todo.UserId));
                
                _logger.LogInformation("Created todo {TodoId} for user {UserId}", response.Resource.Id, todo.UserId);
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Error creating todo for user {UserId}", todo.UserId);
                throw;
            }
        }

        public async Task<Todo> UpdateTodoAsync(string id, Todo todo)
        {
            try
            {
                if (string.IsNullOrEmpty(todo.UserId))
                {
                    throw new ArgumentException("UserId is required", nameof(todo));
                }

                if (id != todo.Id)
                {
                    throw new ArgumentException("Todo ID mismatch", nameof(id));
                }

                var response = await _container.UpsertItemAsync(
                    todo, 
                    new PartitionKey(todo.UserId));
                
                _logger.LogInformation("Updated todo {TodoId} for user {UserId}", id, todo.UserId);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Todo {TodoId} not found for user {UserId} during update", id, todo.UserId);
                throw;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Error updating todo {TodoId} for user {UserId}", id, todo.UserId);
                throw;
            }
        }

        public async Task DeleteTodoAsync(string id, string userId)
        {
            try
            {
                await _container.DeleteItemAsync<Todo>(
                    id, 
                    new PartitionKey(userId));
                
                _logger.LogInformation("Deleted todo {TodoId} for user {UserId}", id, userId);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Todo {TodoId} not found for user {UserId} during delete", id, userId);
                throw;
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex, "Error deleting todo {TodoId} for user {UserId}", id, userId);
                throw;
            }
        }
    }
}

