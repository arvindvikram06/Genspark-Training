import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router, ActivatedRoute } from '@angular/router';
import { BusService } from '../../../core/services/bus.service';
import { BookingService } from '../../../core/services/booking.service';
import { ChangeDetectorRef } from '@angular/core';

interface BusResult {
  scheduleId: number;
  busName: string;
  source: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  duration: string;
  pricePerSeat: number;
  availableSeats: number;
}

interface SeatLayoutItem {
  row: number;
  col: number;
  seatNumber: string;
}

@Component({
  selector: 'app-bus-booking',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
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
          <h1 class="text-3xl font-bold text-white">Book Your Seat</h1>
          <div class="w-32"></div>
        </div>

        @if (loading) {
          <div class="flex flex-col items-center justify-center py-32">
            <div class="w-16 h-16 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-6"></div>
            <p class="text-slate-500 font-medium animate-pulse">Loading bus details...</p>
          </div>
        } @else if (selectedBus) {
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Bus Info -->
            <div class="lg:col-span-1">
              <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-6 mb-6">
                <h2 class="text-xl font-bold text-white mb-4">{{ selectedBus.busName }}</h2>
                <div class="space-y-3">
                  <div class="flex items-center gap-3 text-slate-300">
                    <mat-icon class="text-indigo-500">location_on</mat-icon>
                    <span>{{ selectedBus.source }} → {{ selectedBus.destination }}</span>
                  </div>
                  <div class="flex items-center gap-3 text-slate-300">
                    <span class="text-indigo-500 font-bold">Departure:</span>
                    <span>{{ selectedBus.departureTime | date:'medium' }}</span>
                  </div>
                  <div class="flex items-center gap-3 text-slate-300">
                    <span class="text-indigo-500 font-bold">Arrival:</span>
                    <span>{{ selectedBus.arrivalTime | date:'medium' }}</span>
                  </div>
                  <div class="flex items-center gap-3 text-slate-300">
                    <mat-icon class="text-indigo-500">event_seat</mat-icon>
                    <span>{{ selectedBus.availableSeats }} seats available</span>
                  </div>
                </div>
                <div class="mt-6 pt-4 border-t border-slate-800">
                  <div class="flex justify-between items-center">
                    <span class="text-slate-400">Fare per seat</span>
                    <span class="text-2xl font-bold text-white">₹{{ selectedBus.pricePerSeat }}</span>
                  </div>
                </div>
              </mat-card>

              <!-- Hold Timer -->
              @if (holdId) {
                <mat-card class="!bg-amber-500/10 !border-amber-500/30 !rounded-2xl p-4 mb-6">
                  <div class="flex items-center gap-3">
                    <mat-icon class="text-amber-500">timer</mat-icon>
                    <div>
                      <p class="text-amber-500 font-bold">Seat Hold Active</p>
                      <p class="text-amber-400 text-sm">{{ holdTimerLabel }}</p>
                    </div>
                  </div>
                </mat-card>
              }

              <!-- Selected Seats Summary -->
              @if (selectedSeats.length > 0) {
                <mat-card class="!bg-indigo-500/10 !border-indigo-500/30 !rounded-2xl p-4 mb-6">
                  <h3 class="text-indigo-400 font-bold mb-2">Selected Seats</h3>
                  <div class="flex flex-wrap gap-2 mb-3">
                    @for (seat of selectedSeats; track seat) {
                      <span class="px-3 py-1 bg-indigo-500/20 text-indigo-400 rounded-lg font-bold">{{ seat }}</span>
                    }
                  </div>
                  <div class="flex justify-between items-center pt-3 border-t border-indigo-500/20">
                    <span class="text-slate-400">Total Amount</span>
                    <span class="text-xl font-bold text-white">₹{{ selectedSeats.length * selectedBus.pricePerSeat }}</span>
                  </div>
                </mat-card>
              }

              <!-- Stage Indicator -->
              <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-4">
                <div class="flex justify-between items-center">
                  <div class="flex items-center gap-2" [class.text-indigo-400]="bookingStage === 'seats'" [class.text-slate-500]="bookingStage !== 'seats'">
                    <div class="w-8 h-8 rounded-full flex items-center justify-center" [class.bg-indigo-500]="bookingStage === 'seats'" [class.bg-slate-700]="bookingStage !== 'seats'">
                      <span class="text-white font-bold text-sm">1</span>
                    </div>
                    <span class="font-medium">Select Seats</span>
                  </div>
                  <div class="flex-1 h-1 mx-2" [class.bg-indigo-500]="bookingStage === 'passengers' || bookingStage === 'payment'" [class.bg-slate-700]="bookingStage === 'seats'"></div>
                  <div class="flex items-center gap-2" [class.text-indigo-400]="bookingStage === 'passengers'" [class.text-slate-500]="bookingStage !== 'passengers'">
                    <div class="w-8 h-8 rounded-full flex items-center justify-center" [class.bg-indigo-500]="bookingStage === 'passengers'" [class.bg-slate-700]="bookingStage !== 'passengers'">
                      <span class="text-white font-bold text-sm">2</span>
                    </div>
                    <span class="font-medium">Passengers</span>
                  </div>
                  <div class="flex-1 h-1 mx-2" [class.bg-indigo-500]="bookingStage === 'payment'" [class.bg-slate-700]="bookingStage !== 'payment'"></div>
                  <div class="flex items-center gap-2" [class.text-indigo-400]="bookingStage === 'payment'" [class.text-slate-500]="bookingStage !== 'payment'">
                    <div class="w-8 h-8 rounded-full flex items-center justify-center" [class.bg-indigo-500]="bookingStage === 'payment'" [class.bg-slate-700]="bookingStage !== 'payment'">
                      <span class="text-white font-bold text-sm">3</span>
                    </div>
                    <span class="font-medium">Payment</span>
                  </div>
                </div>
              </mat-card>
            </div>

            <!-- Main Content Area -->
            <div class="lg:col-span-2">
              <!-- Seat Selection Stage -->
              @if (bookingStage === 'seats') {
                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-6">
                  <h2 class="text-xl font-bold text-white mb-6">Select Your Seats</h2>
                  
                  <div class="mb-4 flex gap-4 text-sm">
                    <div class="flex items-center gap-2">
                      <div class="w-6 h-6 bg-indigo-500/20 border-2 border-indigo-500/40 rounded"></div>
                      <span class="text-slate-400">Available</span>
                    </div>
                    <div class="flex items-center gap-2">
                      <div class="w-6 h-6 bg-indigo-500 rounded"></div>
                      <span class="text-slate-400">Selected</span>
                    </div>
                    <div class="flex items-center gap-2">
                      <div class="w-6 h-6 bg-slate-700 rounded"></div>
                      <span class="text-slate-400">Booked</span>
                    </div>
                  </div>

                  <div class="bg-slate-950 rounded-xl p-6 min-h-[400px]">
                    <div class="flex justify-between items-center mb-4">
                      <span class="text-slate-400 text-sm">Driver</span>
                      <div class="w-12 h-12 bg-slate-800 rounded-lg flex items-center justify-center">
                        <mat-icon class="text-slate-500">drive_eta</mat-icon>
                      </div>
                    </div>

                    <div class="space-y-2">
                      @for (row of seatRows; track row.rowNumber) {
                        <div class="flex items-center justify-center gap-2">
                          <!-- Left side seats -->
                          @for (seat of row.left; track seat.seatNumber) {
                            <button class="w-12 h-12 rounded-lg flex items-center justify-center font-bold text-sm transition-all"
                              [class.bg-indigo-500/20]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.border-2]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.border-indigo-500/40]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.text-indigo-400]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.bg-indigo-500]="isSeatSelected(seat.seatNumber)"
                              [class.text-white]="isSeatSelected(seat.seatNumber)"
                              [class.bg-slate-700]="getSeatStatus(seat.seatNumber) !== 0"
                              [class.text-slate-500]="getSeatStatus(seat.seatNumber) !== 0"
                              [class.cursor-not-allowed]="getSeatStatus(seat.seatNumber) !== 0"
                              [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                              (click)="toggleSeat(seat.seatNumber)">
                              {{ seat.seatNumber }}
                            </button>
                          }

                          <!-- Aisle -->
                          <div class="w-16 h-12 bg-slate-800/50 rounded-lg flex items-center justify-center">
                            <div class="w-1 h-8 bg-slate-700 rounded"></div>
                          </div>

                          <!-- Right side seats -->
                          @for (seat of row.right; track seat.seatNumber) {
                            <button class="w-12 h-12 rounded-lg flex items-center justify-center font-bold text-sm transition-all"
                              [class.bg-indigo-500/20]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.border-2]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.border-indigo-500/40]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.text-indigo-400]="getSeatStatus(seat.seatNumber) === 0 && !isSeatSelected(seat.seatNumber)"
                              [class.bg-indigo-500]="isSeatSelected(seat.seatNumber)"
                              [class.text-white]="isSeatSelected(seat.seatNumber)"
                              [class.bg-slate-700]="getSeatStatus(seat.seatNumber) !== 0"
                              [class.text-slate-500]="getSeatStatus(seat.seatNumber) !== 0"
                              [class.cursor-not-allowed]="getSeatStatus(seat.seatNumber) !== 0"
                              [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                              (click)="toggleSeat(seat.seatNumber)">
                              {{ seat.seatNumber }}
                            </button>
                          }
                        </div>
                      }
                    </div>
                  </div>

                  <div class="mt-6 flex justify-end">
                    <button mat-flat-button 
                      class="!bg-indigo-600 !text-white !px-8 !py-3 !rounded-xl"
                      (click)="proceedWithSeatHold()" 
                      [disabled]="selectedSeats.length === 0 || loading">
                      Proceed to Passenger Details
                    </button>
                  </div>
                </mat-card>
              }

              <!-- Passenger Details Stage -->
              @if (bookingStage === 'passengers') {
                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-6">
                  <h2 class="text-xl font-bold text-white mb-6">Passenger Details</h2>
                  
                  <div class="space-y-4">
                    @for (seat of selectedSeats; track seat) {
                      <div class="bg-slate-950 rounded-xl p-4">
                        <h3 class="text-indigo-400 font-bold mb-4">Seat {{ seat }}</h3>
                        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                          <mat-form-field appearance="outline" class="custom-dark-field">
                            <mat-label>Name</mat-label>
                            <input matInput [(ngModel)]="passengerBySeat[seat].name" required>
                          </mat-form-field>
                          <mat-form-field appearance="outline" class="custom-dark-field">
                            <mat-label>Age</mat-label>
                            <input matInput type="number" [(ngModel)]="passengerBySeat[seat].age" min="1" max="100" required>
                          </mat-form-field>
                          <mat-form-field appearance="outline" class="custom-dark-field">
                            <mat-label>Gender</mat-label>
                            <mat-select [(ngModel)]="passengerBySeat[seat].gender" required>
                              <mat-option value="Male">Male</mat-option>
                              <mat-option value="Female">Female</mat-option>
                              <mat-option value="Other">Other</mat-option>
                            </mat-select>
                          </mat-form-field>
                        </div>
                      </div>
                    }
                  </div>

                  <div class="mt-6 flex justify-between">
                    <button mat-button (click)="bookingStage = 'seats'" class="!text-slate-400 hover:!text-white">
                      Back
                    </button>
                    <button mat-flat-button 
                      class="!bg-indigo-600 !text-white !px-8 !py-3 !rounded-xl"
                      (click)="confirmBooking()" 
                      [disabled]="loading">
                      Confirm Booking
                    </button>
                  </div>
                </mat-card>
              }

              <!-- Payment Stage -->
              @if (bookingStage === 'payment') {
                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-6">
                  <h2 class="text-xl font-bold text-white mb-6">Payment</h2>
                  
                  <div class="bg-slate-950 rounded-xl p-6 mb-6">
                    <div class="flex justify-between items-center mb-4">
                      <span class="text-slate-400">Total Amount</span>
                      <span class="text-3xl font-bold text-white">₹{{ selectedSeats.length * selectedBus.pricePerSeat + selectedSeats.length * 25 }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm text-slate-400 mb-2">
                      <span>{{ selectedSeats.length }} seat(s) × ₹{{ selectedBus.pricePerSeat }}</span>
                      <span>₹{{ selectedSeats.length * selectedBus.pricePerSeat }}</span>
                    </div>
                    <div class="flex justify-between items-center text-sm text-slate-400">
                      <span>Convenience Fee ({{ selectedSeats.length }} seat(s) × ₹25)</span>
                      <span>₹{{ selectedSeats.length * 25 }}</span>
                    </div>
                  </div>

                  @if (paymentId) {
                    <div class="bg-slate-950 rounded-xl p-6 mb-6">
                      <div class="text-center">
                        <div class="w-16 h-16 mx-auto mb-4 rounded-full bg-purple-500/20 flex items-center justify-center">
                          <mat-icon class="text-purple-500 text-3xl">payment</mat-icon>
                        </div>
                        <h3 class="text-white font-bold mb-2">UPI Payment</h3>
                        <p class="text-slate-400 text-sm mb-4">Complete payment using your UPI app</p>
                        <div class="bg-slate-800 rounded-lg p-4 mb-4">
                          <p class="text-slate-400 text-xs mb-1">Merchant</p>
                          <p class="text-white font-bold">Bus Booking System</p>
                          <div class="mt-3 pt-3 border-t border-slate-700">
                            <div class="flex justify-between text-sm mb-2">
                              <span class="text-slate-400">{{ selectedSeats.length }} seat(s) × ₹{{ selectedBus.pricePerSeat }}</span>
                              <span class="text-white">₹{{ selectedSeats.length * selectedBus.pricePerSeat }}</span>
                            </div>
                            <div class="flex justify-between text-sm mb-2">
                              <span class="text-slate-400">Convenience Fee</span>
                              <span class="text-white">₹{{ selectedSeats.length * 25 }}</span>
                            </div>
                            <div class="flex justify-between text-sm pt-2 border-t border-slate-700">
                              <span class="text-slate-300 font-bold">Total</span>
                              <span class="text-emerald-400 font-bold text-xl">₹{{ selectedSeats.length * selectedBus.pricePerSeat + selectedSeats.length * 25 }}</span>
                            </div>
                          </div>
                        </div>
                        <div class="flex justify-center gap-4">
                          <button mat-flat-button 
                            class="!bg-emerald-600 !text-white !px-8 !py-3 !rounded-xl"
                            (click)="payNow()" 
                            [disabled]="loading">
                            Pay Now
                          </button>
                          <button mat-button 
                            (click)="cancelCurrentFlow()" 
                            class="!text-slate-400 hover:!text-white"
                            [disabled]="loading">
                            Cancel
                          </button>
                        </div>
                      </div>
                    </div>
                  } @else {
                    <div class="flex justify-end">
                      <button mat-flat-button 
                        class="!bg-indigo-600 !text-white !px-8 !py-3 !rounded-xl"
                        (click)="initiatePayment()" 
                        [disabled]="loading">
                        Proceed to Payment
                      </button>
                    </div>
                  }
                </mat-card>
              }
            </div>
          </div>
        }
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
export class BusBookingComponent implements OnInit {
  fb = inject(FormBuilder);
  busService = inject(BusService);
  bookingService = inject(BookingService);
  snack = inject(MatSnackBar);
  router = inject(Router);
  route = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);

  loading = false;
  selectedBus: BusResult | null = null;
  bookingStage: 'seats' | 'passengers' | 'payment' = 'seats';
  message = '';
  errorMessage = '';

  seatStatusMap: Record<string, number> = {};
  seatRows: Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> = [];
  selectedSeats: string[] = [];
  passengerBySeat: Record<string, { name: string; age: number; gender: string }> = {};

  holdId: number | null = null;
  holdExpiresAt: string | null = null;
  holdCountdownSeconds = 0;
  private holdTimer: ReturnType<typeof setInterval> | null = null;

  paymentId: number | null = null;
  paymentExpiresAt: string | null = null;
  paymentCountdownSeconds = 0;
  private paymentTimer: ReturnType<typeof setInterval> | null = null;
  paymentIdempotencyKey: string = '';

  ngOnInit() {
    const scheduleId = this.route.snapshot.paramMap.get('id');
    if (scheduleId) {
      this.loadBusDetails(Number(scheduleId));
    } else {
      this.router.navigate(['/user/dashboard']);
    }
  }

  loadBusDetails(scheduleId: number) {
    this.loading = true;
    this.busService.getAllUpcomingBuses().subscribe({
      next: (res: any) => {
        const buses = this.extractList(res) || [];
        this.selectedBus = buses.find((b: BusResult) => b.scheduleId === scheduleId) || null;
        if (this.selectedBus) {
          this.loadSeatMap(scheduleId);
        } else {
          this.snack.open('Bus not found', 'Error', { duration: 3000 });
          this.router.navigate(['/user/dashboard']);
        }
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.snack.open('Failed to load bus details', 'Error', { duration: 3000 });
        this.router.navigate(['/user/dashboard']);
        this.loading = false;
      }
    });
  }

  loadSeatMap(scheduleId: number) {
    this.loading = true;
    this.busService.getSeatMap(scheduleId).subscribe({
      next: (res: any) => {
        const data = this.extractData(res);
        const seatLayout: SeatLayoutItem[] = data?.seatLayout?.seats || data?.seatLayout?.Seats || [];
        const statuses: Array<{ seatNumber: string; status: any }> = data?.seats || [];
        this.seatStatusMap = statuses.reduce((acc, s) => {
          acc[s.seatNumber] = this.normalizeSeatStatus(s.status);
          return acc;
        }, {} as Record<string, number>);
        this.seatRows = this.buildSeatRows(seatLayout, statuses);
        this.loading = false;
        this.checkForActiveHold(scheduleId);
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.snack.open('Failed to load seat layout', 'Error', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  checkForActiveHold(scheduleId: number) {
    this.bookingService.getActiveSeatHold(scheduleId).subscribe({
      next: (res: any) => {
        if (res.data && res.data.holdId) {
          this.holdId = res.data.holdId;
          this.holdExpiresAt = res.data.expiresAt;
          this.selectedSeats = res.data.seatNumbers || [];
          this.selectedSeats.forEach(seat => {
            this.passengerBySeat[seat] = { name: '', age: 0, gender: 'Male' };
          });
          if (this.holdExpiresAt) {
            this.startHoldCountdown(this.holdExpiresAt);
          }
          this.bookingStage = 'passengers';
          this.message = 'Your previously selected seats have been restored.';
          this.cdr.detectChanges();
        }
      },
      error: () => {}
    });
  }

  toggleSeat(seatNumber: string) {
    const index = this.selectedSeats.indexOf(seatNumber);
    if (index > -1) {
      this.selectedSeats.splice(index, 1);
      delete this.passengerBySeat[seatNumber];
    } else {
      this.selectedSeats.push(seatNumber);
      this.passengerBySeat[seatNumber] = { name: '', age: 0, gender: 'Male' };
    }
    this.cdr.detectChanges();
  }

  isSeatSelected(seatNumber: string): boolean {
    return this.selectedSeats.includes(seatNumber);
  }

  getSeatStatus(seatNumber: string): number {
    return this.seatStatusMap[seatNumber] || 0;
  }

  proceedWithSeatHold() {
    if (!this.selectedBus || this.selectedSeats.length === 0) return;
    this.loading = true;
    this.bookingService.holdSeats(this.selectedBus.scheduleId, this.selectedSeats).subscribe({
      next: (res: any) => {
        this.holdId = res.data?.holdId;
        this.holdExpiresAt = res.data?.expiresAt;
        if (this.holdExpiresAt) {
          this.startHoldCountdown(this.holdExpiresAt);
        }
        this.bookingStage = 'passengers';
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.snack.open(err?.error?.message || 'Failed to hold seats', 'Error', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  confirmBooking() {
    if (!this.holdId) {
      this.snack.open('No active seat hold', 'Error', { duration: 3000 });
      return;
    }

    const passengers = this.selectedSeats.map(seat => ({
      seatNumber: seat,
      name: this.passengerBySeat[seat].name,
      age: this.passengerBySeat[seat].age,
      gender: this.passengerBySeat[seat].gender
    }));

    if (passengers.some(p => !p.name || !p.age)) {
      this.snack.open('Please fill all passenger details', 'Error', { duration: 3000 });
      return;
    }

    this.loading = true;
    this.bookingService.confirmBooking(this.holdId, passengers).subscribe({
      next: (res: any) => {
        this.holdId = null;
        this.holdExpiresAt = null;
        this.stopHoldCountdown();
        this.paymentId = res.data?.paymentId;
        this.paymentIdempotencyKey = this.generateIdempotencyKey();
        this.paymentExpiresAt = new Date(Date.now() + 10 * 60 * 1000).toISOString();
        if (this.paymentExpiresAt) {
          this.startPaymentCountdown(this.paymentExpiresAt);
        }
        this.bookingStage = 'payment';
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.snack.open(err?.error?.message || 'Failed to confirm booking', 'Error', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  initiatePayment() {
    this.bookingStage = 'payment';
    this.cdr.detectChanges();
  }

  payNow() {
    if (!this.paymentId) return;
    this.loading = true;
    this.bookingService.pay(this.paymentId, this.paymentIdempotencyKey).subscribe({
      next: (res: any) => {
        this.stopPaymentCountdown();
        this.snack.open('Payment successful. Booking confirmed.', 'Success', { duration: 3000 });
        this.router.navigate(['/user/dashboard']);
      },
      error: (err: any) => {
        this.snack.open(err?.error?.message || 'Payment failed', 'Error', { duration: 3000 });
        this.loading = false;
      }
    });
  }

  cancelCurrentFlow() {
    if (this.loading) return;

    if (this.holdId) {
      this.loading = true;
      this.bookingService.releaseHold(this.holdId).subscribe({
        next: () => {
          this.resetBookingFlow();
          this.router.navigate(['/user/dashboard']);
        },
        error: () => {
          this.resetBookingFlow();
          this.router.navigate(['/user/dashboard']);
        }
      });
      return;
    }

    if (this.paymentId) {
      this.loading = true;
      this.bookingService.abortPayment(this.paymentId).subscribe({
        next: () => {
          this.resetBookingFlow();
          this.router.navigate(['/user/dashboard']);
        },
        error: () => {
          this.resetBookingFlow();
          this.router.navigate(['/user/dashboard']);
        }
      });
      return;
    }

    this.router.navigate(['/user/dashboard']);
  }

  resetBookingFlow() {
    this.holdId = null;
    this.holdExpiresAt = null;
    this.stopHoldCountdown();
    this.paymentId = null;
    this.paymentExpiresAt = null;
    this.stopPaymentCountdown();
    this.selectedSeats = [];
    this.passengerBySeat = {};
    this.bookingStage = 'seats';
    this.loading = false;
  }

  startHoldCountdown(expiresAt: string) {
    this.stopHoldCountdown();
    if (!expiresAt) return;
    const expiry = new Date(expiresAt).getTime();
    this.holdTimer = setInterval(() => {
      const now = Date.now();
      const remaining = Math.max(0, Math.floor((expiry - now) / 1000));
      this.holdCountdownSeconds = remaining;
      if (remaining <= 0) {
        this.stopHoldCountdown();
        this.cancelCurrentFlow();
      }
      this.cdr.detectChanges();
    }, 1000);
  }

  stopHoldCountdown() {
    if (this.holdTimer) {
      clearInterval(this.holdTimer);
      this.holdTimer = null;
    }
  }

  startPaymentCountdown(expiresAt: string) {
    this.stopPaymentCountdown();
    if (!expiresAt) return;
    const expiry = new Date(expiresAt).getTime();
    this.paymentTimer = setInterval(() => {
      const now = Date.now();
      const remaining = Math.max(0, Math.floor((expiry - now) / 1000));
      this.paymentCountdownSeconds = remaining;
      if (remaining <= 0) {
        this.stopPaymentCountdown();
        this.cancelCurrentFlow();
      }
      this.cdr.detectChanges();
    }, 1000);
  }

  stopPaymentCountdown() {
    if (this.paymentTimer) {
      clearInterval(this.paymentTimer);
      this.paymentTimer = null;
    }
  }

  get holdTimerLabel(): string {
    const minutes = Math.floor(this.holdCountdownSeconds / 60);
    const seconds = this.holdCountdownSeconds % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }

  generateIdempotencyKey(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  }

  normalizeSeatStatus(status: any): number {
    if (typeof status === 'number') return status;
    if (typeof status === 'string') {
      const s = status.toLowerCase();
      if (s === 'available' || s === '0') return 0;
      if (s === 'booked' || s === '1') return 1;
      if (s === 'held' || s === '2') return 2;
    }
    return 0;
  }

  buildSeatRows(seatLayout: SeatLayoutItem[], statuses: any[]): Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> {
    const rowsMap = new Map<number, { left: SeatLayoutItem[]; right: SeatLayoutItem[] }>();
    
    seatLayout.forEach(seat => {
      if (!rowsMap.has(seat.row)) {
        rowsMap.set(seat.row, { left: [], right: [] });
      }
      const row = rowsMap.get(seat.row)!;
      if (seat.col <= 2) {
        row.left.push(seat);
      } else {
        row.right.push(seat);
      }
    });

    const sortedRows = Array.from(rowsMap.entries())
      .map(([rowNumber, seats]) => ({
        rowNumber,
        left: seats.left.sort((a, b) => a.col - b.col),
        right: seats.right.sort((a, b) => a.col - b.col)
      }))
      .sort((a, b) => a.rowNumber - b.rowNumber);

    return sortedRows;
  }

  extractList(res: any): BusResult[] {
    if (Array.isArray(res)) return res;
    if (res?.data) {
      if (Array.isArray(res.data)) return res.data;
      if (res.data.items) return res.data.items;
    }
    return [];
  }

  extractData(res: any): any {
    if (res?.data) return res.data;
    return res;
  }

  goBack() {
    this.cancelCurrentFlow();
  }
}
