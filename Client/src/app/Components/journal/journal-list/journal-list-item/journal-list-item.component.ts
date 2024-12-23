import { Component, Input } from '@angular/core';
import { JournalListItem } from '../../../../Interfaces/Response/JournalListItem';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-journal-list-item',
  imports: [RouterLink],
  templateUrl: './journal-list-item.component.html',
  styleUrl: './journal-list-item.component.css',
})
export class JournalListItemComponent {
  @Input() journal: JournalListItem | any;
}
