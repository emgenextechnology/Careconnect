import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PracticeSpecialityTypesComponent } from './practice-speciality-types.component';

describe('PracticeSpecialityTypesComponent', () => {
  let component: PracticeSpecialityTypesComponent;
  let fixture: ComponentFixture<PracticeSpecialityTypesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PracticeSpecialityTypesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PracticeSpecialityTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
