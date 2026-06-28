import { Component, inject, OnInit } from '@angular/core';
import { AccountsListComponent } from './accounts-list/accounts-list.component';
import { AccountService } from '../../Services/account.service';
import { Account } from '../../Interfaces/Response/Account';
import { SearchComponent } from '../search/search.component';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-account',
  imports: [AccountsListComponent, SearchComponent],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css',
})
export class AccountComponent implements OnInit {
  AccountCounts: any;

  accounts: Account[] = [];
  private accountService = inject(AccountService);
  private route = inject(ActivatedRoute);

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      const archive = params.get('archive') === 'true';

      this.accountService.GetAllAccount(archive).subscribe({
        next: (accounts) => {
          this.accounts = accounts;
        },
      });
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
