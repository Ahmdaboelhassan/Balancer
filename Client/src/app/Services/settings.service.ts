import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DashboardSettingsDTO } from '../Interfaces/Both/DashboardSettingsDTO';
import { SettingDTO } from '../Interfaces/Both/SettingDTO';
import { Observable } from 'rxjs';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import { HttpClient } from '@angular/common/http';
import { BudgetAccountSettingsDTO } from '../Interfaces/Both/BudgetAccountSettingsDTO';

@Injectable({
  providedIn: 'root',
})
export class SettingsService {
  url = environment.baseUrl + 'Settings';
  constructor(private http: HttpClient) {}

  // Get general settings
  getSettings(): Observable<any> {
    return this.http.get(`${this.url}/GetSettings`);
  }

  // Edit general settings
  editSettings(model: SettingDTO): Observable<ConfirmationRespose> {
    return this.http.post<ConfirmationRespose>(
      `${this.url}/EditSettings`,
      model
    );
  }

  // Get dashboard settings
  getDashboardSettings(): Observable<any> {
    return this.http.get(`${this.url}/GetDashboardSettings`);
  }

  // Edit dashboard settings
  editDashboardSettings(
    model: DashboardSettingsDTO
  ): Observable<ConfirmationRespose> {
    return this.http.post<ConfirmationRespose>(
      `${this.url}/EditDashboardSettings`,
      model
    );
  }

  // Get budget account settings
  getBudgetAccountSettings(): Observable<any> {
    return this.http.get(`${this.url}/GetBudgetAccountSettings`);
  }

  // Edit budget account settings
  editBudgetAccountSettings(
    model: BudgetAccountSettingsDTO[]
  ): Observable<ConfirmationRespose> {
    return this.http.post<ConfirmationRespose>(
      `${this.url}/EditBudgetAccountSettings`,
      model
    );
  }
}
