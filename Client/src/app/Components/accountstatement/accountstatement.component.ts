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
    private titleServive: Title
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

  GetDefaultDate() {
    const currentDate = new Date();
    const firstDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth(),
      2
    );
    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1
    );

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
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

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.ctrlKey && event.key === 'Enter') {
      event.preventDefault();
      this.GetAccountStatement(this.myForm);
    }
  }
}
