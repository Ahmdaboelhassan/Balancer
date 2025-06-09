import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../Services/auth.service';
import { inject } from '@angular/core';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  let authReq = req;
  const authService = inject(AuthService);
  const user = authService.user();

  if (user && user.getToken) {
    authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${user.getToken}`),
    });
  }

  return next(authReq);
};
