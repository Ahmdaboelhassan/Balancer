import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountBudgetSettingsComponent } from './account-budget-settings.component';

describe('AccountBudgetSettingsComponent', () => {
  let component: AccountBudgetSettingsComponent;
  let fixture: ComponentFixture<AccountBudgetSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountBudgetSettingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountBudgetSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
