import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationTypesComponent } from './notification-types.component';

describe('NotificationTypesComponent', () => {
  let component: NotificationTypesComponent;
  let fixture: ComponentFixture<NotificationTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotificationTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
