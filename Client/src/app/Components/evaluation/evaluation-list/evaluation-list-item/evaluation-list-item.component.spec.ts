import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluationListItemComponent } from './evaluation-list-item.component';

describe('EvaluationListItemComponent', () => {
  let component: EvaluationListItemComponent;
  let fixture: ComponentFixture<EvaluationListItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EvaluationListItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EvaluationListItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
