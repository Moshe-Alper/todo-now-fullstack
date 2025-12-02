using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Data;

namespace TodoApi.Controllers
{
    [Route("todos")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public TodosController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        private string GetUserId()
        {
            // Try to get userId from header first, then query string, then default to "demo"
            var userId = Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrEmpty(userId))
            {
                userId = Request.Query["userId"].ToString();
            }
            if (string.IsNullOrEmpty(userId))
            {
                userId = "demo"; // Hardcoded default as per requirements
            }
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var userId = GetUserId();
            var todos = await _cosmosDbService.GetTodosAsync(userId);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodoById(string id)
        {
            var userId = GetUserId();
            var todo = await _cosmosDbService.GetTodoByIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
        {
            var userId = GetUserId();
            
            // Ensure userId is set and Id is generated if not provided
            todo.UserId = userId;
            if (string.IsNullOrEmpty(todo.Id))
            {
                todo.Id = Guid.NewGuid().ToString();
            }

            var createdTodo = await _cosmosDbService.CreateTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodoById), new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(string id, Todo todo)
        {
            var userId = GetUserId();

            if (id != todo.Id)
            {
                return BadRequest("Todo ID mismatch");
            }

            // Ensure userId matches
            todo.UserId = userId;

            // Check if todo exists
            var existingTodo = await _cosmosDbService.GetTodoByIdAsync(id, userId);
            if (existingTodo == null)
            {
                return NotFound();
            }

            await _cosmosDbService.UpdateTodoAsync(id, todo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(string id)
        {
            var userId = GetUserId();

            // Check if todo exists
            var todo = await _cosmosDbService.GetTodoByIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound();
            }

            await _cosmosDbService.DeleteTodoAsync(id, userId);
            return NoContent();
        }
    }
}