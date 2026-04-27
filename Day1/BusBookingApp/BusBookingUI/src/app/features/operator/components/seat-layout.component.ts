import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { OperatorService } from '../../../core/services/operator.service';
import { ChangeDetectorRef } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PassengerDetailsDialogComponent } from './passenger-details-dialog.component';

interface SeatLayoutItem {
  row: number;
  col: number;
  seatNumber: string;
}

@Component({
  selector: 'app-seat-layout',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="min-h-screen bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900">
      <div class="max-w-7xl mx-auto px-4 py-8">
        <div class="flex items-center justify-between mb-8">
          <button (click)="goBack()" class="flex items-center gap-2 text-slate-400 hover:text-white transition-colors">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
            <span class="font-semibold">Back to Dashboard</span>
          </button>
          <h1 class="text-3xl font-bold text-white">Seat Layout</h1>
          <div></div>
        </div>

        @if (loading) {
          <div class="flex flex-col items-center justify-center py-20">
            <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
            <p class="text-slate-400">Loading seat layout...</p>
          </div>
        } @else {
          <div class="bg-slate-800/50 backdrop-blur-sm rounded-2xl p-6 border border-slate-700">
            <div class="flex justify-between items-center mb-6">
              <div>
                <h2 class="text-2xl font-bold text-white">{{ schedule?.busName }}</h2>
                <p class="text-slate-400">{{ schedule?.source }} → {{ schedule?.destination }}</p>
              </div>
              <div class="text-right">
                <p class="text-lg text-slate-300">{{ schedule?.totalSeats }} total seats</p>
                <p class="text-emerald-400">{{ availableSeatsCount }} available</p>
              </div>
            </div>

            <div class="flex justify-center gap-8 mb-6 text-sm">
              <div class="flex items-center gap-2">
                <div class="w-6 h-6 bg-slate-700 rounded"></div>
                <span class="text-slate-400">Available</span>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-6 h-6 bg-slate-600 rounded"></div>
                <span class="text-slate-400">Booked</span>
              </div>
            </div>

            <div class="flex flex-col items-center gap-2">
              @for (row of seatRows; track row.rowNumber) {
                <div class="flex items-center gap-4">
                  <div class="flex gap-2">
                    @for (seat of row.left; track seat.seatNumber) {
                      <button class="w-12 h-12 rounded text-sm font-semibold transition-colors"
                        [class]="getSeatStatus(seat.seatNumber) === 'booked' ? 'bg-slate-600 text-slate-400 hover:bg-slate-500' : 'bg-slate-700 text-white hover:bg-slate-600'"
                        (click)="showSeatDetails(seat.seatNumber)">
                        {{ seat.seatNumber }}
                      </button>
                    }
                  </div>
                  <div class="w-10"></div>
                  <div class="flex gap-2">
                    @for (seat of row.right; track seat.seatNumber) {
                      <button class="w-12 h-12 rounded text-sm font-semibold transition-colors"
                        [class]="getSeatStatus(seat.seatNumber) === 'booked' ? 'bg-slate-600 text-slate-400 hover:bg-slate-500' : 'bg-slate-700 text-white hover:bg-slate-600'"
                        (click)="showSeatDetails(seat.seatNumber)">
                        {{ seat.seatNumber }}
                      </button>
                    }
                  </div>
                </div>
              }
            </div>

            <div class="mt-6 text-center">
              <p class="text-slate-400 text-sm">Click on booked seats to view passenger details</p>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class SeatLayoutComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private operatorService = inject(OperatorService);
  private cdr = inject(ChangeDetectorRef);
  private dialog = inject(MatDialog);

  scheduleId: number | null = null;
  schedule: any = null;
  seatStatusMap: Record<string, any> = {};
  seatRows: Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> = [];
  loading = false;

  ngOnInit() {
    this.scheduleId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.scheduleId) {
      this.loadScheduleSeats();
    }
  }

  loadScheduleSeats() {
    this.loading = true;
    this.operatorService.getScheduleSeats(this.scheduleId!).subscribe({
      next: (res: any) => {
        console.log('Seat map response:', res);
        this.schedule = res.data;
        const seats = res.data.seats || [];

        this.seatStatusMap = {};
        seats.forEach((seat: any) => {
          this.seatStatusMap[seat.seatNumber] = seat;
        });
        console.log('Seat status map:', this.seatStatusMap);
        this.buildSeatLayout();
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading seat layout:', err);
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  buildSeatLayout() {
    const rows = Math.ceil(this.schedule.totalSeats / 4);
    const newRows: Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> = [];

    for (let r = 1; r <= rows; r++) {
      const left: SeatLayoutItem[] = [];
      const right: SeatLayoutItem[] = [];

      for (let c = 1; c <= 2; c++) {
        const seatNum = `${r}${String.fromCharCode(64 + c)}`;
        left.push({ row: r, col: c, seatNumber: seatNum });
      }

      for (let c = 3; c <= 4; c++) {
        const seatNum = `${r}${String.fromCharCode(64 + c)}`;
        right.push({ row: r, col: c, seatNumber: seatNum });
      }

      newRows.push({ rowNumber: r, left, right });
    }

    this.seatRows = newRows;
  }

  getSeatStatus(seatNumber: string): string {
    const seat = this.seatStatusMap[seatNumber];
    if (!seat) return 'available';
    if (seat.status === 1) return 'booked';
    if (seat.status === 2) return 'booked';
    return 'available';
  }

  get availableSeatsCount(): number {
    return Object.values(this.seatStatusMap).filter((seat: any) => seat.status === 0).length;
  }

  showSeatDetails(seatNumber: string) {
    const seat = this.seatStatusMap[seatNumber];
    console.log('Seat details for', seatNumber, ':', seat);
    
    this.dialog.open(PassengerDetailsDialogComponent, {
      width: '400px',
      data: {
        seatNumber,
        seat
      },
      panelClass: 'dark-dialog'
    });
  }

  goBack() {
    this.router.navigate(['/operator/dashboard']);
  }
}
