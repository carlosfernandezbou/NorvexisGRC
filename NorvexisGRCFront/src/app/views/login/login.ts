import { CommonModule } from '@angular/common';
import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../services/loginService';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private services = inject(LoginService);
  private cdr = inject(ChangeDetectorRef);

  loading = false;
  showPassword = false;
  errorMessage = '';

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [false],
  });

  get emailInvalid(): boolean {
    const field = this.loginForm.controls.email;
    return field.invalid && (field.dirty || field.touched);
  }

  get passwordInvalid(): boolean {
    const field = this.loginForm.controls.password;
    return field.invalid && (field.dirty || field.touched);
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  submitLogin(): void {
    this.errorMessage = '';

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;

    const { email, password } = this.loginForm.getRawValue();

    this.services.login(email!, password!).subscribe({
      next: (response) => {
        this.services.saveToken(
          response.accessToken,
          response.expiresInMinutes
        );

        const userId = response.user.id;

        this.services.saveUserId(userId);

        this.services.getUserRoles(userId).subscribe({
          next: (roles) => {
            const role = roles[0]?.name ?? roles[0];

            this.services.saveUserRole(role);

            this.loading = false;
            this.router.navigateByUrl('/home');
          },
          error: () => {
            this.loading = false;
            this.errorMessage = 'Error loading user role.';
          }
        });
      },
      error: (error) => {
        this.loading = false;

        if (error.status === 423) {
          this.errorMessage =
            'Account temporarily locked due to multiple failed attempts. Try again in 15 minutes.';
        } else if (error.status === 429) {
          this.errorMessage =
            'Too many requests. Please wait a few minutes and try again.';
        } else {
          this.errorMessage = 'Email or password incorrect.';
        }

        this.cdr.detectChanges();
      }
    });
  }
}