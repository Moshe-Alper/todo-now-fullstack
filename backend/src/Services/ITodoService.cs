using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Services
{
    /// <summary>
    /// Service interface for Todo operations with Cosmos DB
    /// </summary>
    public interface ITodoService
    {
        Task<IEnumerable<Todo>> GetTodosAsync(string userId);
        Task<Todo?> GetTodoByIdAsync(string id, string userId);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task<Todo> UpdateTodoAsync(string id, Todo todo);
        Task DeleteTodoAsync(string id, string userId);
    }
}

