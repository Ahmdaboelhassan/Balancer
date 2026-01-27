import {
  Component,
  HostListener,
  Signal,
  signal,
  ViewChild,
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../Services/account.service';
import { CostcenterService } from '../../Services/costcenter.service';
import { AccountSelectList } from '../../Interfaces/Response/AccountSelectList';
import { CostCenterSelectList } from '../../Interfaces/Response/CostCenterSelectList';
import { NgFor } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { NgSelectComponent } from '@ng-select/ng-select';
import Swal from 'sweetalert2';

@Component({
  imports: [FormsModule, NgFor, NgSelectComponent],
  templateUrl: './accountstatement.component.html',
  styleUrl: './accountstatement.component.css',
})
export class AccountstatementComponent {
  from = signal('');
  to = signal('');
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];
  openingBalance: boolean = false;
  ignoreTo: boolean = false;
  ignoreFrom: boolean = false;
  @ViewChild('myForm') myForm!: NgForm;

  constructor(
    private accountService: AccountService,
    private costCenterService: CostcenterService,
    private titleServive: Title,
  ) {
    this.GetDefaultDate();
    this.accountService.GetAllAccountSelectList().subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
    this.costCenterService
      .GetAllCostCenterSelectList()
      .subscribe({ next: (costCenters) => (this.costCenters = costCenters) });
    this.titleServive.setTitle('Account Statement');
  }

  GetAccountStatement(form: NgForm) {
    const baseUrl = window.location.origin;
    if (!form) Swal.fire('', 'Invalid Form', 'info');

    const formValue = form.value;
    if (!formValue.account && !formValue.costCenter) {
      Swal.fire('', 'Please Select Account Or Cost Center', 'info');
    } else {
      const params = {
        openingBalance: formValue.openingBalance,
      };

      if (formValue.account) {
        params['account'] = formValue.account;
      }

      if (formValue.costCenter) {
        params['costCenter'] = formValue.costCenter;
      }

      if (formValue.from && !formValue.ignoreDates) {
        params['from'] = formValue.from;
      }
      if (formValue.to && !formValue.ignoreDates) {
        params['to'] = formValue.to;
      }
      const queryParams = new URLSearchParams(params).toString();
      const hashRoute = `#/AccountStatement/Get?${queryParams}`;

      const fullUrl = `${baseUrl}/${hashRoute}`;
      window.open(fullUrl, '_blank');
    }
  }
  DecrementMonth() {
    let f = new Date(this.from());
    f.setMonth(f.getMonth() - 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }
  IncrementMonth() {
    let f = new Date(this.from());
    f.setMonth(f.getMonth() + 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetCurrentDay() {
    const currentDate = new Date();
    this.from.set(this.formatLocalDate(currentDate));
    this.to.set(this.formatLocalDate(currentDate));
  }

  GetDefaultWeekDates() {
    const currentDate = new Date();
    const day = currentDate.getDay();

    const diffToSaturday = day === 6 ? 0 : day + 1;

    const firstDayOfWeek = new Date(currentDate);
    firstDayOfWeek.setDate(currentDate.getDate() - diffToSaturday);

    const lastDayOfWeek = new Date(firstDayOfWeek);
    lastDayOfWeek.setDate(firstDayOfWeek.getDate() + 6);

    this.from.set(this.formatLocalDate(firstDayOfWeek));
    this.to.set(this.formatLocalDate(lastDayOfWeek));
  }

  private formatLocalDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
  GetDefaultDate() {
    const currentDate = new Date();
    const firstDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth(),
      2,
    );
    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1,
    );

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetDefaultYear() {
    const currentDate = new Date();

    const firstDay = new Date(currentDate.getFullYear(), 0, 2); // Jan 1
    const lastDay = new Date(currentDate.getFullYear(), 12, 1); // Dec 31

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.ctrlKey && event.key === 'Enter') {
      event.preventDefault();
      this.GetAccountStatement(this.myForm);
    }
  }
}
