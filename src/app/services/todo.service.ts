import { Injectable } from '@angular/core'
import { BehaviorSubject, Observable } from 'rxjs'
import { dummyTodos } from '../data/dummy-todos'
import { Task } from '../models/task.model'

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private todosSubject = new BehaviorSubject<Task[]>([...dummyTodos])
  public todos$: Observable<Task[]> = this.todosSubject.asObservable()
  
  constructor() { 
    this.loadTodos()
  }

  private loadTodos(): void {
    this.todosSubject.next([...dummyTodos])
  }

  getTodos(): Task[] {
    return [...this.todosSubject.value]
  }

  addTodo(title: string): Task {
    const newTodo: Task = {
      id: this.generateId(),
      title: title.trim(),
      isCompleted: false,
      createdAt: Date.now()
    }
    const todos = this.getTodos()
    todos.push(newTodo)
    this.todosSubject.next(todos)
    return newTodo
  }

  updateTodo(updatedTodo: Task): void {
    const todos = this.getTodos()
    const index = todos.findIndex(todo => todo.id === updatedTodo.id)
    if (index !== -1) {
      todos[index] = { ...updatedTodo }
      this.todosSubject.next(todos)
    }
  }

  deleteTodo(id: string): void {
    const todos = this.getTodos().filter(todo => todo.id !== id)
    this.todosSubject.next(todos)
  }

  toggleComplete(id: string): void {
    const todos = this.getTodos()
    const todo = todos.find(t => t.id === id)
    if (todo) {
      todo.isCompleted = !todo.isCompleted
      this.todosSubject.next(todos)
    }
  }

  private generateId(): string {
    return 't' + Date.now().toString()
  }
}
