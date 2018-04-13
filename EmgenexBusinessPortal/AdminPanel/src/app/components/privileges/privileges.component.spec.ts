import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivilegesComponent } from './privileges.component';

describe('PrivilegesComponent', () => {
  let component: PrivilegesComponent;
  let fixture: ComponentFixture<PrivilegesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrivilegesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrivilegesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
