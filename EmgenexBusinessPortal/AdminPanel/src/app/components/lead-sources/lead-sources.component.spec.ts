import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LeadSourcesComponent } from './lead-sources.component';

describe('LeadSourcesComponent', () => {
  let component: LeadSourcesComponent;
  let fixture: ComponentFixture<LeadSourcesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LeadSourcesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LeadSourcesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
