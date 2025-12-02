import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppHeaderComponent } from './cmps/app-header/app-header.component';
import { TodoListComponent } from './cmps/todo-list/todo-list.component';
import { TodoPreviewComponent } from './cmps/todo-preview/todo-preview.component';

@NgModule({
  declarations: [
    AppComponent,
    AppHeaderComponent,
    TodoListComponent,
    TodoPreviewComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
