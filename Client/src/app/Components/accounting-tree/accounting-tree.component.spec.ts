import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountingTreeComponent } from './accounting-tree.component';

describe('AccountingTreeComponent', () => {
  let component: AccountingTreeComponent;
  let fixture: ComponentFixture<AccountingTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountingTreeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountingTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
