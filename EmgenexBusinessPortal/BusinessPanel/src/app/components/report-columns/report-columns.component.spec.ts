import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportColumnsComponent } from './report-columns.component';

describe('ReportColumnsComponent', () => {
  let component: ReportColumnsComponent;
  let fixture: ComponentFixture<ReportColumnsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportColumnsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportColumnsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
