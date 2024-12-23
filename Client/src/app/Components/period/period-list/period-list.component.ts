import { Component, Input } from '@angular/core';
import { PeriodListItemComponent } from './period-list-item/period-list-item.component';
import { PeriodListItem } from '../../../Interfaces/Response/PeriodListItem';

@Component({
  selector: 'app-period-list',
  imports: [PeriodListItemComponent],
  templateUrl: './period-list.component.html',
  styleUrl: './period-list.component.css',
})
export class PeriodListComponent {
  @Input() periods: PeriodListItem[] = [];
}
