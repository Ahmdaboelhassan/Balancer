import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeriodJournalsComponent } from './period-journals.component';

describe('PeriodJournalsComponent', () => {
  let component: PeriodJournalsComponent;
  let fixture: ComponentFixture<PeriodJournalsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeriodJournalsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeriodJournalsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
