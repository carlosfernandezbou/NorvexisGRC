import { Component, inject, ElementRef, HostListener } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { LoginService } from '../services/loginService';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, NgIf],
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class HeaderComponent {
  private router = inject(Router);
  private services = inject(LoginService);

  riskMenuOpen = false;
  mobileMenuOpen = false;

  constructor(private eRef: ElementRef) {}

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
    this.riskMenuOpen = false;
  }

  toggleRiskMenu(event?: Event) {
    event?.stopPropagation();
    this.riskMenuOpen = !this.riskMenuOpen;
  }

  closeAllMenus() {
    this.mobileMenuOpen = false;
    this.riskMenuOpen = false;
  }

  logout(): void {
    this.services.logout().subscribe({
      next: () => this.services.clearSession(),
      error: () => this.services.clearSession()
    });

    this.router.navigateByUrl('/login');
  }

  @HostListener('document:click', ['$event'])
  clickOutside(event: Event) {
    if (!this.eRef.nativeElement.contains(event.target)) {
      this.closeAllMenus();
    }
  }
}