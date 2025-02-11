import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchPeriodComponent } from './search-period.component';

describe('SearchPeriodComponent', () => {
  let component: SearchPeriodComponent;
  let fixture: ComponentFixture<SearchPeriodComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SearchPeriodComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchPeriodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
