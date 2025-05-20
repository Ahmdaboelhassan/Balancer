import { Component, OnInit, signal } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts';
import { AccountsBalance } from '../../Interfaces/Response/AccountsBalance';
import { NgClass, NgStyle } from '@angular/common';
import { HomeService } from '../../Services/home.service';
import { Title } from '@angular/platform-browser';
import { Home } from '../../Interfaces/Response/Home';
import { BudgetBarDirective } from '../../directive/budget-bar.directive';
import { slideUpAnimation } from '../../Animations/slideUpAnimation';
import { BudgetProgress } from '../../Interfaces/Response/BudgetProgress';

@Component({
  selector: 'app-home',
  imports: [BaseChartDirective, BudgetBarDirective, NgClass, NgStyle],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  animations: [slideUpAnimation],
})
export class HomeComponent implements OnInit {
  home: Home;
  budgetProgresses: BudgetProgress[];

  // Bar Chart
  RevenuesAndExpensesChart = signal({});
  RevenuesAndExpensesChartType = signal<string>('bar');
  switchRevenuesAndExpensesIcon = signal<string>('fa-chart-line');
  // Pie Chart
  pieChart = signal({});
  showPieChat = signal(true);
  switchPieIcon = signal<string>('fa-bars-progress');
  // Line Chart
  lineChart = signal({});
  // Balances
  balances = signal<AccountsBalance[]>([]);
  // Estimated Time
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
        this.budgetProgresses = this.GetBudgetProgress(this.home, false);
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
    const difference = revenues.map((el, i) => {
      if (el && expenses[i]) {
        const dif = el - expenses[i];
        return dif > 0 ? dif : 0;
      }
      return null;
    });

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
          tension: 0.2,
        },
        {
          label: 'Expenses',
          data: expenses,
          tension: 0.2,
        },
        {
          label: 'Profit',
          data: difference,
          tension: 0.2,
          hidden: true,
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
    this.switchRevenuesAndExpensesIcon.update((x) =>
      x == 'fa-chart-line' ? 'fa-chart-column' : 'fa-chart-line'
    );
  }

  TogglePieChart() {
    this.showPieChat.update((x) => !x);
    this.switchPieIcon.update((x) =>
      x === 'fa-bars-progress' ? 'fa-chart-pie' : 'fa-bars-progress'
    );

    if (this.showPieChat()) {
      this.budgetProgresses = this.GetBudgetProgress(this.home, false);
    } else {
      setTimeout(() => {
        this.budgetProgresses = this.GetBudgetProgress(this.home, true);
      }, 50);
    }
  }

  GetBudgetProgress(home: Home, fillAmount: boolean): BudgetProgress[] {
    return home.budgetProgress.map((el) => ({
      budget: el.budget,
      color: el.color,
      displayName: el.displayName,
      spent: fillAmount ? el.spent : 0,
      percentage: fillAmount ? el.percentage : 0,
    }));
  }
}
