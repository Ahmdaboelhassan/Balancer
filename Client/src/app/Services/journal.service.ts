import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Periodjournals } from '../Interfaces/Response/Periodjournals';
import { JournalListItem } from '../Interfaces/Response/JournalListItem';
import { Journal } from '../Interfaces/Response/Journal';
import { CreateJournal } from '../Interfaces/Request/CreateJournal';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import { Subject } from 'rxjs';
import { Result } from '../Interfaces/Response/Result';

@Injectable({
  providedIn: 'root',
})
export class JournalService {
  url = environment.baseUrl + 'Journal';
  advancedSearch$ = new Subject();
  journalsSummary$ = new Subject();

  constructor(private http: HttpClient) {}

  GetPeriodJournals(periodId: number) {
    const url = this.url + `/GetJournals/${periodId}`;
    return this.http.get<Periodjournals>(url);
  }
  GetJournals(from: string, to: string) {
    const url = this.url + `/FilterByTime?From=${from}&To=${to}`;
    return this.http.get<JournalListItem[]>(url);
  }
  SearchJournals(key: string) {
    const url = this.url + `/Search?criteria=${key}`;
    return this.http.get<JournalListItem[]>(url);
  }

  AvancedSearchJournals(keys) {
    const params = new HttpParams()
      .set('key', keys.key || '')
      .set('from', keys.from || '')
      .set('to', keys.to || '')
      .set('orderBy', keys.orderBy || '')
      .set('type', keys.type || '')
      .set('filterByDate', keys.filterByDate || false)
      .set('filterByKey', keys.filterByKey || false);

    return this.http.get<JournalListItem[]>(`${this.url}/AdvancedSearch`, {
      params,
    });
  }

  GetNewJournal(periodId?: number) {
    let url = this.url + '/New';

    if (periodId) {
      url = url + `?periodId=${periodId}`;
    }

    return this.http.get<Result<Journal>>(url);
  }

  GetEditJournal(journalId: number) {
    let url = this.url + `/Get/${journalId}`;
    return this.http.get<Result<Journal>>(url);
  }

  SaveJournal(journal: CreateJournal) {
    const url = this.url + `/Create`;
    return this.http.post<Result<number>>(url, journal);
  }

  EditJournal(journal: CreateJournal) {
    const url = this.url + `/Edit`;
    return this.http.put<Result<number>>(url, journal);
  }

  DeleteJournal(id: number) {
    const url = this.url + `/Delete/${id}`;
    return this.http.delete<ConfirmationRespose>(url);
  }

  GetAdjacentJournal(id, isNext) {
    const action = isNext ? '/GetNextJournal' : '/GetPrevJournal';
    const url = this.url + action + `/${id}`;

    return this.http.get<Result<Journal>>(url);
  }

  AdjustBudget(amount: number) {
    const url = this.url + `/AdjustBudget`;
    return this.http.post<ConfirmationRespose>(url, { amount });
  }
}
