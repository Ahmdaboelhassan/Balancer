import { Component, signal } from '@angular/core';
import { ReportService } from '../../../Services/report.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountComparer } from '../../../Interfaces/Response/AccountComparer';
import { BaseChartDirective } from 'ng2-charts';
import { slideUpAnimation } from '../../../Animations/slideUpAnimation';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-account-comparer-details',
  imports: [BaseChartDirective, NgClass],
  animations: [slideUpAnimation],
  templateUrl: './account-comparer-details.component.html',
  styleUrl: './account-comparer-details.component.css',
})
export class AccountComparerDetailsComponent {
  accountComparer: AccountComparer;
  chart = signal({});
  chartType = signal('bar');
  chartTypeIcon = signal<string>('fa-chart-line');
  constructor(
    private reportService: ReportService,
    private route: ActivatedRoute,
    private toestr: ToastrService
  ) {}

  ngOnInit(): void {
    const from = this.route.snapshot.queryParams['from'] || '';
    const to = this.route.snapshot.queryParams['to'] || '';
    const account = this.route.snapshot.queryParams['account'];
    const costcenter = this.route.snapshot.queryParams['costCenter'] || '';
    const groupType = this.route.snapshot.queryParams['groupType'];

    if (!account) {
      this.toestr.error('Please Select Account');
    }

    this.reportService
      .GetAccountComparer(from, to, account, costcenter, groupType)
      .subscribe({
        next: (accountComparer) => {
          this.accountComparer = accountComparer;
          console.log(accountComparer);
          this.InitChart(accountComparer);
        },
      });
  }

  InitChart(accountComparer: AccountComparer) {
    this.chart.set({
      labels: accountComparer.time, // x-axis labels
      datasets: [
        {
          label: accountComparer.label,
          data: accountComparer.amounts,
          borderColor: 'rgb(75, 192, 192)',
          backgroundColor: 'rgba(75, 192, 192, 0.7)',
          tension: 0.5,
          pointStyle: 'circle',
          pointRadius: 10,
          pointHoverRadius: 20,
        },
      ],
    });
  }

  SwitchChartType() {
    this.chartType.update((x) => (x == 'bar' ? 'line' : 'bar'));
    this.chartTypeIcon.update((x) =>
      x == 'fa-chart-line' ? 'fa-chart-column' : 'fa-chart-line'
    );
  }
}
