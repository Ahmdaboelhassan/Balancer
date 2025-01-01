import { Component, Signal, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../../Services/account.service';
import { CostcenterService } from '../../Services/costcenter.service';
import { AccountSelectList } from '../../Interfaces/Response/AccountSelectList';
import { CostCenterSelectList } from '../../Interfaces/Response/CostCenterSelectList';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-accountstatement',
  imports: [FormsModule, NgFor],
  templateUrl: './accountstatement.component.html',
  styleUrl: './accountstatement.component.css',
})
export class AccountstatementComponent {
  from = signal('');
  to = signal('');
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];
  openingBalance: boolean = true;

  constructor(
    private router: Router,
    private accountService: AccountService,
    private costCenterService: CostcenterService
  ) {
    this.GetDefaultDate();
    this.accountService
      .GetAllAccountSelectList()
      .subscribe({ next: (accounts) => (this.accounts = accounts) });
    this.costCenterService
      .GetAllCostCenterSelectList()
      .subscribe({ next: (costCenters) => (this.costCenters = costCenters) });
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
    this.router.navigate(['AccountStatement/Get'], { queryParams: form.value });
  }
}
