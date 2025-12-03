import { NgModule } from '@angular/core'
import { BrowserModule } from '@angular/platform-browser'
import { HttpClientModule } from '@angular/common/http'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { CommonModule } from '@angular/common'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { AppHeaderComponent } from './cmps/app-header/app-header.component'
import { TodoListComponent } from './cmps/todo-list/todo-list.component'
import { TodoPreviewComponent } from './cmps/todo-preview/todo-preview.component'
import { AddTodoComponent } from './cmps/add-todo/add-todo.component'
import { TodoFilterComponent } from './cmps/todo-filter/todo-filter.component'
import { TodoIndexComponent } from './cmps/todo-index/todo-index.component'

@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent,
    TodoListComponent,
    TodoPreviewComponent,
    AddTodoComponent,
    TodoIndexComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
    TodoFilterComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
