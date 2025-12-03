import { Component, Input, OnInit } from '@angular/core'
import { FormBuilder, FormGroup, Validators } from '@angular/forms'
import { TodoService } from '../../services/todo.service'
import { Todo } from '../../models/todo.model'

@Component({
  selector: 'todo-preview',
  standalone: false,
  templateUrl: './todo-preview.component.html',
  styleUrl: './todo-preview.component.scss'
})
export class TodoPreviewComponent implements OnInit {
  @Input() todo!: Todo
  isEditing = false
  editForm: FormGroup

  constructor(
    private todoService: TodoService,
    private fb: FormBuilder
  ) {
    this.editForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  ngOnInit(): void {
    this.editForm.patchValue({ title: this.todo.title })
  }

  toggleComplete(): void {
    this.todoService.toggleComplete(this.todo.id)
  }

  startEdit(): void {
    this.isEditing = true
    this.editForm.patchValue({ title: this.todo.title })
  }

  cancelEdit(): void {
    this.isEditing = false
    this.editForm.patchValue({ title: this.todo.title })
  }

  saveEdit(): void {
    if (this.editForm.valid) {
      const updatedTodo: Todo = {
        ...this.todo,
        title: this.editForm.get('title')?.value.trim()
      }
      this.todoService.updateTodo(updatedTodo)
      this.isEditing = false
    }
  }

  deleteTodo(): void {
    // Delete immediately - no confirmation needed
    this.todoService.removeTodo(this.todo.id)
  }
}
