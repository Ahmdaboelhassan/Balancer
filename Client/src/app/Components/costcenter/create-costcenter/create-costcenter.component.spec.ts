import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCostcenterComponent } from './create-costcenter.component';

describe('CreateCostcenterComponent', () => {
  let component: CreateCostcenterComponent;
  let fixture: ComponentFixture<CreateCostcenterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCostcenterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCostcenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
