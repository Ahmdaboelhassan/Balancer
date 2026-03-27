import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize, catchError, throwError } from 'rxjs';
import { LoadingService } from '../Services/loading.service';
import Swal from 'sweetalert2';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);

  loadingService.LoadingStarted();

  return next(req).pipe(
    catchError((error) => {
      if (!navigator.onLine) {
        Swal.fire({
          icon: 'error',
          title: 'No Internet',
          text: 'Please check your internet connection',
        });
      } else if (error.status === 0) {
        Swal.fire({
          icon: 'error',
          title: 'Server Unreachable',
          text: 'Cannot connect to server',
        });
      } else if (error.status === 500) {
        Swal.fire({
          icon: 'error',
          title: 'Server Error',
          text: 'Something went wrong on the server',
        });
      }

      return throwError(() => error);
    }),
    finalize(() => loadingService.LoadingFinsihed()),
  );
};
