import { Component, computed, inject } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { LoadingService } from '../../Services/loading.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-auth',
  imports: [FormsModule, LoadingComponent],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.css',
})
export class AuthComponent {
  authService = inject(AuthService);
  loadingService = inject(LoadingService);

  isLoad = computed(() => this.loadingService.isLoad());

  Login(form: NgForm) {
    if (form.valid) {
      this.authService.Login(form.value);
    }
  }
}
