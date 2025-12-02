import { Component } from '@angular/core'
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { TodoService } from '../../services/todo.service'

@Component({
  selector: 'add-todo',
  standalone: false,
  templateUrl: './add-todo.component.html',
  styleUrl: './add-todo.component.scss'
})
export class AddTodoComponent {
  todoForm: FormGroup

  constructor(
    private fb: FormBuilder,
    private todoService: TodoService
  ) {
    this.todoForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  onSubmit(): void {
    if (this.todoForm.valid) {
      const title = this.todoForm.get('title')?.value
      this.todoService.addTodo(title)
      this.todoForm.reset()
    }
  }
}
