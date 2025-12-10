import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JournalSearchModalComponent } from './journal-search-modal.component';

describe('JournalSearchModalComponent', () => {
  let component: JournalSearchModalComponent;
  let fixture: ComponentFixture<JournalSearchModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JournalSearchModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JournalSearchModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
