import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountComparerComponent } from './account-comparer.component';

describe('AccountComparerComponent', () => {
  let component: AccountComparerComponent;
  let fixture: ComponentFixture<AccountComparerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountComparerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountComparerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
