import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { AccountsBalance } from '../../Interfaces/Response/AccountsBalance';
import { HomeService } from '../../Services/home.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  imports: [BaseChartDirective, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  homeService = inject(HomeService);
  titleService = inject(Title);
  balances = signal<AccountsBalance[]>([]);
  colors = ['bg-green-500', ' bg-blue-400', 'bg-purple-500', 'bg-red-500'];
  icons = [
    'fa-solid fa-vault',
    'fa-solid fa-building-columns',
    'fa-solid fa-house',
    'fa-solid fa-calendar-days',
  ];
  barChart = signal({});
  pieChart = signal({});
  ngOnInit(): void {
    this.titleService.setTitle('Home');
    this.homeService.GetHome().subscribe({
      next: (result) => {
        this.balances.set(result.accountsSummary);
        this.IntializePieChart(result.currentAndLastMonthExpenses);
        this.IntializeBarChart(
          result.currentYearRevenues,
          result.currentYearExpenses
        );
      },
    });
  }

  IntializeBarChart(revenues: number[], expenses: number[]) {
    if (!revenues && !expenses) return;

    this.barChart.set({
      labels: [
        'Jan',
        'Feb',
        'Mar',
        'Apr',
        'May',
        'Jun',
        'Jul',
        'Aug',
        'Sep',
        'Oct',
        'Nov',
        'Dec',
      ],
      datasets: [
        {
          label: 'Revenues',
          data: revenues,
        },
        {
          label: 'Expenses',
          data: expenses,
        },
      ],
    });
  }

  IntializePieChart(data: number[]) {
    this.pieChart.set({
      labels: ['Last Month Expenses', 'Current Month Expenses'],
      datasets: [
        {
          label: 'Number',
          data: data,
        },
      ],
    });
  }
}
