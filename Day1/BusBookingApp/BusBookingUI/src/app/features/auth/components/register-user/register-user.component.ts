import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/services/auth.service';

export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  return password && confirmPassword && password.value !== confirmPassword.value ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register-user',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="auth-wrapper flex items-center justify-center min-h-screen bg-slate-900 px-4 py-12">
      <div class="auth-card w-full max-w-[420px] bg-white p-8 rounded-xl shadow-2xl border border-slate-200">
        <h2 class="text-3xl font-bold text-slate-800 mb-2">Create Account</h2>
        <p class="text-slate-500 mb-8">Join us to start booking your bus trips</p>

        <form [formGroup]="registerForm" (ngSubmit)="onRegister()" class="space-y-4">
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Full Name</mat-label>
            <input matInput formControlName="name" placeholder="John Doe">
            <mat-error *ngIf="registerForm.get('name')?.hasError('required')">Name is required</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Email Address</mat-label>
            <input matInput type="email" formControlName="email" placeholder="name@example.com">
            <mat-error *ngIf="registerForm.get('email')?.hasError('required')">Email is required</mat-error>
            <mat-error *ngIf="registerForm.get('email')?.hasError('email')">Invalid email format</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Phone Number</mat-label>
            <input matInput type="tel" formControlName="phone" placeholder="10-digit number">
            <mat-error *ngIf="registerForm.get('phone')?.hasError('required')">Phone is required</mat-error>
            <mat-error *ngIf="registerForm.get('phone')?.hasError('pattern')">Must be a 10-digit number</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password" placeholder="Min 8 characters">
            <mat-error *ngIf="registerForm.get('password')?.hasError('required')">Password is required</mat-error>
            <mat-error *ngIf="registerForm.get('password')?.hasError('minlength')">Minimum 8 characters required</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Confirm Password</mat-label>
            <input matInput type="password" formControlName="confirmPassword" placeholder="Repeat password">
            <mat-error *ngIf="registerForm.get('confirmPassword')?.hasError('required')">Please confirm your password</mat-error>
            <mat-error *ngIf="registerForm.hasError('passwordMismatch') && registerForm.get('confirmPassword')?.touched">Passwords do not match</mat-error>
          </mat-form-field>

          @if(error) {
            <p class="text-red-500 text-sm font-medium mt-1">{{ error }}</p>
          }

          <button mat-flat-button color="primary" class="w-full h-12 !rounded-lg text-lg font-semibold" 
                  [disabled]="registerForm.invalid || loading">
            <span *ngIf="!loading">Sign Up</span>
            <mat-spinner *ngIf="loading" diameter="24" class="mx-auto"></mat-spinner>
          </button>
        </form>

        <div class="mt-8 pt-6 border-t border-slate-100 text-center">
          <p class="text-slate-600 text-sm">
            Already have an account? 
            <a routerLink="/auth/login" class="text-indigo-600 font-semibold hover:underline">Log In</a>
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .auth-wrapper { background: #0f172a; }
    .auth-card { animation: slideUp 0.4s ease-out; background: #ffffff !important; color: #1e293b !important; }
    
    /* Force high contrast for Material form fields */
    ::ng-deep .mat-mdc-form-field { width: 100%; }
    ::ng-deep .mat-mdc-form-field-label-wrapper label { color: #475569 !important; font-weight: 600 !important; }
    ::ng-deep .mat-mdc-text-field-wrapper { background-color: #f8fafc !important; }
    ::ng-deep .mdc-text-field--outlined:not(.mdc-text-field--disabled) .mdc-notched-outline__leading,
    ::ng-deep .mdc-text-field--outlined:not(.mdc-text-field--disabled) .mdc-notched-outline__notch,
    ::ng-deep .mdc-text-field--outlined:not(.mdc-text-field--disabled) .mdc-notched-outline__trailing {
      border-color: #94a3b8 !important;
      border-width: 1px !important;
    }
    ::ng-deep .mat-mdc-form-field-input-control { color: #0f172a !important; font-weight: 500 !important; }
    ::ng-deep input::placeholder { color: #94a3b8 !important; opacity: 1 !important; }
    
    ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: contents; }

    @keyframes slideUp {
      from { opacity: 0; transform: translateY(20px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class RegisterUserComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm = this.fb.group({
    name: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });

  loading = false;
  error = '';

  onRegister() {
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.error = '';

    const userData = {
      name: this.registerForm.value.name,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      phone: this.registerForm.value.phone
    };

    this.authService.registerUser(userData).subscribe({
      next: () => {
        this.router.navigate(['/auth/login'], { queryParams: { registered: 'true' } });
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed. Please try again.';
        this.loading = false;
      }
    });
  }
}
