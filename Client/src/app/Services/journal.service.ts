import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Periodjournals } from '../Interfaces/Response/Periodjournals';
import { JournalListItem } from '../Interfaces/Response/JournalListItem';
import { Journal } from '../Interfaces/Response/Journal';
import { CreateJournal } from '../Interfaces/Request/CreateJournal';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';

@Injectable({
  providedIn: 'root',
})
export class JournalService {
  url = environment.baseUrl + 'Journal';

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
  GetNewJournal(periodId?: number) {
    let url = this.url + '/New';

    if (periodId) {
      url = url + `?periodId=${periodId}`;
    }

    return this.http.get<Journal>(url);
  }

  GetEditJournal(journalId: number) {
    let url = this.url + `/Get/${journalId}`;
    return this.http.get<Journal>(url);
  }

  SaveJournal(journal: CreateJournal) {
    const url = this.url + `/Create`;
    return this.http.post<ConfirmationRespose>(url, journal);
  }

  EditJournal(journal: CreateJournal) {
    const url = this.url + `/Edit`;
    return this.http.put<ConfirmationRespose>(url, journal);
  }

  DeleteJournal(id: number) {
    const url = this.url + `/Delete/${id}`;
    return this.http.delete<ConfirmationRespose>(url);
  }
}
