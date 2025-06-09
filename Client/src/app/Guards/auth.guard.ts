import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../Services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const user = authService.user();

  if (authService.isPublicEndpoint(state.url) || user?.getToken) return true;

  authService.Logout();
  return false;
};
