import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivilegeModulesComponent } from './privilege-modules.component';

describe('PrivilegeModulesComponent', () => {
  let component: PrivilegeModulesComponent;
  let fixture: ComponentFixture<PrivilegeModulesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrivilegeModulesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrivilegeModulesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
