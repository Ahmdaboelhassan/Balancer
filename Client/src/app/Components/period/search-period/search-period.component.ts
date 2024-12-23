import { Component, inject } from '@angular/core';
import { SearchComponent } from '../../search/search.component';
import { PeriodListItem } from '../../../Interfaces/Response/PeriodListItem';
import { PeriodListComponent } from '../period-list/period-list.component';
import { PeriodService } from '../../../Services/period.service';
import { ToastrService } from 'ngx-toastr';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-search-period',
  imports: [SearchComponent, PeriodListComponent],
  templateUrl: './search-period.component.html',
  styleUrl: './search-period.component.css',
})
export class SearchPeriodComponent {
  items: PeriodListItem | any = [];

  constructor(
    private periodService: PeriodService,
    private toester: ToastrService,
    private titleService: Title
  ) {
    this.titleService.setTitle('Search Period');
  }

  SearchPeriod(key: string) {
    if (key.trim() === '') {
      this.toester.info('Search key Can not be empty', 'Search Period');
      return;
    }

    this.periodService.SearchPeriod(key).subscribe({
      next: (result: PeriodListItem | any) => {
        this.items = result;
      },
    });
  }
}
