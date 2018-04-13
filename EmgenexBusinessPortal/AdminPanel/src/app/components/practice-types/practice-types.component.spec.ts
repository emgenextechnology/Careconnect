import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeTypesComponent } from './practice-types.component';

describe('PracticeTypesComponent', () => {
  let component: PracticeTypesComponent;
  let fixture: ComponentFixture<PracticeTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PracticeTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PracticeTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
