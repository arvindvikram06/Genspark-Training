import { Component, Inject } from '@angular/core';
import { MatDialogRef, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-passenger-details-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <div class="p-6">
      <h2 class="text-xl font-bold text-white mb-4">Seat Details</h2>
      
      @if (data.seat) {
        <div class="space-y-3">
          <div class="flex justify-between">
            <span class="text-slate-400">Seat Number:</span>
            <span class="text-white font-semibold">{{ data.seatNumber }}</span>
          </div>
          
          <div class="flex justify-between">
            <span class="text-slate-400">Status:</span>
            <span [class]="data.seat.status === 1 || data.seat.status === 2 ? 'text-amber-400' : 'text-emerald-400'" class="font-semibold">
              {{ data.seat.status === 1 || data.seat.status === 2 ? 'Booked' : 'Available' }}
            </span>
          </div>
          
          @if (data.seat.passengerName) {
            <div class="border-t border-slate-700 pt-3 mt-3">
              <h3 class="text-lg font-semibold text-white mb-3">Passenger Information</h3>
              <div class="space-y-2">
                <div class="flex justify-between">
                  <span class="text-slate-400">Name:</span>
                  <span class="text-white">{{ data.seat.passengerName }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-slate-400">Age:</span>
                  <span class="text-white">{{ data.seat.passengerAge }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-slate-400">Gender:</span>
                  <span class="text-white">{{ data.seat.passengerGender }}</span>
                </div>
              </div>
            </div>
          } @else {
            <p class="text-slate-400 mt-3">No passenger information available for this seat.</p>
          }
        </div>
      } @else {
        <p class="text-slate-400">No seat data available.</p>
      }
      
      <div class="mt-6 flex justify-end">
        <button mat-button (click)="closeDialog()" class="!text-white hover:!bg-slate-700">Close</button>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      background: #1e293b;
      color: #e2e8f0;
    }
  `]
})
export class PassengerDetailsDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<PassengerDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  closeDialog() {
    this.dialogRef.close();
  }
}
