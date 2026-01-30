import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-income-statement',
  imports: [DateRangeComponent, CurrencyPipe, RouterLink],
  templateUrl: './income-statement.component.html',
  styleUrl: './income-statement.component.css',
})
export class IncomeStatementComponent {
  accounts: AccountSummary[] = [];
  to;
  from;

  constructor(
    private reportService: ReportService,
    private titleServive: Title,
  ) {
    this.titleServive.setTitle('Income Statement');
  }

  GetIncomeStatement(dates: any) {
    this.to = dates.to;
    this.from = dates.from;

    this.reportService.GetIncomeStatement(dates.from, dates.to).subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
  }
}
