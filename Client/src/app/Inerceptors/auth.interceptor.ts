import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';

import { AuthService } from '../Services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const user = authService.user();

  if (authService.isPublicEndpoint(req.url)) {
    return next(req);
  }

  if (user?.getToken) {
    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${user.getToken}`),
    });

    return next(authReq);
  }

  authService.Logout();
  return next(req);
};
