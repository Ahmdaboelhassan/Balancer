import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { CurrencyPipe } from '@angular/common';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-accounts-overview',
  imports: [CurrencyPipe, DateRangeComponent, RouterLink],
  templateUrl: './accounts-overview.component.html',
  styleUrl: './accounts-overview.component.css',
})
export class AccountsOverviewComponent {
  accounts: AccountSummary[] = [];
  to;
  from;

  constructor(
    private reportService: ReportService,
    private titleServive: Title
  ) {
    this.titleServive.setTitle('Accounts Overview');
  }

  GetAccountsOverview(filter: any) {
    this.to = filter.to;
    this.from = filter.from;

    this.reportService
      .GetAccountsOverview(filter.from, filter.to, filter.maxLevel)
      .subscribe({
        next: (accounts) => (this.accounts = accounts),
      });
  }
}
