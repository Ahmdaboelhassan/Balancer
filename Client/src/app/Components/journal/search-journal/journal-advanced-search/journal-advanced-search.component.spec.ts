import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JournalAdvancedSearchComponent } from './journal-advanced-search.component';

describe('JournalAdvancedSearchComponent', () => {
  let component: JournalAdvancedSearchComponent;
  let fixture: ComponentFixture<JournalAdvancedSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JournalAdvancedSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JournalAdvancedSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
