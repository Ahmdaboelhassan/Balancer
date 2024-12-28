import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountstatementDetialsComponent } from './accountstatement-detials.component';

describe('AccountstatementDetialsComponent', () => {
  let component: AccountstatementDetialsComponent;
  let fixture: ComponentFixture<AccountstatementDetialsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountstatementDetialsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountstatementDetialsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
