import { Component, Input } from '@angular/core';
import { AccountListItemComponent } from './account-list-item/account-list-item.component';
import { Account } from '../../../Interfaces/Response/Account';

@Component({
  selector: 'app-accounts-list',
  imports: [AccountListItemComponent],
  templateUrl: './accounts-list.component.html',
  styleUrl: './accounts-list.component.css',
})
export class AccountsListComponent {
  @Input() accounts: Account[];
}
