import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeriodListItemComponent } from './period-list-item.component';

describe('PeriodListItemComponent', () => {
  let component: PeriodListItemComponent;
  let fixture: ComponentFixture<PeriodListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PeriodListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PeriodListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
