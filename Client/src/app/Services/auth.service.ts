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
  private url = environment.baseUrl + 'Auth';
  private refreshTokenHandler;
  private refreshTokenTimeInHours = 3;

  constructor(private http: HttpClient, private toest: ToastrService) {}

  Login(model: Login) {
    const url = this.url + `/Login`;
    this.http.post<AuthResponse>(url, model).subscribe({
      next: (response) => this.ManageLogin(response),
      error: (error) => this.manageError(error),
    });
  }

  RefreshToken(refreshToken: string) {
    const url = this.url + `/RefreshToken`;
    return this.http
      .post<AuthResponse>(url, { refreshToken: refreshToken })
      .subscribe({
        next: (response) => this.ManageLogin(response, false),
        error: (error) => this.manageError(error),
      });
  }

  Logout() {
    this.user.set(null);

    clearInterval(this.refreshTokenHandler);

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
        localStorageUser._expireOn,
        localStorageUser._refreshToken,
        localStorageUser._refreshTokenExpireOn
      );

      if (singInUser?.getToken) {
        this.user.set(singInUser);
      } else if (singInUser?.getRefreshToken) {
        this.RefreshToken(singInUser?.getRefreshToken);
      }
    } else {
      this.Logout();
    }
  }

  AutoRefreshToken() {
    var refreshToken = this.user()?.getRefreshToken;
    if (refreshToken) {
      const timeInMillSecounds = this.refreshTokenTimeInHours * 60 * 60 * 1000;

      this.refreshTokenHandler = setInterval(() => {
        this.RefreshToken(refreshToken);
      }, timeInMillSecounds);
    }
  }

  isPublicEndpoint(route: string): boolean {
    const publicEndpoints = ['/Login', '/RefreshToken', '/Refresh', '/public'];
    return publicEndpoints.some((endpoint) => route.includes(endpoint));
  }

  private ManageLogin(response: AuthResponse, ShowHelloMessage = true) {
    const user: User = new User(
      response.userName,
      response.token,
      response.expireOn,
      response.refreshToken,
      response.refreshTokenExpireOn
    );

    this.user.set(user);
    localStorage.setItem('user', JSON.stringify(user));
    if (ShowHelloMessage) {
      this.toest.success(`Hi ${user.Username}`);
    }
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
