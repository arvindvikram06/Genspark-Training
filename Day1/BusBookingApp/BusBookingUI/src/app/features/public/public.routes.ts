import { Routes } from '@angular/router';
import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { BusService } from '../../core/services/bus.service';
import { AuthService } from '../../core/services/auth.service';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

interface BusResult {
  scheduleId: number;
  busName: string;
  operatorName: string;
  source: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  pricePerSeat: number;
  totalSeats: number;
  availableSeats: number;
  boardingPoint: string;
  dropPoint: string;
  busType: string;
  amenities: string[];
}

interface SeatLayoutItem {
  row: number;
  col: number;
  seatNumber: string;
}

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="min-h-screen bg-slate-950 text-slate-200">
      <div class="max-w-7xl mx-auto px-6 py-8">
        <div class="text-center mb-10">
          <h1 class="text-4xl font-extrabold tracking-tight text-white mb-2">Find Your Bus</h1>
          <p class="text-slate-400 font-medium">Search buses across routes without logging in</p>
        </div>

        <div class="bg-slate-900 rounded-2xl p-6 mb-8 border border-slate-800">
          <div class="flex gap-4 mb-4">
            <div class="flex-1 relative">
              <input [(ngModel)]="searchQuery" (input)="onSearchInput()" placeholder="Search by destination or travels name..." class="w-full bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 pl-12 focus:outline-none focus:border-indigo-500" />
              <svg class="w-5 h-5 text-slate-500 absolute left-4 top-1/2 -translate-y-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
            </div>
            <button (click)="toggleFilters()" class="!bg-slate-800 hover:!bg-slate-700 text-white rounded-lg px-4 py-3 transition-colors">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"></path></svg>
            </button>
            <button (click)="searchBuses()" [disabled]="loading" class="!bg-indigo-600 hover:!bg-indigo-700 text-white font-semibold rounded-lg px-6 py-3 transition-colors disabled:opacity-50">
              @if (loading) { Searching... } @else { Search }
            </button>
          </div>

          @if (showFilters) {
            <div class="border-t border-slate-800 pt-4 mt-4">
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
                <input [(ngModel)]="filters.source" placeholder="Source" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                <input [(ngModel)]="filters.destination" placeholder="Destination" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                <input [(ngModel)]="filters.date" type="date" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                <input [(ngModel)]="filters.minPrice" type="number" placeholder="Min Price" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
              </div>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
                <input [(ngModel)]="filters.maxPrice" type="number" placeholder="Max Price" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                <input [(ngModel)]="filters.departureAfter" type="time" placeholder="Departure After" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                <input [(ngModel)]="filters.departureBefore" type="time" placeholder="Departure Before" class="bg-slate-950 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
              </div>
            </div>
          }

          <button (click)="loadAllBuses()" [disabled]="loading" class="mt-4 text-indigo-400 hover:text-indigo-300 text-sm">View all upcoming buses</button>
        </div>

        @if (error) {
          <div class="bg-rose-500/10 border border-rose-500/30 rounded-xl p-6 text-center">
            <p class="text-rose-400 mb-4">{{ error }}</p>
            <button (click)="loadAllBuses()" class="bg-rose-600 hover:bg-rose-700 text-white px-4 py-2 rounded-lg">Retry</button>
          </div>
        } @else if (loading) {
          <div class="flex flex-col items-center justify-center py-20">
            <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
            <p class="text-slate-500">Loading buses...</p>
          </div>
        } @else {
          <div class="grid grid-cols-1 gap-4">
            @for (bus of buses; track bus.scheduleId) {
              <div class="bg-slate-900 rounded-xl p-6 border border-slate-800 hover:border-slate-700 transition-colors">
                <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
                  <div class="flex-1">
                    <div class="flex items-center gap-3 mb-2">
                      <h3 class="text-xl font-bold text-white">{{ bus.busName }}</h3>
                      <span class="bg-indigo-500/10 text-indigo-400 text-xs px-2 py-1 rounded-full">{{ bus.busType || 'Standard' }}</span>
                    </div>
                    <p class="text-slate-400 text-sm mb-3">{{ bus.operatorName }}</p>
                    <div class="flex items-center gap-4 text-slate-300">
                      <span class="flex items-center gap-1">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"></path><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"></path></svg>
                        {{ bus.source }} → {{ bus.destination }}
                      </span>
                      <span class="flex items-center gap-1">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path></svg>
                        {{ bus.departureTime | date:'mediumDate' }}
                      </span>
                      <span class="flex items-center gap-1">
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
                        {{ bus.departureTime | date:'shortTime' }}
                      </span>
                    </div>
                    @if (bus.amenities && bus.amenities.length > 0) {
                      <div class="flex gap-2 mt-3">
                        @for (amenity of bus.amenities; track amenity) {
                          <span class="bg-slate-800 text-slate-300 text-xs px-2 py-1 rounded">{{ amenity }}</span>
                        }
                      </div>
                    }
                  </div>
                  <div class="flex flex-col items-end gap-3">
                    <div class="text-right">
                      <p class="text-2xl font-bold text-emerald-400">Rs {{ bus.pricePerSeat }}</p>
                      <p class="text-slate-500 text-sm">{{ bus.availableSeats }}/{{ bus.totalSeats }} seats available</p>
                    </div>
                    <div class="flex gap-2">
                      <button (click)="viewSeats(bus)" class="!bg-slate-800 hover:!bg-slate-700 text-white px-4 py-2 rounded-lg transition-colors text-sm">View Seats</button>
                      <button (click)="proceedToBook(bus)" class="!bg-indigo-600 hover:!bg-indigo-700 text-white px-4 py-2 rounded-lg transition-colors text-sm font-semibold">Proceed to Book</button>
                    </div>
                  </div>
                </div>
              </div>
            }
            @if (buses.length === 0 && !loading) {
              <div class="text-center py-20 border-2 border-dashed border-slate-800 rounded-3xl">
                <div class="text-6xl mb-4">🚌</div>
                <p class="text-slate-500 text-lg">No buses found for your search</p>
                <p class="text-slate-500 text-sm mt-2">Try different dates or routes</p>
              </div>
            }
          </div>
        }
      </div>

      @if (showSeatModal) {
        <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" (click)="closeSeatModal()">
          <div class="bg-slate-900 border border-slate-800 rounded-2xl p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
            <div class="flex justify-between items-center mb-6">
              <div>
                <h2 class="text-2xl font-bold text-white">{{ selectedBus?.busName }}</h2>
                <p class="text-slate-400">{{ selectedBus?.source }} → {{ selectedBus?.destination }}</p>
              </div>
              <button (click)="closeSeatModal()" class="text-slate-500 hover:text-white text-2xl">&times;</button>
            </div>

            @if (loadingSeats) {
              <div class="flex flex-col items-center justify-center py-20">
                <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
                <p class="text-slate-500">Loading seat layout...</p>
              </div>
            } @else {
              <div class="border border-slate-800 rounded-xl p-4 bg-slate-950">
                <div class="flex justify-center gap-8 mb-4 text-sm">
                  <div class="flex items-center gap-2">
                    <div class="w-6 h-6 bg-slate-800 rounded"></div>
                    <span class="text-slate-400">Available</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <div class="w-6 h-6 bg-slate-700 rounded"></div>
                    <span class="text-slate-400">Booked</span>
                  </div>
                </div>

                <div class="flex flex-col items-center gap-2">
                  @for (row of seatRows; track row.rowNumber) {
                    <div class="flex items-center gap-4">
                      <div class="flex gap-2">
                        @for (seat of row.left; track seat.seatNumber) {
                          <button class="w-10 h-10 rounded text-sm font-semibold transition-colors"
                            [class]="getSeatStatus(seat.seatNumber) === 'booked' ? 'bg-slate-700 text-slate-500 cursor-not-allowed' : 'bg-slate-800 text-white hover:bg-slate-700'"
                            [disabled]="getSeatStatus(seat.seatNumber) === 'booked'">
                            {{ seat.seatNumber }}
                          </button>
                        }
                      </div>
                      <div class="w-8"></div>
                      <div class="flex gap-2">
                        @for (seat of row.right; track seat.seatNumber) {
                          <button class="w-10 h-10 rounded text-sm font-semibold transition-colors"
                            [class]="getSeatStatus(seat.seatNumber) === 'booked' ? 'bg-slate-700 text-slate-500 cursor-not-allowed' : 'bg-slate-800 text-white hover:bg-slate-700'"
                            [disabled]="getSeatStatus(seat.seatNumber) === 'booked'">
                            {{ seat.seatNumber }}
                          </button>
                        }
                      </div>
                    </div>
                  }
                </div>
              </div>

              <div class="mt-6 text-center">
                <p class="text-slate-400 text-sm mb-4">Login to book seats</p>
                <button (click)="proceedToBook(selectedBus!)" class="!bg-indigo-600 hover:!bg-indigo-700 text-white px-6 py-3 rounded-lg font-semibold transition-colors">
                  Proceed to Book
                </button>
              </div>
            }
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class SearchComponent implements OnInit, OnDestroy {
  private busService = inject(BusService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthService);
  private destroy$ = new Subject<void>();
  private searchSubject$ = new Subject<string>();

  loading = false;
  buses: BusResult[] = [];
  searchQuery = '';
  showFilters = false;
  filters = { source: '', destination: '', date: '', minPrice: null, maxPrice: null, departureAfter: '', departureBefore: '' };
  error = '';

  showSeatModal = false;
  selectedBus: BusResult | null = null;
  seatStatusMap: Record<string, number> = {};
  seatRows: Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> = [];
  loadingSeats = false;

  ngOnInit() {
    // Redirect authenticated users to their dashboard
    if (this.authService.isLoggedIn()) {
      const role = this.authService.getRole();
      if (role === 'Operator') {
        this.router.navigate(['/operator/dashboard']);
      } else if (role === 'Admin') {
        this.router.navigate(['/admin/dashboard']);
      } else if (role === 'User') {
        this.router.navigate(['/user/dashboard']);
      }
      return;
    }

    // Set up debounced search
    this.searchSubject$.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.searchBuses();
    });

    // Load all upcoming buses on init
    this.loadAllBuses();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchInput() {
    this.searchSubject$.next(this.searchQuery);
  }

  loadAllBuses() {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();
    this.busService.getAllUpcomingBuses().subscribe({
      next: (res: any) => {
        console.log('Buses loaded:', res);
        this.buses = res.data || [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading buses:', err);
        this.error = 'Failed to load buses. Please try again.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  searchBuses() {
    this.loading = true;
    this.error = '';
    this.cdr.detectChanges();

    // Build query params, only include non-empty values
    const params: any = {};
    if (this.searchQuery) params.query = this.searchQuery;
    if (this.filters.source) params.source = this.filters.source;
    if (this.filters.destination) params.destination = this.filters.destination;
    if (this.filters.date) params.date = this.filters.date;
    if (this.filters.minPrice) params.minPrice = this.filters.minPrice;
    if (this.filters.maxPrice) params.maxPrice = this.filters.maxPrice;
    if (this.filters.departureAfter) params.departureAfter = this.filters.departureAfter + ':00';
    if (this.filters.departureBefore) params.departureBefore = this.filters.departureBefore + ':00';

    console.log('Search params:', params);
    this.busService.searchBuses(params).subscribe({
      next: (res: any) => {
        console.log('Search results:', res);
        this.buses = res.data || [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error searching buses:', err);
        this.error = 'Failed to search buses. Please try again.';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  viewSeats(bus: BusResult) {
    this.selectedBus = bus;
    this.showSeatModal = true;
    this.loadingSeats = true;
    this.busService.getSeatMap(bus.scheduleId).subscribe({
      next: (res: any) => {
        console.log('Seat map response:', res);
        // The API returns a complex object with schedule and seats array
        const data = res.data;
        let seats = [];

        if (Array.isArray(data)) {
          seats = data;
        } else if (data && Array.isArray(data.seats)) {
          seats = data.seats;
        } else if (data && Array.isArray(data)) {
          seats = data;
        }

        this.seatStatusMap = {};
        seats.forEach((seat: any) => {
          this.seatStatusMap[seat.seatNumber] = seat.status;
        });
        console.log('Seat status map:', this.seatStatusMap);
        this.buildSeatLayout();
        this.loadingSeats = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.error('Error loading seat layout:', err);
        this.loadingSeats = false;
        this.cdr.detectChanges();
      }
    });
  }

  buildSeatLayout() {
    const rows = Math.ceil(this.selectedBus!.totalSeats / 4);
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

  closeSeatModal() {
    this.showSeatModal = false;
    this.selectedBus = null;
    this.seatRows = [];
    this.seatStatusMap = {};
  }

  getSeatStatus(seatNumber: string): string {
    const status = this.seatStatusMap[seatNumber];
    if (status === 1) return 'booked';
    if (status === 2) return 'booked';
    return 'available';
  }

  proceedToBook(bus: BusResult) {
    this.router.navigate(['/auth/login'], { queryParams: { redirect: 'search', busId: bus.scheduleId } });
  }
}

export const PUBLIC_ROUTES: Routes = [
  { path: 'search', component: SearchComponent }
];
