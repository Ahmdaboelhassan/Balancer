import { Component, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { AccountSelectList } from '../../Interfaces/Response/AccountSelectList';
import { CostCenterSelectList } from '../../Interfaces/Response/CostCenterSelectList';
import { AccountService } from '../../Services/account.service';
import { CostcenterService } from '../../Services/costcenter.service';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-account-comparer',
  imports: [FormsModule, NgFor],
  templateUrl: './account-comparer.component.html',
  styleUrl: './account-comparer.component.css',
})
export class AccountComparerComponent {
  from = signal('');
  to = signal('');
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];

  constructor(
    private router: Router,
    private accountService: AccountService,
    private costCenterService: CostcenterService,
    private titleServive: Title
  ) {
    this.GetDefaultDate();
    this.accountService
      .GetAllAccountSelectList()
      .subscribe({ next: (accounts) => (this.accounts = accounts) });
    this.costCenterService
      .GetAllCostCenterSelectList()
      .subscribe({ next: (costCenters) => (this.costCenters = costCenters) });
    this.titleServive.setTitle(' Account Comparer');
  }

  GetDefaultDate() {
    const currentDate = new Date();

    const firstDay = new Date(currentDate.getFullYear(), 0, 2);
    const lastDay = new Date(currentDate.getFullYear(), 12, 1);

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetAccountComparer(form: NgForm) {
    const baseUrl = window.location.origin;
    const formValue = form.value;

    const params = {
      account: formValue.account,
      groupType: formValue.groupType,
      from: formValue.from,
      to: formValue.to,
    };

    const queryParams = new URLSearchParams(params).toString();
    const hashRoute = `#/AccountComparer/Get?${queryParams}`;

    const fullUrl = `${baseUrl}/${hashRoute}`;
    window.open(fullUrl, '_blank');
  }
}
