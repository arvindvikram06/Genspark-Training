import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../../core/services/auth.service';
import { OperatorService } from '../../../../core/services/operator.service';
import { passwordMatchValidator } from '../register-user/register-user.component';

@Component({
  selector: 'app-register-operator',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="auth-wrapper flex items-center justify-center min-h-screen bg-slate-900 px-4 py-12">
      <div class="auth-card w-full max-w-[420px] bg-white p-8 rounded-xl shadow-2xl border border-slate-200 text-center">
        
        @if(!submitted) {
          <h2 class="text-3xl font-bold text-slate-800 mb-2">Operator Registration</h2>
          <p class="text-slate-500 mb-8 text-left">Register your business to start managing buses</p>

          <form [formGroup]="registerForm" (ngSubmit)="onRegister()" class="space-y-4 text-left">
            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Business Name</mat-label>
              <input matInput formControlName="name" placeholder="Murugan Travels">
              <mat-error *ngIf="registerForm.get('name')?.hasError('required')">Name is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Email Address</mat-label>
              <input matInput type="email" formControlName="email" placeholder="name@example.com">
              <mat-error *ngIf="registerForm.get('email')?.hasError('required')">Email is required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Phone Number</mat-label>
              <input matInput type="tel" formControlName="phone" placeholder="10-digit number">
              <mat-error *ngIf="registerForm.get('phone')?.hasError('pattern')">Must be a 10-digit number</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Password</mat-label>
              <input matInput type="password" formControlName="password" placeholder="Min 8 characters">
              <mat-error *ngIf="registerForm.get('password')?.hasError('minlength')">Minimum 8 characters required</mat-error>
            </mat-form-field>

            <mat-form-field appearance="outline" class="w-full">
              <mat-label>Confirm Password</mat-label>
              <input matInput type="password" formControlName="confirmPassword">
              <mat-error *ngIf="registerForm.hasError('passwordMismatch') && registerForm.get('confirmPassword')?.touched">Passwords do not match</mat-error>
            </mat-form-field>

            @if(error) {
              <p class="text-red-500 text-sm font-medium mt-1">{{ error }}</p>
            }

            <button mat-flat-button color="primary" class="w-full h-12 !rounded-lg text-lg font-semibold" 
                    [disabled]="registerForm.invalid || loading">
              <span *ngIf="!loading">Register Now</span>
              <mat-spinner *ngIf="loading" diameter="24" class="mx-auto"></mat-spinner>
            </button>
          </form>

          <div class="mt-8 pt-6 border-t border-slate-100 text-center">
            <p class="text-slate-600 text-sm">
              Already have an account? 
              <a routerLink="/auth/login" class="text-indigo-600 font-semibold hover:underline">Log In</a>
            </p>
          </div>
        } @else {
          <div class="py-6">
            <div class="w-20 h-20 bg-amber-100 text-amber-600 rounded-full flex items-center justify-center mx-auto mb-6">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <h2 class="text-2xl font-bold text-slate-800 mb-4">Waiting for Approval</h2>
            <p class="text-slate-600 mb-8 leading-relaxed">
              Your registration has been submitted successfully. <br>
              An admin will review and approve your account shortly.
            </p>
            <button mat-stroked-button color="primary" routerLink="/auth/login" class="w-full h-12 !rounded-lg font-semibold">
              Back to Login
            </button>
          </div>
        }
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
    ::ng-deep .mat-mdc-select-value-text { color: #0f172a !important; font-weight: 500 !important; }
    ::ng-deep input::placeholder { color: #94a3b8 !important; opacity: 1 !important; }
    
    ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: contents; }

    @keyframes slideUp {
      from { opacity: 0; transform: translateY(20px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class RegisterOperatorComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  loading = false;
  submitted = false;
  error = '';

  registerForm = this.fb.group({
    name: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });

  onRegister() {
    if (this.registerForm.invalid) return;

    this.loading = true;
    this.error = '';

    const payload = {
      ...this.registerForm.value,
      headOfficeDistrict: 'Chennai' // Auto-attached as requested
    };
    delete (payload as any).confirmPassword;

    this.authService.registerOperator(payload).subscribe({
      next: () => {
        this.submitted = true;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.error?.message || 'Registration failed. Please try again.';
        this.loading = false;
      }
    });
  }
}
