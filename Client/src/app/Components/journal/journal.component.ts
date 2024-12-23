import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { DateRangeComponent } from '../date-range/date-range.component';
import { JournalListComponent } from './journal-list/journal-list.component';
import { JournalListItem } from '../../Interfaces/Response/JournalListItem';
import { JournalService } from '../../Services/journal.service';

@Component({
  selector: 'app-journal',
  imports: [DateRangeComponent, JournalListComponent],
  templateUrl: './journal.component.html',
  styleUrl: './journal.component.css',
})
export class JournalComponent {
  journals: JournalListItem[] = [];

  constructor(
    private titleService: Title,
    private journalService: JournalService
  ) {
    this.titleService.setTitle('Journals');
  }

  GetJournals(dates: any) {
    if (!dates.from || !dates.to) {
      return;
    }
    this.journalService
      .GetJournals(dates.from, dates.to)
      .subscribe((result) => {
        this.journals = result;
      });
  }
}
