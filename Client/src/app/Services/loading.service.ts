import { Injectable, signal } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  isLoad = signal(false);

  LoadingStarted() {
    this.isLoad.set(true);
  }
  LoadingFinsihed() {
    this.isLoad.set(false);
  }
}
