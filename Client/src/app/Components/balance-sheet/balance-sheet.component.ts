import { Component } from '@angular/core';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { DateRangeComponent } from '../date-range/date-range.component';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-balance-sheet',
  imports: [DateRangeComponent, CurrencyPipe, RouterLink],
  templateUrl: './balance-sheet.component.html',
  styleUrl: './balance-sheet.component.css',
})
export class BalanceSheetComponent {
  accounts: AccountSummary[] = [];
  constructor(
    private reportService: ReportService,
    private titleServive: Title,
  ) {
    this.titleServive.setTitle('Balance Sheet');
  }
  from;
  to;

  GetBalanceSheet(dates: any) {
    this.from = dates.from;
    this.to = dates.to;

    this.reportService.GetBalanceSheet(dates.to, dates.maxLevel).subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
  }
}
