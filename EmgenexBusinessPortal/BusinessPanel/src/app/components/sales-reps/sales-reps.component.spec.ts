import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SalesRepsComponent } from './sales-reps.component';

describe('SalesRepsComponent', () => {
  let component: SalesRepsComponent;
  let fixture: ComponentFixture<SalesRepsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SalesRepsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalesRepsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
