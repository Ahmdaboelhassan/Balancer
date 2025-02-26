import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AccountStatement } from '../Interfaces/Response/AccountStatement';
import { AccountsBalance } from '../Interfaces/Response/AccountsBalance';
import { AccountSummary } from '../Interfaces/Response/AccountSummary';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  url = environment.baseUrl + 'Report';

  constructor(private http: HttpClient) {}

  GetAccountStatemnt(from, to, account, costcenter, openingbalance) {
    let url =
      this.url + `/AccountStatement?from=${from}&to=${to}&account=${account}`;
    if (costcenter) {
      url += `&costcenter=${costcenter}`;
    }
    if (openingbalance) {
      url += `&openingbalance=${openingbalance}`;
    }
    return this.http.get<AccountStatement>(url);
  }

  GetIncomeStatement(from, to) {
    let url = this.url + `/IncomeStatement?from=${from}&to=${to}`;

    return this.http.get<AccountSummary[]>(url);
  }
  GetAccountsSummary(from, to) {
    let url = this.url + `/AccountsSummary?from=${from}&to=${to}`;

    return this.http.get<AccountSummary[]>(url);
  }
  GetAccountsOverview(from, to) {
    let url = this.url + `/AccountsOverview?from=${from}&to=${to}`;

    return this.http.get<AccountSummary[]>(url);
  }
}
