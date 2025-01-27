import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../Services/loading.service';
import { inject } from '@angular/core';
import { delay, finalize } from 'rxjs';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  loadingService.LoadingStarted();

  return next(req).pipe(
    delay(2000),
    finalize(() => loadingService.LoadingFinsihed())
  );
};
