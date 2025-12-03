import { Component, inject, Signal } from '@angular/core';
import { TodoService } from '../../services/todo.service';
import { FilterBy } from '../../models/todo.model';

@Component({
  selector: 'todo-filter',
  imports: [],
  templateUrl: './todo-filter.component.html',
  styleUrl: './todo-filter.component.scss'
})
export class TodoFilterComponent {
  private todoService = inject(TodoService)
  filterBy_: Signal<FilterBy> = this.todoService.filterBy_

  setFilter(filterType: 'all' | 'active' | 'completed'): void {
    const filterBy: FilterBy = 
      filterType === 'all' 
        ? { isCompleted: null }
        : { isCompleted: filterType === 'completed' }
    this.todoService.setFilterBy(filterBy)
  }

  isActive(filterType: 'all' | 'active' | 'completed'): boolean {
    const currentFilter = this.filterBy_()
    if (filterType === 'all') {
      return currentFilter.isCompleted === null || currentFilter.isCompleted === undefined
    }
    return currentFilter.isCompleted === (filterType === 'completed')
  }
}
