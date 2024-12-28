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
  // Number Can Change In Time
  periodDays = 7;
  dayRate = 90;

  timeLeft: { periods: number; days: number } = {
    periods: 0,
    days: 0,
  };
  moneyNeed: string = '0';
  homeService = inject(HomeService);
  titleService = inject(Title);
  balances = signal<AccountsBalance[]>([]);
  colors = [
    'bg-green-500',
    ' bg-blue-400',
    'bg-purple-500',
    'bg-red-500',
    'bg-yellow-500',
    'bg-orange-500',
  ];
  icons = [
    'fa-solid fa-vault',
    'fa-solid fa-building-columns',
    'fa-solid fa-house',
    'fa-solid fa-calendar-days',
    'fa-solid fa-hourglass-start',
    'fa-solid fa-wallet',
  ];
  barChart = signal({});
  pieChart = signal({});
  ngOnInit(): void {
    this.GetEstimatedMonthNeed();
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
      labels: ['Last Month', 'Current Month'],
      datasets: [
        {
          label: 'Number',
          data: data,
        },
      ],
    });
  }

  GetEstimatedMonthNeed() {
    const currentDate = new Date();

    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1
    );

    const timeDifference = lastDay.getTime() - currentDate.getTime();
    const days = Math.ceil(timeDifference / 1000 / 60 / 60 / 24);
    const periods = Math.floor(days / this.periodDays);
    const money = days * this.dayRate;

    this.moneyNeed = new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(money);

    this.timeLeft = { periods: periods, days: days };
  }
}
