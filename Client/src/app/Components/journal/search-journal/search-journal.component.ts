import { Component, inject, OnInit } from '@angular/core';

import { JournalListComponent } from '../journal-list/journal-list.component';
import { JournalService } from '../../../Services/journal.service';
import { ToastrService } from 'ngx-toastr';
import { Title } from '@angular/platform-browser';
import { JournalAdvancedSearchComponent } from './journal-advanced-search/journal-advanced-search.component';

@Component({
  selector: 'app-search-journal',
  imports: [JournalAdvancedSearchComponent, JournalListComponent],
  templateUrl: './search-journal.component.html',
  styleUrl: './search-journal.component.css',
})
export class SearchJournalComponent implements OnInit {
  journals: any;
  summary: number = 0;
  count: number = 0;

  constructor(
    private titleService: Title,
    private journalService: JournalService,
    private toestr: ToastrService
  ) {
    this.titleService.setTitle('Journal Search');
  }

  ngOnInit(): void {
    this.journalService.advancedSearch$.subscribe({
      next: (keys) => {
        this.AdvancedSearchJournals(keys);
      },
    });
  }

  SearchJournals(key: string) {
    if (key.trim() === '') {
      this.toestr.info('Search key Can not be empty', 'Search Journal');
      return;
    }
    this.journalService.SearchJournals(key).subscribe({
      next: (result) => {
        this.journals = result;
        this.summary = this.journals
          .reduce((acc, journal) => acc + Number(journal.amount), 0)
          .toFixed(2);
        this.count = result.length;
      },
    });
  }

  AdvancedSearchJournals(keys: any) {
    debugger;
    this.journalService.AvancedSearchJournals(keys).subscribe({
      next: (result) => {
        this.journals = result;
        this.summary = this.journals
          .reduce((acc, journal) => acc + Number(journal.amount), 0)
          .toFixed(2);
        this.count = result.length;
      },
    });
  }
}
