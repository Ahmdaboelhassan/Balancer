import { HttpClient } from '@angular/common/http';
import { Injectable, signal, WritableSignal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Login } from '../Interfaces/Request/Login';
import { User } from '../Models/User';
import { AuthResponse } from '../Interfaces/Response/AuthResponse';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  user: WritableSignal<User> = signal(null);

  url = environment.baseUrl + 'Auth';
  constructor(private http: HttpClient, private toest: ToastrService) {}

  Login(model: Login) {
    const url = this.url + `/Login`;
    this.http.post<AuthResponse>(url, model).subscribe({
      next: (response) => this.ManageLogin(response),
      error: (error) => this.manageError(error),
    });
  }

  Logout() {
    this.user.set(null);

    if (localStorage.getItem('user')) {
      localStorage.removeItem('user');
      localStorage.removeItem('token');
    }
  }

  AutoLogin() {
    if (localStorage.getItem('user')) {
      let localStorageUser = JSON.parse(localStorage.getItem('user'));

      let singInUser = new User(
        localStorageUser.Username,
        localStorageUser._token,
        localStorageUser._expireOn
      );

      if (singInUser?.getToken) {
        this.user.set(singInUser);
      }
    } else {
      this.user.set(null);
    }
  }

  private ManageLogin(response: AuthResponse) {
    const user: User = new User(
      response.userName,
      response.token,
      response.expireOn
    );

    this.user.set(user);
    localStorage.setItem('user', JSON.stringify(user));
    localStorage.setItem('token', response.token);
    this.toest.success(`Hi ${user.Username}`);
  }

  private manageError(resError) {
    const errorObject = resError.error;
    let errorList: string[] = [];
    if (resError.status == 500) {
      errorList.push('Internal Server Error');
    } else if (errorObject.errors) {
      Object.keys(errorObject.errors).forEach((key) => {
        errorList.push(...errorObject.errors[key]);
      });
    } else if (errorObject) {
      errorList.push(errorObject['message']);
    } else {
      errorList.push('Unknown Error Occured');
    }
    errorList.forEach((el) => {
      this.toest.error(el);
    });
  }
}
