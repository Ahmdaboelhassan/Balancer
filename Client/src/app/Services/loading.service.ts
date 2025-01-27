import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  isLoad = signal(false);
  private isDisabled = false;

  LoadingStarted() {
    if (!this.isDisabled) {
      this.isLoad.set(true);
    }
  }
  LoadingFinsihed() {
    this.isLoad.set(false);
  }
  DisableLoading() {
    this.isDisabled = true;
  }
  EnableLoading() {
    this.isDisabled = false;
  }
}
