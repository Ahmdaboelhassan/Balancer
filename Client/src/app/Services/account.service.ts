import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { CreateAccount } from '../Interfaces/Request/CreateAccount';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import { Account } from '../Interfaces/Response/Account';
import { AccountSelectList } from '../Interfaces/Response/AccountSelectList';
import { AccountTreeItem } from '../Interfaces/Response/AccountingTreeItem';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  url = environment.baseUrl + 'Account';

  constructor(private http: HttpClient) {}

  GetAccountBalance(accountId) {
    const url = this.url + `/GetBalance/${accountId}`;
    return this.http.get<{ balance: number }>(url);
  }
  GetPrimaryAccounts() {
    const url = this.url + `/GetPrimaryAccounts`;
    return this.http.get<AccountTreeItem[]>(url);
  }

  GetChilds(accountId) {
    const url = this.url + `/GetChilds/${accountId}`;
    return this.http.get<AccountTreeItem[]>(url);
  }
  GetAccount(accountId) {
    const url = this.url + `/Get/${accountId}`;
    return this.http.get<Account>(url);
  }

  SearchAccounts(key) {
    const url = this.url + `/Search?criteria=${key}`;
    return this.http.get<Account[]>(url);
  }
  GetAllAccount() {
    const url = this.url + `/GetAll`;
    return this.http.get<Account[]>(url);
  }
  GetAllAccountSelectList() {
    const url = this.url + `/GetSelectList`;
    return this.http.get<AccountSelectList[]>(url);
  }
  CreateAccount(account: CreateAccount) {
    const url = this.url + `/Create`;
    return this.http.post<ConfirmationRespose>(url, account);
  }
  EditAccount(Account: CreateAccount) {
    const url = this.url + `/Edit`;
    return this.http.put<ConfirmationRespose>(url, Account);
  }
  DeleteAccount(accountId) {
    const url = this.url + `/Delete/${accountId}`;
    return this.http.delete<ConfirmationRespose>(url);
  }
}
