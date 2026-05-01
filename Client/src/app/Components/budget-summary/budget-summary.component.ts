import { Component } from '@angular/core';
import { DateRangeComponent } from '../date-range/date-range.component';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import {
  BudgetSummaryDTO,
  BudgetSummaryReportDTO,
} from '../../Interfaces/Response/BudgetSummaryReportDTO';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-budget-summary',
  imports: [DateRangeComponent, CurrencyPipe, DecimalPipe, RouterLink],
  templateUrl: './budget-summary.component.html',
  styleUrl: './budget-summary.component.css',
})
export class BudgetSummaryComponent {
  periods: BudgetSummaryDTO[] = [];
  accounts: BudgetSummaryDTO[] = [];
  savings: BudgetSummaryDTO[] = [];
  to: string;
  from: string;

  constructor(
    private reportService: ReportService,
    private titleService: Title,
  ) {
    this.titleService.setTitle('Budget Summary');
  }

  GetBudgetSummary(dates: any) {
    this.to = dates.to;
    this.from = dates.from;

    this.reportService.GetBudgetSummary(dates.from, dates.to).subscribe({
      next: (result) => {
        this.periods = result.periods ?? [];
        this.accounts = result.accounts ?? [];
        this.savings = result.savings ?? [];
      },
    });
  }
}
