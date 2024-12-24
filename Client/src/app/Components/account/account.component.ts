import { Component, inject, OnInit } from '@angular/core';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountService } from '../../Services/account.service';
import { Account } from '../../Interfaces/Response/Account';
import { RouterLink } from '@angular/router';
import { SearchComponent } from '../search/search.component';

@Component({
  selector: 'app-account',
  imports: [AccountsListComponent, RouterLink, SearchComponent],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent implements OnInit {
  AccountCounts: any;

  accounts: Account[] = [];
  private accountService = inject(AccountService);

  ngOnInit(): void {
    this.accountService.GetAllAccount().subscribe({
      next: (accounts) => {
        this.accounts = accounts;
      },
    });
  }

  SearchAccounts(key: any) {
    this.accountService.SearchAccounts(key).subscribe({
      next: (accounts) => {
        this.accounts = accounts;
        this.AccountCounts = this.accounts.reduce((am, acc) => am + 1, 0);
      },
    });
  }
}
