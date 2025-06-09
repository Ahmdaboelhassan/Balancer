import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../Services/auth.service';
import { firstValueFrom } from 'rxjs';

export const authGuard: CanActivateFn = async (route, state) => {
  const authService = inject(AuthService);
  const user = authService.user();

  if (user && user.getToken) {
    return true;
  }

  if (user && user.getRefreshToken) {
    try {
      const response = await firstValueFrom(
        authService.RefreshToken(user.getRefreshToken)
      );
      authService.ManageLogin(response);
      return true;
    } catch (error) {
      authService.manageError(error);
      return false;
    }
  }
  authService.Logout();
  return false;
};
