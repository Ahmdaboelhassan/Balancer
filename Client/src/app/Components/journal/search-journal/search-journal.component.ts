import { Component, inject } from '@angular/core';
import { SearchComponent } from '../../search/search.component';
import { JournalListComponent } from '../journal-list/journal-list.component';
import { JournalService } from '../../../Services/journal.service';
import { ToastrService } from 'ngx-toastr';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-search-journal',
  imports: [SearchComponent, JournalListComponent],
  templateUrl: './search-journal.component.html',
  styleUrl: './search-journal.component.css',
})
export class SearchJournalComponent {
  journals: any;
  journalService = inject(JournalService);
  toestr = inject(ToastrService);

  constructor(private titleService: Title) {
    titleService.setTitle('Journal Search');
  }

  SearchJournals(key: string) {
    if (key.trim() === '') {
      this.toestr.info('Search key Can not be empty', 'Search Journal');
      return;
    }
    this.journalService.SearchJournals(key).subscribe({
      next: (result) => {
        this.journals = result;
      },
    });
  }
}
