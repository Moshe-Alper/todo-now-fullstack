import { Injectable, signal } from '@angular/core'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { BehaviorSubject, Observable, combineLatest } from 'rxjs'
import { map, share } from 'rxjs/operators'
import { toSignal } from '@angular/core/rxjs-interop'
import { Todo, FilterBy } from '../models/todo.model'
import { environment } from '../../environments/environment'

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private apiUrl = `${environment.apiUrl}/todos`
  private todosSubject = new BehaviorSubject<Todo[]>([])
  public todos$: Observable<Todo[]> = this.todosSubject.asObservable()
  
  // Filter sources
  private _filterBy$ = new BehaviorSubject<FilterBy>({ isCompleted: null })
  
  // Signals
  filterBy_ = toSignal(this._filterBy$, { requireSync: true })
  allTodos_ = toSignal(this.todos$, { initialValue: [] })
  
  // Filtered todos observable
  filteredTodos$ = combineLatest([
    this.todos$,
    this._filterBy$
  ]).pipe(
    map(([todos, filterBy]) => {
      if (filterBy.isCompleted === null || filterBy.isCompleted === undefined) {
        return todos
      }
      return todos.filter(todo => todo.isCompleted === filterBy.isCompleted)
    }),
    share()
  )
  
  filteredTodos_ = toSignal(this.filteredTodos$, { initialValue: [] })
  
  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'X-User-Id': 'demo'
    })
  }
  
  constructor(private http: HttpClient) {
    this.loadTodos()
  }

  private loadTodos(): void {
    this.http.get<Todo[]>(this.apiUrl, this.httpOptions).subscribe({
      next: (todos) => {
        this.todosSubject.next(todos)
      },
      error: (error) => {
        console.error('Error loading todos:', error)
        this.todosSubject.next([])
      }
    })
  }

  getTodos(): Todo[] {
    return [...this.todosSubject.value]
  }

  addTodo(title: string): void {
    const tempId = `temp-${Date.now()}`
    const newTodo: Todo = {
      id: tempId,
      title: title.trim(),
      isCompleted: false,
      createdAt: Date.now()
    }
    
    // Optimistically add to UI immediately
    this.todosSubject.next([newTodo, ...this.todosSubject.value])
    
    // Create on backend
    const todoToCreate = { ...newTodo, id: '' }
    this.http.post<Todo>(this.apiUrl, todoToCreate, this.httpOptions).subscribe({
      next: (createdTodo) => {
        // Replace temp todo with server version
        const currentTodos = this.todosSubject.value
        const index = currentTodos.findIndex(t => t.id === tempId)
        if (index !== -1) {
          const updatedTodos = [...currentTodos]
          updatedTodos[index] = createdTodo
          this.todosSubject.next(updatedTodos)
        }
      },
      error: (error) => {
        console.error('Error adding todo:', error)
        // Remove on error
        const currentTodos = this.todosSubject.value.filter(t => t.id !== tempId)
        this.todosSubject.next(currentTodos)
      }
    })
  }

  updateTodo(updatedTodo: Todo): void {
    const currentTodos = this.todosSubject.value
    const index = currentTodos.findIndex(todo => todo.id === updatedTodo.id)
    if (index === -1) return
    
    const originalTodo = { ...currentTodos[index] }
    
    // Optimistically update UI immediately
    const updatedTodos = [...currentTodos]
    updatedTodos[index] = { ...updatedTodo }
    this.todosSubject.next(updatedTodos)
    
    // Update on backend
    this.http.put<Todo>(`${this.apiUrl}/${updatedTodo.id}`, updatedTodo, this.httpOptions).subscribe({
      next: () => {
        // Successfully updated
      },
      error: (error) => {
        console.error('Error updating todo:', error)
        // Rollback on error
        const rollbackTodos = [...this.todosSubject.value]
        rollbackTodos[index] = originalTodo
        this.todosSubject.next(rollbackTodos)
      }
    })
  }

  removeTodo(id: string): void {
    const todoToDelete = this.todosSubject.value.find(todo => todo.id === id)
    if (!todoToDelete) return

    // Optimistically remove from UI immediately
    const currentTodos = this.todosSubject.value.filter(todo => todo.id !== id)
    this.todosSubject.next(currentTodos)

    // Delete from backend
    this.http.delete(`${this.apiUrl}/${id}`, this.httpOptions).subscribe({
      next: () => {
        // Successfully deleted
      },
      error: (error) => {
        console.error('Error removing todo:', error)
        // Restore on error
        this.todosSubject.next([...currentTodos, todoToDelete])
      }
    })
  }

  toggleComplete(id: string): void {
    const todo = this.todosSubject.value.find(t => t.id === id)
    if (todo) {
      const updatedTodo = { ...todo, isCompleted: !todo.isCompleted }
      this.updateTodo(updatedTodo)
    }
  }

  setFilterBy(filterBy: FilterBy): void {
    this._filterBy$.next({ ...filterBy })
  }

  clearCompleted(): void {
    const currentTodos = this.todosSubject.value
    const completedTodos = currentTodos.filter(todo => todo.isCompleted)
    const activeTodos = currentTodos.filter(todo => !todo.isCompleted)
    
    // Optimistically update UI
    this.todosSubject.next(activeTodos)
    
    // Delete all completed todos from backend
    completedTodos.forEach(todo => {
      this.http.delete(`${this.apiUrl}/${todo.id}`, this.httpOptions).subscribe({
        error: (error) => {
          console.error('Error deleting completed todo:', error)
          // Reload todos on error to sync with backend
          this.loadTodos()
        }
      })
    })
  }

  getActiveCount(): number {
    return this.todosSubject.value.filter(todo => !todo.isCompleted).length
  }

  getCompletedCount(): number {
    return this.todosSubject.value.filter(todo => todo.isCompleted).length
  }

}