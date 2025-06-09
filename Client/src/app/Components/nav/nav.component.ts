import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Output,
  ViewChild,
  viewChild,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatAccordion, MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-nav',
  providers: [provideNativeDateAdapter()],
  imports: [
    MatButtonModule,
    MatExpansionModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {
  accordion = viewChild.required(MatAccordion);
  @ViewChild('nav', { static: true }) navbar: ElementRef;
  @ViewChild('toggleButton', { static: true }) toggleButton: ElementRef;
  @Output() navbarEvent = new EventEmitter<boolean>();

  constructor(
    private elementRef: ElementRef,
    private authService: AuthService
  ) {}

  @HostListener('document:click', ['$event'])
  handleOutsideClick(event: MouseEvent): void {
    const targetElement = event.target as HTMLElement;
    if (
      targetElement &&
      !this.elementRef.nativeElement.contains(targetElement)
    ) {
      if (
        !this.navbar.nativeElement.classList.contains('-translate-x-[102%]')
      ) {
        this.navbar.nativeElement.classList.add('-translate-x-[102%]');
      }
    }
  }

  toggleNavBar() {
    if (!this.navbar.nativeElement.classList.contains('lg:translate-x-0')) {
      this.navbar.nativeElement.classList.add('lg:translate-x-0');
      this.toggleButton.nativeElement.classList.add('lg:hidden');
      this.navbarEvent.emit(true);
    } else {
      this.navbar.nativeElement.classList.toggle('-translate-x-[102%]');
    }
  }

  hideNavBar() {
    this.navbar.nativeElement.classList.remove('lg:translate-x-0');
    this.toggleButton.nativeElement.classList.remove('lg:hidden');
    this.navbarEvent.emit(false);
  }

  LogOut() {
    this.authService.Logout();
  }
}
