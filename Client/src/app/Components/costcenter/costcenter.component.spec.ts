import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CostcenterComponent } from './costcenter.component';

describe('CostcenterComponent', () => {
  let component: CostcenterComponent;
  let fixture: ComponentFixture<CostcenterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CostcenterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CostcenterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
