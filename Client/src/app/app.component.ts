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
  }
}
