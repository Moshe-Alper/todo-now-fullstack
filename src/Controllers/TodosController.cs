using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    /// <summary>
    /// REST API controller for Todo operations
    /// All operations are scoped to userId (extracted from header, query, or defaults to "demo")
    /// </summary>
    [Route("todos")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodosController> _logger;

        public TodosController(ITodoService todoService, ILogger<TodosController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        /// <summary>
        /// Extracts userId from request header, query parameter, or defaults to "demo"
        /// </summary>
        private string GetUserId()
        {
            var userId = Request.Headers["X-User-Id"].ToString();
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = Request.Query["userId"].ToString();
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                userId = "demo"; // Default for development/testing
            }
            return userId;
        }

        /// <summary>
        /// GET /todos - Get all todos for the current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var userId = GetUserId();
            var todos = await _todoService.GetTodosAsync(userId);
            return Ok(todos);
        }

        /// <summary>
        /// GET /todos/{id} - Get a specific todo by ID for the current user
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Todo>> GetTodoById(string id)
        {
            var userId = GetUserId();
            var todo = await _todoService.GetTodoByIdAsync(id, userId);
            
            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id '{id}' not found" });
            }
            
            return Ok(todo);
        }

        /// <summary>
        /// POST /todos - Create a new todo for the current user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            
            // Ensure userId is set and generate ID if not provided
            todo.UserId = userId;
            if (string.IsNullOrWhiteSpace(todo.Id))
            {
                todo.Id = Guid.NewGuid().ToString();
            }

            var createdTodo = await _todoService.CreateTodoAsync(todo);
            return CreatedAtAction(
                nameof(GetTodoById), 
                new { id = createdTodo.Id }, 
                createdTodo);
        }

        /// <summary>
        /// PUT /todos/{id} - Update an existing todo for the current user
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodo(string id, [FromBody] Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();

            if (id != todo.Id)
            {
                return BadRequest(new { message = "Todo ID in URL does not match ID in request body" });
            }

            // Ensure userId matches
            todo.UserId = userId;

            // Verify todo exists before updating
            var existingTodo = await _todoService.GetTodoByIdAsync(id, userId);
            if (existingTodo == null)
            {
                return NotFound(new { message = $"Todo with id '{id}' not found" });
            }

            await _todoService.UpdateTodoAsync(id, todo);
            return NoContent();
        }

        /// <summary>
        /// DELETE /todos/{id} - Delete a todo for the current user
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodo(string id)
        {
            var userId = GetUserId();

            // Verify todo exists before deleting
            var todo = await _todoService.GetTodoByIdAsync(id, userId);
            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id '{id}' not found" });
            }

            await _todoService.DeleteTodoAsync(id, userId);
            return NoContent();
        }
    }
}