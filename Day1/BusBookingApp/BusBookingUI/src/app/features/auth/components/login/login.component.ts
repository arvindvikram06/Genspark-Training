import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-login',
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
    <div class="auth-wrapper flex items-center justify-center min-h-screen bg-slate-900 px-4">
      <div class="auth-card w-full max-w-[420px] bg-white p-8 rounded-xl shadow-2xl border border-slate-200">
        
        @if(showRegisteredBanner) {
          <div class="bg-emerald-50 border border-emerald-200 text-emerald-700 px-4 py-3 rounded-lg mb-6 text-sm">
            Account created. Please log in.
          </div>
        }

        <h2 class="text-3xl font-bold text-slate-800 mb-2">Welcome Back</h2>
        <p class="text-slate-500 mb-8">Enter your credentials to access your account</p>

        <form [formGroup]="loginForm" (ngSubmit)="onLogin()" class="space-y-4">
          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Email Address</mat-label>
            <input matInput type="email" formControlName="email" placeholder="name@example.com">
            <mat-error *ngIf="loginForm.get('email')?.hasError('required')">Email is required</mat-error>
            <mat-error *ngIf="loginForm.get('email')?.hasError('email')">Invalid email format</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Password</mat-label>
            <input matInput type="password" formControlName="password" placeholder="••••••••">
            <mat-error *ngIf="loginForm.get('password')?.hasError('required')">Password is required</mat-error>
          </mat-form-field>

          @if(error) {
            <p class="text-red-500 text-sm font-medium mt-1">{{ error }}</p>
          }

          <button mat-flat-button color="primary" class="w-full h-12 !rounded-lg text-lg font-semibold" 
                  [disabled]="loginForm.invalid || loading">
            <span *ngIf="!loading">Sign In</span>
            <mat-spinner *ngIf="loading" diameter="24" class="mx-auto"></mat-spinner>
          </button>
        </form>

        <div class="mt-8 pt-6 border-t border-slate-100 text-center space-y-3">
          <p class="text-slate-600 text-sm">
            Don't have an account? 
            <a routerLink="/auth/register-user" class="text-indigo-600 font-semibold hover:underline">Register</a>
          </p>
          <p class="text-slate-600 text-sm">
            <a routerLink="/auth/register-operator" class="text-indigo-600 font-semibold hover:underline">Register as operator</a>
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
    
    /* Fix Submit Button Visibility */
    ::ng-deep .mat-mdc-unelevated-button.mat-primary {
      background-color: #6366f1 !important;
      color: #ffffff !important;
    }
    
    ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: contents; }

    @keyframes slideUp {
      from { opacity: 0; transform: translateY(20px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  loading = false;
  error = '';
  showRegisteredBanner = false;

  ngOnInit() {
    // Redirect if already logged in
    const userStr = localStorage.getItem('user');
    if (userStr) {
      const user = JSON.parse(userStr);
      if (user.role === 'Admin') this.router.navigate(['/admin/dashboard']);
      else if (user.role === 'Operator') this.router.navigate(['/operator/dashboard']);
      else this.router.navigate(['/user/dashboard']);
      return;
    }

    this.route.queryParams.subscribe(params => {
      this.showRegisteredBanner = params['registered'] === 'true';
    });
  }

  onLogin() {
    if (this.loginForm.invalid) return;

    this.loading = true;
    this.error = '';

    const { email, password } = this.loginForm.value;
    this.authService.login(email!, password!).subscribe({
      next: (res) => {
        if (res && res.success && res.data) {
          const role = res.data.role;
          if (role === 'Admin') this.router.navigate(['/admin/dashboard']);
          else if (role === 'Operator') this.router.navigate(['/operator/dashboard']);
          else this.router.navigate(['/user/dashboard']);
        } else {
          this.error = res.message || 'Login failed. Please check your credentials.';
          this.loading = false;
        }
      },
      error: (err) => {
        this.error = 'Invalid email or password';
        this.loading = false;
      }
    });
  }
}
