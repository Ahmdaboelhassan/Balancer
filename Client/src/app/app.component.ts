import {
  Component,
  computed,
  HostListener,
  OnInit,
  signal,
} from '@angular/core';
import { AuthComponent } from './Components/auth/auth.component';
import { NavComponent } from './Components/nav/nav.component';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './Components/loading/loading.component';
import { LoadingService } from './Services/loading.service';
import { AuthService } from './Services/auth.service';
import { Router } from '@angular/router';

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
    private authService: AuthService,
    private router: Router
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

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (
      event.ctrlKey &&
      event.altKey &&
      (event.key === 'J' || event.key === 'j')
    ) {
      event.preventDefault();
      this.router.navigate(['/', 'Journal', 'Create']);
    } else if (
      event.ctrlKey &&
      event.altKey &&
      (event.key === 'K' || event.key === 'k')
    ) {
      event.preventDefault();
      this.router.navigate(['/', 'AccountStatement']);
    }
  }
}
