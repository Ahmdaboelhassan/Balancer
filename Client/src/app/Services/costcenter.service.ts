import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { CostCenter } from '../Interfaces/Response/CostCenter';
import { CreateCostCenter } from '../Interfaces/Request/CreateCostCenter';
import { ConfirmationRespose } from '../Interfaces/Response/ConfirmationRespose';
import { CostCenterSelectList } from '../Interfaces/Response/CostCenterSelectList';

@Injectable({
  providedIn: 'root',
})
export class CostcenterService {
  url = environment.baseUrl + 'CostCenter';

  constructor(private http: HttpClient) {}

  GetCostCenter(CostCenterId) {
    const url = this.url + `/Get/${CostCenterId}`;
    return this.http.get<CostCenter>(url);
  }

  GetAllCostCenter() {
    const url = this.url + `/GetAll`;
    return this.http.get<CostCenter[]>(url);
  }
  GetAllCostCenterSelectList() {
    const url = this.url + `/GetAllSelectList`;
    return this.http.get<CostCenterSelectList[]>(url);
  }
  CreateCostCenter(CostCenter: CreateCostCenter) {
    const url = this.url + `/Create`;
    return this.http.post<ConfirmationRespose>(url, CostCenter);
  }
  EditCostCenter(CostCenter: CreateCostCenter) {
    const url = this.url + `/Edit`;
    return this.http.put<ConfirmationRespose>(url, CostCenter);
  }
  DeleteCostCenter(CostCenterId) {
    const url = this.url + `/Delete/${CostCenterId}`;
    return this.http.delete<ConfirmationRespose>(url);
  }
}
