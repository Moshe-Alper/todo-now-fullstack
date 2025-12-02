import { Component, OnInit, OnDestroy } from '@angular/core'
import { Subscription } from 'rxjs'
import { TodoService } from '../../services/todo.service'
import { Task } from '../../models/task.model'

@Component({
  selector: 'todo-list',
  standalone: false,
  templateUrl: './todo-list.component.html',
  styleUrl: './todo-list.component.scss'
})
export class TodoListComponent implements OnInit, OnDestroy {
  todos: Task[] = []
  private subscription?: Subscription

  constructor(private todoService: TodoService) {}

  ngOnInit(): void {
    this.todos = this.todoService.getTodos()
    this.subscription = this.todoService.todos$.subscribe(todos => {
      this.todos = todos
    })
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe()
  }
}
