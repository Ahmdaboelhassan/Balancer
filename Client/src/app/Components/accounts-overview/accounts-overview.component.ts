import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { CurrencyPipe } from '@angular/common';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';

@Component({
  selector: 'app-accounts-overview',
  imports: [DateRangeComponent, CurrencyPipe],
  templateUrl: './accounts-overview.component.html',
  styleUrl: './accounts-overview.component.css',
})
export class AccountsOverviewComponent {
  accounts: AccountSummary[] = [];
  constructor(
    private reportService: ReportService,
    private titleServive: Title
  ) {
    this.titleServive.setTitle('Income Statement');
  }

  GetAccountsOverview(dates: any) {
    this.reportService.GetAccountsOverview(dates.from, dates.to).subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
  }
}
