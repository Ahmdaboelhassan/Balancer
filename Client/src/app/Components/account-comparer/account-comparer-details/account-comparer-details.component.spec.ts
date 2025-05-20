import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountComparerDetailsComponent } from './account-comparer-details.component';

describe('AccountComparerDetailsComponent', () => {
  let component: AccountComparerDetailsComponent;
  let fixture: ComponentFixture<AccountComparerDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountComparerDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountComparerDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
