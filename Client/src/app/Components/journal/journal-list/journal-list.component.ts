import { Component, Input } from '@angular/core';
import { JournalListItemComponent } from './journal-list-item/journal-list-item.component';
import { JournalListItem } from '../../../Interfaces/Response/JournalListItem';

@Component({
  selector: 'app-journal-list',
  imports: [JournalListItemComponent],
  templateUrl: './journal-list.component.html',
  styleUrl: './journal-list.component.css',
})
export class JournalListComponent {
  @Input() journals: JournalListItem[] | any;
}
