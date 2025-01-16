import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-accounts-summary',
  imports: [DateRangeComponent, CurrencyPipe],
  templateUrl: './accounts-summary.component.html',
  styleUrl: './accounts-summary.component.css',
})
export class AccountsSummaryComponent {
  accounts: AccountSummary[] = [];
  constructor(
    private reportService: ReportService,
    private titleServive: Title
  ) {
    this.titleServive.setTitle('Income Statement');
  }

  GetAccountsSummary(dates: any) {
    this.reportService.GetAccountsSummary(dates.from, dates.to).subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
  }
}
