import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PeriodListItem } from '../../../../Interfaces/Response/PeriodListItem';

@Component({
  selector: 'app-period-list-item',
  imports: [RouterLink],
  templateUrl: './period-list-item.component.html',
  styleUrl: './period-list-item.component.css',
})
export class PeriodListItemComponent {
  @Input() item: PeriodListItem | null = null;
}
