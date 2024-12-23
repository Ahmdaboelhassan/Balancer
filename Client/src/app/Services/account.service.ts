import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

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
}
