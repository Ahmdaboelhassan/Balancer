import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsSummaryComponent } from './accounts-summary.component';

describe('AccountsSummaryComponent', () => {
  let component: AccountsSummaryComponent;
  let fixture: ComponentFixture<AccountsSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountsSummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountsSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
