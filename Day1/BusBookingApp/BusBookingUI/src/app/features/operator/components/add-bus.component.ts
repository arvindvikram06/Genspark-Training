import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { OperatorService } from '../../../core/services/operator.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-add-bus',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  template: `
    <div class="min-h-screen bg-slate-950 p-8">
      <div class="max-w-7xl mx-auto">
        <!-- Header -->
        <div class="flex items-center justify-between mb-8">
          <button mat-button (click)="goBack()" class="!text-slate-400 hover:!text-white flex items-center gap-2">
            <mat-icon>arrow_back</mat-icon>
            Back to Dashboard
          </button>
          <h1 class="text-3xl font-bold text-white">Register New Vehicle</h1>
          <div class="w-32"></div>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <!-- Form Section -->
          <div class="space-y-6">
            <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-8">
              <h2 class="text-xl font-bold text-white mb-6">Vehicle Information</h2>
              
              <form [formGroup]="busForm" class="space-y-6">
                <div>
                  <mat-form-field appearance="outline" class="w-full custom-dark-field">
                    <mat-label>Vehicle Name</mat-label>
                    <input matInput formControlName="name" placeholder="e.g. Express Line 1" required>
                    <mat-error *ngIf="busForm.get('name')?.hasError('required')">Vehicle name is required</mat-error>
                  </mat-form-field>
                </div>

                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <mat-form-field appearance="outline" class="w-full custom-dark-field">
                      <mat-label>Total Seats</mat-label>
                      <input matInput type="number" formControlName="totalSeats" min="10" max="60" required>
                      <mat-error *ngIf="busForm.get('totalSeats')?.hasError('required')">Total seats is required</mat-error>
                      <mat-error *ngIf="busForm.get('totalSeats')?.hasError('min')">Minimum 10 seats</mat-error>
                      <mat-error *ngIf="busForm.get('totalSeats')?.hasError('max')">Maximum 60 seats</mat-error>
                    </mat-form-field>
                  </div>
                  <div>
                    <mat-form-field appearance="outline" class="w-full custom-dark-field">
                      <mat-label>Seating Layout</mat-label>
                      <mat-select formControlName="layoutType" required>
                        <mat-option value="2x2">2x2 (Standard)</mat-option>
                        <mat-option value="2x1">2x1 (Sleeper)</mat-option>
                      </mat-select>
                    </mat-form-field>
                  </div>
                </div>

                <div class="pt-6">
                  <button type="submit" mat-flat-button 
                    class="w-full !bg-indigo-600 !text-white !h-16 !rounded-2xl !text-sm !font-black !shadow-2xl !shadow-indigo-500/20 hover:!bg-indigo-500 transition-all uppercase tracking-widest" 
                    (click)="saveBus()" 
                    [disabled]="busForm.invalid || loading">
                    {{ loading ? 'Synchronizing...' : 'Authorize Vehicle' }}
                  </button>
                </div>
              </form>
            </mat-card>

            <!-- Instructions -->
            <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-6">
              <h3 class="text-lg font-bold text-white mb-4 flex items-center gap-2">
                <mat-icon class="text-indigo-500">info</mat-icon>
                Registration Guidelines
              </h3>
              <ul class="space-y-2 text-slate-400 text-sm">
                <li class="flex items-start gap-2">
                  <mat-icon class="text-emerald-500 text-sm mt-0.5">check_circle</mat-icon>
                  Vehicle name should be unique and descriptive
                </li>
                <li class="flex items-start gap-2">
                  <mat-icon class="text-emerald-500 text-sm mt-0.5">check_circle</mat-icon>
                  Total seats must be between 10 and 60
                </li>
                <li class="flex items-start gap-2">
                  <mat-icon class="text-emerald-500 text-sm mt-0.5">check_circle</mat-icon>
                  Choose seating layout based on vehicle type
                </li>
                <li class="flex items-start gap-2">
                  <mat-icon class="text-emerald-500 text-sm mt-0.5">check_circle</mat-icon>
                  Vehicle will be reviewed by admin before approval
                </li>
              </ul>
            </mat-card>
          </div>

          <!-- Preview Section -->
          <div class="space-y-6">
            <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-8">
              <h2 class="text-xl font-bold text-white mb-6">Seat Layout Preview</h2>
              
              <div class="bg-slate-950 rounded-xl p-6 min-h-[400px]">
                <div class="flex justify-between items-center mb-4">
                  <span class="text-slate-400 text-sm">Driver</span>
                  <div class="w-12 h-12 bg-slate-800 rounded-lg flex items-center justify-center">
                    <mat-icon class="text-slate-500">drive_eta</mat-icon>
                  </div>
                </div>

                <div class="space-y-2">
                  @for (row of previewRows; track row.id) {
                    <div class="flex items-center justify-center gap-2">
                      <!-- Left side seats -->
                      @for (seat of row.leftSeats; track seat) {
                        @if (seat) {
                          <div class="w-12 h-12 bg-indigo-500/20 border-2 border-indigo-500/40 rounded-lg flex items-center justify-center text-indigo-400 font-bold text-sm">
                            {{seat}}
                          </div>
                        }
                      }

                      <!-- Aisle (walk path) -->
                      <div class="w-16 h-12 bg-slate-800/50 rounded-lg flex items-center justify-center">
                        <div class="w-1 h-8 bg-slate-700 rounded"></div>
                      </div>

                      <!-- Right side seats -->
                      @for (seat of row.rightSeats; track seat) {
                        @if (seat) {
                          <div class="w-12 h-12 bg-indigo-500/20 border-2 border-indigo-500/40 rounded-lg flex items-center justify-center text-indigo-400 font-bold text-sm">
                            {{seat}}
                          </div>
                        }
                      }
                    </div>
                  }
                </div>

                <div class="mt-6 pt-4 border-t border-slate-800">
                  <div class="flex justify-between text-sm">
                    <span class="text-slate-400">Total Seats:</span>
                    <span class="text-white font-bold">{{busForm.value.totalSeats}}</span>
                  </div>
                  <div class="flex justify-between text-sm mt-2">
                    <span class="text-slate-400">Layout:</span>
                    <span class="text-white font-bold">{{busForm.value.layoutType}}</span>
                  </div>
                </div>
              </div>
            </mat-card>
          </div>
        </div>
      </div>
    </div>

    <style>
      ::ng-deep .custom-dark-field .mat-mdc-text-field-wrapper {
        background-color: #1e293b !important;
        border: 1px solid #334155 !important;
        border-radius: 8px !important;
      }

      ::ng-deep .custom-dark-field .mat-mdc-form-field-label {
        color: #94a3b8 !important;
      }

      ::ng-deep .custom-dark-field .mat-mdc-input-element {
        color: #e2e8f0 !important;
      }

      ::ng-deep .custom-dark-field .mat-mdc-select-value {
        color: #e2e8f0 !important;
      }

      ::ng-deep .mat-mdc-select-panel {
        background-color: #1e293b !important;
        border: 1px solid #334155 !important;
      }

      ::ng-deep .mat-mdc-option {
        color: #e2e8f0 !important;
      }

      ::ng-deep .mat-mdc-option.mdc-list-item--selected {
        background-color: #4f46e5 !important;
      }
    </style>
  `
})
export class AddBusComponent implements OnInit {
  fb = inject(FormBuilder);
  operatorService = inject(OperatorService);
  snack = inject(MatSnackBar);
  router = inject(Router);
  cdr = inject(ChangeDetectorRef);

  busForm: FormGroup;
  loading = false;
  previewRows: any[] = [];

  constructor() {
    this.busForm = this.fb.group({
      name: ['', Validators.required],
      totalSeats: [40, [Validators.required, Validators.min(10), Validators.max(60)]],
      layoutType: ['2x2', Validators.required]
    });
  }

  ngOnInit() {
    this.generatePreview();
    this.cdr.detectChanges();

    this.busForm.valueChanges.subscribe(() => {
      this.generatePreview();
      this.cdr.detectChanges();
    });
  }

  generatePreview() {
    const total = this.busForm.value.totalSeats;
    const layout = this.busForm.value.layoutType;
    const seatsPerRow = layout === '2x2' ? 4 : 3;
    const rowCount = Math.ceil(total / seatsPerRow);

    this.previewRows = [];
    const labels = ['A', 'B', 'C', 'D', 'E'];
    let seatCount = 0;

    for (let i = 1; i <= rowCount; i++) {
      const rowSeats = [];
      const leftSeats = [];
      const rightSeats = [];

      // Split seats into left and right sides with aisle in between
      const seatsPerSide = Math.ceil(seatsPerRow / 2);

      for (let j = 0; j < seatsPerRow; j++) {
        if (seatCount < total) {
          if (j < seatsPerSide) {
            leftSeats.push(`${i}${labels[j]}`);
          } else {
            rightSeats.push(`${i}${labels[j]}`);
          }
          seatCount++;
        }
      }

      this.previewRows.push({ id: i, leftSeats, rightSeats });
    }
  }

  saveBus() {
    if (this.busForm.invalid) return;
    this.loading = true;

    const layout = this.busForm.value.layoutType;
    const total = this.busForm.value.totalSeats;
    const seatsPerRow = layout === '2x2' ? 4 : 3;
    const rowCount = Math.ceil(total / seatsPerRow);
    const labels = ['A', 'B', 'C', 'D', 'E'];

    const seatsArray = [];
    for (let i = 1; i <= rowCount; i++) {
      for (let j = 0; j < seatsPerRow; j++) {
        if (seatsArray.length < total) {
          seatsArray.push({
            row: i,
            col: j + 1,
            seatNumber: `${i}${labels[j]}`
          });
        }
      }
    }

    const payload = {
      name: this.busForm.value.name,
      totalSeats: total,
      seatLayout: {
        rows: rowCount,
        cols: layout === '2x2' ? 2 : 2,
        seats: seatsArray
      }
    };

    this.operatorService.createBus(payload).subscribe({
      next: () => {
        this.snack.open('Vehicle Registered for Review', 'OK', { duration: 3000 });
        this.router.navigate(['/operator/dashboard']);
      },
      error: (err: any) => {
        const msg = err.error?.message || err.error || 'Failed to register vehicle';
        this.snack.open(msg, 'Error', { duration: 5000 });
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  goBack() {
    this.router.navigate(['/operator/dashboard']);
  }
}
