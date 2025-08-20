import { Component } from '@angular/core';
import { PeriodListComponent } from './period-list/period-list.component';
import { DateRangeComponent } from '../date-range/date-range.component';
import { PeriodService } from '../../Services/period.service';
import { PeriodListItem } from '../../Interfaces/Response/PeriodListItem';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-period',
  imports: [PeriodListComponent, DateRangeComponent],
  templateUrl: './period.component.html',
  styleUrl: './period.component.css',
})
export class PeriodComponent {
  items: PeriodListItem[] = [];

  constructor(
    private periodService: PeriodService,
    private titleService: Title
  ) {
    this.titleService.setTitle('Periods');
  }

  GetPeriodsByDate(date: object | any) {
    this.periodService.GetPeriod(date.from, date.to).subscribe({
      next: (result: PeriodListItem[] | any) => {
        this.items = result;
      },
    });
  }
}
