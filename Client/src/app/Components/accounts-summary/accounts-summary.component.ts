import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-accounts-summary',
  imports: [DateRangeComponent, CurrencyPipe, RouterLink],
  templateUrl: './accounts-summary.component.html',
  styleUrl: './accounts-summary.component.css',
})
export class AccountsSummaryComponent {
  accounts: AccountSummary[] = [];
  constructor(
    private reportService: ReportService,
    private titleServive: Title,
  ) {
    this.titleServive.setTitle('Accounts Summary');
  }

  to;
  from;

  GetAccountsSummary(dates: any) {
    this.to = dates.to;
    this.from = dates.from;

    this.reportService.GetAccountsSummary(dates.from, dates.to).subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
  }
}
