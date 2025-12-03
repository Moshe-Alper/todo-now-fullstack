import { Component, inject, Signal, computed } from '@angular/core'
import { TodoService } from '../../services/todo.service'
import { Todo } from '../../models/todo.model'

@Component({
  selector: 'todo-list',
  standalone: false,
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.scss'
})
export class TodoListComponent {
  private todoService = inject(TodoService)
  todos: Signal<Todo[]> = this.todoService.filteredTodos_
  filterBy: Signal<any> = this.todoService.filterBy_
  allTodos: Signal<Todo[]> = this.todoService.allTodos_
  
  // Computed message based on filter state
  emptyMessage = computed(() => {
    const filtered = this.todos()
    const all = this.allTodos()
    const filter = this.filterBy()
    
    // If there are no todos at all
    if (all.length === 0) {
      return "No todos yet. Add one above!"
    }
    
    // If filtering for active todos and none exist
    if (filter.isCompleted === false) {
      return "No active todos. All todos are completed!"
    }
    
    // If filtering for completed todos and none exist
    if (filter.isCompleted === true) {
      return "No completed todos yet."
    }
    
    // Default message (shouldn't reach here if filtering "all" and there are todos)
    return "No todos yet. Add one above!"
  })
}
