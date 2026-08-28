import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoadingService {
  isLoad = signal(false);
  private isDisabled = false;

  LoadingStarted() {
    this.isLoad.set(true);
  }
  LoadingFinsihed() {
     this.isLoad.set(false);
  }
  
  IsDisabled() {
    return this.isDisabled;
  }
  DisableLoading() {
    this.isDisabled = true;
  }
  EnableLoading() {
    this.isDisabled = false;
  }
}
