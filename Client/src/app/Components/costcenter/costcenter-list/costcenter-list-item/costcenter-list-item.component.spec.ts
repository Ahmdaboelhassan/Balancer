import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CostcenterListItemComponent } from './costcenter-list-item.component';

describe('CostcenterListItemComponent', () => {
  let component: CostcenterListItemComponent;
  let fixture: ComponentFixture<CostcenterListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CostcenterListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CostcenterListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
