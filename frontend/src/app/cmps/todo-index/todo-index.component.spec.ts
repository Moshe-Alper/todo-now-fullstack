import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoIndexComponent } from './todo-index.component';

describe('TodoIndexComponent', () => {
  let component: TodoIndexComponent;
  let fixture: ComponentFixture<TodoIndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoIndexComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TodoIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
