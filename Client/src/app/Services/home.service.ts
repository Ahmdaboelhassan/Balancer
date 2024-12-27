import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Home } from '../Interfaces/Response/Home';

@Injectable({
  providedIn: 'root',
})
export class HomeService {
  constructor(private http: HttpClient) {}
  url = environment.baseUrl;

  GetHome() {
    let url = this.url + 'Home/Index';
    return this.http.get<Home>(url);
  }
}
