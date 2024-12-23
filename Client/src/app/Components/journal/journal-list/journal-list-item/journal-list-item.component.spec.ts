import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JournalListItemComponent } from './journal-list-item.component';

describe('JournalListItemComponent', () => {
  let component: JournalListItemComponent;
  let fixture: ComponentFixture<JournalListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JournalListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JournalListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
