import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatePeriodComponent } from './create-period.component';

describe('CreatePeriodComponent', () => {
  let component: CreatePeriodComponent;
  let fixture: ComponentFixture<CreatePeriodComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreatePeriodComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreatePeriodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
