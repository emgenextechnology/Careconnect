import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserPrivilegesComponent } from './user-privileges.component';

describe('UserPrivilegesComponent', () => {
  let component: UserPrivilegesComponent;
  let fixture: ComponentFixture<UserPrivilegesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserPrivilegesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserPrivilegesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
