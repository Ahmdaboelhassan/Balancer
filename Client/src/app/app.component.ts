import { Component, computed, OnInit, signal } from '@angular/core';
import { AuthComponent } from './Components/auth/auth.component';
import { NavComponent } from './Components/nav/nav.component';
import { RouterOutlet } from '@angular/router';
import { LoadingComponent } from './Components/loading/loading.component';
import { LoadingService } from './Services/loading.service';

@Component({
  selector: 'app-root',
  imports: [NavComponent, AuthComponent, LoadingComponent, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  isAuth = signal(true);
  isLoad = computed(() => this.loadingService.isLoad());
  constructor(private loadingService: LoadingService) {}
}
