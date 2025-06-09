import { Component, computed, OnInit, signal } from '@angular/core';
import { AuthComponent } from './Components/auth/auth.component';
import { NavComponent } from './Components/nav/nav.component';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './Components/loading/loading.component';
import { LoadingService } from './Services/loading.service';
import { AuthService } from './Services/auth.service';

@Component({
  selector: 'app-root',
  imports: [NavComponent, AuthComponent, LoadingComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  user = computed(() => this.authService.user());
  isLoad = computed(() => this.loadingService.isLoad());

  constructor(
    private loadingService: LoadingService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authService.AutoLogin();
    this.authService.AutoRefreshToken();
  }

  handleNavbarDispaly(
    isDispaly: boolean,
    navBarHolder: HTMLElement,
    appBody: HTMLElement
  ) {
    if (isDispaly) {
      navBarHolder.classList.remove('lg:absolute');
      appBody.classList.add('lg:col-span-4');
    } else {
      navBarHolder.classList.add('lg:absolute');
      appBody.classList.remove('lg:col-span-4');
    }
  }
}
