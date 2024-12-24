import { Component, Input } from '@angular/core';
import { Account } from '../../../../Interfaces/Response/Account';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-account-list-item',
  imports: [RouterLink],
  templateUrl: './account-list-item.component.html',
  styleUrl: './account-list-item.component.css',
})
export class AccountListItemComponent {
  @Input() account: Account;
}
