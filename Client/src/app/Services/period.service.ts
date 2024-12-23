import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PeriodListItem } from '../Interfaces/Response/PeriodListItem';
import { environment } from '../../environments/environment';
import { Period } from '../Interfaces/Response/Period';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import { CreatePeriod } from '../Interfaces/Request/CreatePeriod';

@Injectable({
  providedIn: 'root',
})
export class PeriodService {
  url = environment.baseUrl + 'Period';

  constructor(private http: HttpClient) {}

  GetPeriod(from: string, to: string) {
    const url = this.url + `/FilterByTime?from=${from}&to=${to}`;
    return this.http.get<PeriodListItem>(url);
  }
  SearchPeriod(key: string) {
    const url = this.url + `/Search?criteria=${key}`;
    return this.http.get<PeriodListItem>(url);
  }
  GetCreatePeriod() {
    const url = this.url + `/New`;
    return this.http.get<Period>(url);
  }
  GetPeriodDetail(id: number) {
    const url = this.url + `/Get/${id}`;
    return this.http.get<Period>(url);
  }
  CreatePeriod(period: CreatePeriod) {
    const url = this.url + `/Create`;
    return this.http.post<ConfirmationRespose>(url, period);
  }
  EditPeriod(period: CreatePeriod) {
    const url = this.url + `/Edit`;
    return this.http.put<ConfirmationRespose>(url, period);
  }
  DeletePeriod(periodId: number) {
    const url = this.url + `/Delete/${periodId}`;
    return this.http.delete<ConfirmationRespose>(url);
  }
}
