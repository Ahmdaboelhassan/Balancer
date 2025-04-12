import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../Services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if (authService.user().getToken) {
    return true;
  } else {
    authService.Logout();
    return false;
  }
};
