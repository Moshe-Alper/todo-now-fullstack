import { NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { CommonModule } from '@angular/common'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { AppHeaderComponent } from './cmps/app-header/app-header.component'
import { TodoListComponent } from './cmps/todo-list/todo-list.component'
import { TodoPreviewComponent } from './cmps/todo-preview/todo-preview.component'
import { AddTodoComponent } from './cmps/add-todo/add-todo.component'

@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent,
    TodoListComponent,
    TodoPreviewComponent,
    AddTodoComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
