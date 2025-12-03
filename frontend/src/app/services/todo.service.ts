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
    const newTodo: Todo = {
      id: '',
      title: title.trim(),
      isCompleted: false,
      createdAt: Date.now()
    }
    
    this.http.post<Todo>(this.apiUrl, newTodo, this.httpOptions).subscribe({
      next: (createdTodo) => {
        const currentTodos = this.todosSubject.value
        this.todosSubject.next([createdTodo, ...currentTodos])
      },
      error: (error) => {
        console.error('Error adding todo:', error)
      }
    })
  }

  updateTodo(updatedTodo: Todo): void {
    this.http.put<Todo>(`${this.apiUrl}/${updatedTodo.id}`, updatedTodo, this.httpOptions).subscribe({
      next: () => {
        const currentTodos = this.todosSubject.value
        const index = currentTodos.findIndex(todo => todo.id === updatedTodo.id)
        if (index !== -1) {
          const updatedTodos = [...currentTodos]
          updatedTodos[index] = { ...updatedTodo }
          this.todosSubject.next(updatedTodos)
        }
      },
      error: (error) => {
        console.error('Error updating todo:', error)
      }
    })
  }

  removeTodo(id: string): void {
    this.http.delete(`${this.apiUrl}/${id}`, this.httpOptions).subscribe({
      next: () => {
        const currentTodos = this.todosSubject.value.filter(todo => todo.id !== id)
        this.todosSubject.next(currentTodos)
      },
      error: (error) => {
        console.error('Error removing todo:', error)
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

}
