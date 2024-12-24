import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountListItemComponent } from './account-list-item.component';

describe('AccountListItemComponent', () => {
  let component: AccountListItemComponent;
  let fixture: ComponentFixture<AccountListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
