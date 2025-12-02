import { Injectable } from '@angular/core'
import { BehaviorSubject, Observable } from 'rxjs'
import { dummyTodos } from '../data/dummy-todos'
import { Todo } from '../models/todo.model'

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private todos = dummyTodos
  private todosSubject = new BehaviorSubject<Todo[]>([...this.todos])
  public todos$: Observable<Todo[]> = this.todosSubject.asObservable()
  
  constructor() {
    const todos = localStorage.getItem('todos')
    
    if (todos) {
      this.todos = JSON.parse(todos)
      this.todosSubject.next([...this.todos])
    }
  }

  getTodos(): Todo[] {
    return [...this.todos]
  }

  addTodo(title: string): Todo {
    const newTodo: Todo = {
      id: this.generateId(),
      title: title.trim(),
      isCompleted: false,
      createdAt: Date.now()
    }
    this.todos.unshift(newTodo)
    this.saveTodos()
    return newTodo
  }

  updateTodo(updatedTodo: Todo): void {
    const index = this.todos.findIndex(todo => todo.id === updatedTodo.id)
    if (index !== -1) {
      this.todos[index] = { ...updatedTodo }
      this.saveTodos()
    }
  }

  removeTodo(id: string): void {
    this.todos = this.todos.filter(todo => todo.id !== id)
    this.saveTodos()
  }

  toggleComplete(id: string): void {
    const todo = this.todos.find(t => t.id === id)
    if (todo) {
      todo.isCompleted = !todo.isCompleted
      this.saveTodos()
    }
  }

  private saveTodos(): void {
    localStorage.setItem('todos', JSON.stringify(this.todos))
    this.todosSubject.next([...this.todos])
  }

  private generateId(): string {
    return 't' + Date.now().toString()
  }

}
