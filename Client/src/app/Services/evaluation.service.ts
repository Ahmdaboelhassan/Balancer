import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { EvaluationListItem } from '../Interfaces/Response/EvaluationListItem';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import {
  Evaluation,
  EvaluationDetail,
} from '../Interfaces/Response/Evaluation';
import { Result } from '../Interfaces/Response/Result';

@Injectable({
  providedIn: 'root',
})
export class EvaluationService {
  url = environment.baseUrl + 'Evaluation';

  constructor(private http: HttpClient) {}

  GetAll(from: string, to: string) {
    const url = this.url + `/GetAll?from=${from}&to=${to}`;
    return this.http.get<EvaluationListItem>(url);
  }

  New() {
    const url = this.url + `/New`;
    return this.http.get<Evaluation>(url);
  }
  Get(evaluationId) {
    const url = this.url + `/Get/${evaluationId}`;
    return this.http.get<Evaluation>(url);
  }

  CalculateDetailsBalances(evaluation: Evaluation) {
    const url = this.url + `/CalculateDetailsBalances`;
    return this.http.post<Result<Evaluation>>(url, evaluation);
  }

  Create(evaluation: Evaluation) {
    const url = this.url + `/Create`;
    return this.http.post<ConfirmationRespose>(url, evaluation);
  }
  Edit(evaluation: Evaluation) {
    const url = this.url + `/Edit`;
    return this.http.put<ConfirmationRespose>(url, evaluation);
  }
  Delete(evaluationId: number) {
    const url = this.url + `/Delete/${evaluationId}`;
    return this.http.delete<ConfirmationRespose>(url);
  }
}
