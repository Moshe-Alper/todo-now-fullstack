import { Injectable } from '@angular/core';
import { dummyTodos } from '../data/dummy-todos'

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private todos = dummyTodos
  
  constructor() { 
    
    
  }
}
