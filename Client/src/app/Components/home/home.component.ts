import {
  AfterViewInit,
  Component,
  inject,
  OnInit,
  signal,
  WritableSignal,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { AccountsBalance } from '../../Interfaces/Response/AccountsBalance';
import { HomeService } from '../../Services/home.service';
import { Title } from '@angular/platform-browser';
import { Home } from '../../Interfaces/Response/Home';
import { BudgetBarDirective } from '../../directive/budget-bar.directive';

@Component({
  selector: 'app-home',
  imports: [BaseChartDirective, BudgetBarDirective],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  home: Home;
  RevenuesAndExpensesChartType = signal<string>('bar');
  RevenuesAndExpensesChart = signal({});
  showPieChat = signal(true);
  pieChart = signal({});
  lineChart = signal({});
  balances = signal<AccountsBalance[]>([]);

  timeLeft: { periods: number; days: number } = {
    periods: 0,
    days: 0,
  };

  moneyNeed: string = '0';

  colors = [
    'bg-green-500',
    'bg-blue-400',
    'bg-purple-500',
    'bg-red-500',
    'bg-yellow-500',
    'bg-orange-500',
    'bg-rose-500',
  ];
  icons = [
    'fa-solid fa-vault',
    'fa-solid fa-building-columns',
    'fa-solid fa-wallet',
    'fa-solid fa-calendar-days',
    'fa-solid fa-hourglass-start',
    'fa-solid fa-sack-dollar',
    'fa-solid fa-house',
  ];

  constructor(private homeService: HomeService, private titleService: Title) {}
  ngOnInit(): void {
    this.titleService.setTitle('Balancer');
    this.homeService.GetHome().subscribe({
      next: (result) => {
        this.home = result;
        this.balances.set(result.accountsSummary);
        this.InitializeLineChart(result.lastPeriods);
        this.IntializePieChart(result.currentAndLastMonthExpenses);
        this.IntializeRevenuesAndExpensesChart(
          result.currentYearRevenues,
          result.currentYearExpenses
        );
        this.GetEstimatedMonthNeed(result.periodDays, result.dayRate);
      },
    });
  }

  IntializeRevenuesAndExpensesChart(revenues: number[], expenses: number[]) {
    revenues = revenues ? revenues.map((x) => (x === 0 ? null : x)) : [];
    expenses = expenses ? expenses.map((x) => (x === 0 ? null : x)) : [];

    this.RevenuesAndExpensesChart.set({
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
          tension: 0.3,
        },
        {
          label: 'Expenses',
          data: expenses,
          tension: 0.3,
        },
      ],
    });
  }

  IntializePieChart(data: number[]) {
    this.pieChart.set({
      labels: ['Current', 'Last'],
      datasets: [
        {
          label: 'Amount',
          data: data,
        },
      ],
    });
  }
  InitializeLineChart(data: number[]) {
    this.lineChart.set({
      labels: ['', '', '', 'Current'], // x-axis labels
      datasets: [
        {
          label: 'Last Periods',
          data: data,
          fill: true,
          borderColor: 'rgb(75, 192, 192)',
          backgroundColor: 'rgba(75, 192, 192, 0.15)',
          tension: 0.5,
          pointStyle: 'circle',
          pointRadius: 5,
          pointHoverRadius: 10,
        },
      ],
    });
  }

  GetEstimatedMonthNeed(periodDays, dayRate) {
    const currentDate = new Date();

    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1
    );

    const timeDifference = lastDay.getTime() - currentDate.getTime();
    const days = Math.ceil(timeDifference / 1000 / 60 / 60 / 24);
    const periods = Math.floor(days / periodDays);
    const money = days * dayRate;

    this.moneyNeed = new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(money);

    this.timeLeft = { periods: periods, days: days };
  }

  ToggleBalance($event) {
    const el = $event.currentTarget as HTMLElement;
    const cover = el.querySelector('.cover');
    if (cover) {
      cover.classList.toggle('opacity-0');
    }
  }

  ScrollHorizontally(event, scrollContainer: HTMLElement) {
    event.preventDefault();
    scrollContainer.scrollLeft += event.deltaY;
  }

  SwitchRevenueAndExpensesChartType() {
    this.RevenuesAndExpensesChartType.update((x) =>
      x == 'bar' ? 'line' : 'bar'
    );
  }

  TogglePieChart() {
    this.showPieChat.update((x) => !x);
  }
}
