import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ColumnLookupsComponent } from './column-lookups.component';

describe('ColumnLookupsComponent', () => {
  let component: ColumnLookupsComponent;
  let fixture: ComponentFixture<ColumnLookupsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ColumnLookupsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ColumnLookupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
