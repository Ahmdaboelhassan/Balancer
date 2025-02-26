import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsOverviewComponent } from './accounts-overview.component';

describe('AccountsOverviewComponent', () => {
  let component: AccountsOverviewComponent;
  let fixture: ComponentFixture<AccountsOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountsOverviewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountsOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
