import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../Services/loading.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  loadingService.LoadingStarted();

  return next(req).pipe(finalize(() => loadingService.LoadingFinsihed()));
};
