import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AccountStatement } from '../Interfaces/Response/AccountStatement';

import { AccountSummary } from '../Interfaces/Response/AccountSummary';
import { AccountComparer } from '../Interfaces/Response/AccountComparer';

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

  GetCostCenterStatemnt(from, to, costcenter, openingbalance) {
    let url =
      this.url +
      `/CostCenterStatement?from=${from}&to=${to}&costcenter=${costcenter}`;

    if (openingbalance) {
      url += `&openingbalance=${openingbalance}`;
    }
    return this.http.get<AccountStatement>(url);
  }

  GetAccountComparer(from, to, account, costcenter, groupType) {
    let url =
      this.url +
      `/AccountComparer?from=${from}&to=${to}&account=${account}&groupType=${groupType}`;

    if (costcenter) {
      url += `&costcenter=${costcenter}`;
    }
    return this.http.get<AccountComparer>(url);
  }

  GetIncomeStatement(from, to) {
    let url = this.url + `/IncomeStatement?from=${from}&to=${to}`;

    return this.http.get<AccountSummary[]>(url);
  }
  GetAccountsSummary(from, to) {
    let url = this.url + `/AccountsSummary?from=${from}&to=${to}`;

    return this.http.get<AccountSummary[]>(url);
  }
  GetAccountsOverview(from, to, maxLevel) {
    let url = this.url + `/AccountsOverview?from=${from}&to=${to}`;

    if (maxLevel) {
      url += `&maxLevel=${maxLevel}`;
    }
    return this.http.get<AccountSummary[]>(url);
  }
}
