import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { BusService } from '../../../core/services/bus.service';
import { BookingService } from '../../../core/services/booking.service';

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
}

interface SeatLayoutItem {
  row: number;
  col: number;
  seatNumber: string;
}

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="dashboard-container">
      <div class="glass-card">
        <div class="header">
          <div>
            <h1 class="text-gradient">User Dashboard</h1>
            <p class="subtitle">Hello, {{ userName }}! Book your seats and manage trips.</p>
          </div>
          <button (click)="logout()" class="btn-primary">Logout</button>
        </div>

        <div class="tabs">
          <button class="tab-btn" [class.active]="activeTab === 'book'" (click)="activeTab = 'book'">Book Tickets</button>
          <button class="tab-btn" [class.active]="activeTab === 'checkout'" (click)="activeTab = 'checkout'">Checkout</button>
          <button class="tab-btn" [class.active]="activeTab === 'history'" (click)="activeTab = 'history'">Booking History</button>
        </div>

        @if (message) { <p class="message">{{ message }}</p> }
        @if (errorMessage) { <p class="error">{{ errorMessage }}</p> }

        @if (activeTab === 'book') {
          <div class="section">
            <h2>Find Buses</h2>
            <div class="search-grid">
              <input [(ngModel)]="search.source" placeholder="Source (e.g. Chennai)" />
              <input [(ngModel)]="search.destination" placeholder="Destination (e.g. Madurai)" />
              <input [(ngModel)]="search.date" type="date" />
              <button class="btn-secondary" (click)="searchBuses()" [disabled]="loading">Search</button>
              <button class="btn-secondary" (click)="loadAllBuses()" [disabled]="loading">List All Upcoming</button>
            </div>

            <div class="bus-list">
              @for (bus of buses; track bus.scheduleId) {
                <button class="bus-card" [class.selected-card]="selectedBus?.scheduleId === bus.scheduleId" (click)="selectBus(bus)">
                  <div>
                    <strong>{{ bus.busName }}</strong>
                    <div class="muted">{{ bus.source }} -> {{ bus.destination }}</div>
                    <div class="muted">Departure: {{ bus.departureTime | date:'medium' }}</div>
                  </div>
                  <div class="right">
                    <div>Rs {{ bus.pricePerSeat }}</div>
                    <div class="muted">{{ bus.availableSeats }}/{{ bus.totalSeats }} seats</div>
                  </div>
                </button>
              }
              @if (!loading && buses.length === 0) { <p class="muted">No buses found.</p> }
            </div>
          </div>
        }

        @if (activeTab === 'checkout') {
          <div class="section">
            <h2>Checkout</h2>
            <p class="muted">Checkout is now continuous inside the seat popup window.</p>
            @if (showSeatPopup) {
              <button class="btn-primary" (click)="activeTab = 'book'">Go to active popup</button>
            }
          </div>
        }

        @if (activeTab === 'history') {
          <div class="section">
            <div class="history-header">
              <h2>Booking History</h2>
              <button class="btn-secondary" (click)="loadHistory()">Refresh</button>
            </div>
            @for (booking of bookingHistory; track booking.id) {
              <div class="history-card">
                <div>
                  <strong>#{{ booking.id }}</strong> - {{ booking.source }} -> {{ booking.destination }}
                  <div class="muted">{{ booking.departureTime | date:'medium' }}</div>
                </div>
                <div class="right">
                  <div>Rs {{ booking.totalAmount }}</div>
                  <div class="muted">{{ statusLabel(booking.status) }}</div>
                  <button class="btn-link" (click)="loadBookingDetail(booking.id)">Details</button>
                </div>
              </div>
            }
            @if (bookingHistory.length === 0) { <p class="muted">No bookings yet.</p> }

            @if (selectedHistoryDetail) {
              <div class="detail-card">
                <h3>Booking #{{ selectedHistoryDetail.id }}</h3>
                <p>{{ selectedHistoryDetail.busName }} - {{ selectedHistoryDetail.source }} -> {{ selectedHistoryDetail.destination }}</p>
                <p>Boarding: {{ selectedHistoryDetail.boardingPoint }} | Drop: {{ selectedHistoryDetail.dropPoint }}</p>
                <p>Total: Rs {{ selectedHistoryDetail.totalAmount }} | Status: {{ statusLabel(selectedHistoryDetail.status) }}</p>
                <ul>
                  @for (p of selectedHistoryDetail.passengers; track p.seatNumber) {
                    <li>{{ p.seatNumber }} - {{ p.name }}, {{ p.age }}, {{ p.gender }}</li>
                  }
                </ul>
              </div>
            }
          </div>
        }
      </div>
    </div>

    @if (showSeatPopup && selectedBus) {
      <div class="popup-backdrop" (click)="closeSeatPopup()">
        <div class="popup-card" (click)="$event.stopPropagation()">
          <div class="popup-header">
            <h2>
              @if (popupStage === 'seats') { Select Seats }
              @if (popupStage === 'passengers') { Passenger Details }
              @if (popupStage === 'summary') { Review & Pay }
              - {{ selectedBus.busName }}
            </h2>
            <div class="popup-actions">
              @if (holdId) {
                <span class="timer-chip">Hold: {{ holdTimerLabel }}</span>
              }
              <button class="btn-secondary" (click)="cancelCurrentFlow()">Cancel</button>
            </div>
          </div>
          <div class="muted">{{ selectedBus.source }} -> {{ selectedBus.destination }} | {{ selectedBus.departureTime | date:'medium' }}</div>

          @if (popupStage === 'seats') {
            <div class="seat-shell">
              <div class="seat-head">Live Seat Layout</div>
              <div class="seat-layout">
                @for (row of seatRows; track row.rowNumber) {
                  <div class="seat-row">
                    <div class="seat-group">
                      @for (seat of row.left; track seat.seatNumber) {
                        <button class="seat-node"
                          [class.active-seat]="isSeatSelected(seat.seatNumber)"
                          [class.booked-seat]="getSeatStatus(seat.seatNumber) !== 0"
                          [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                          (click)="toggleSeat(seat.seatNumber)">
                          {{ seat.seatNumber }}
                        </button>
                      }
                    </div>
                    <div class="aisle"></div>
                    <div class="seat-group">
                      @for (seat of row.right; track seat.seatNumber) {
                        <button class="seat-node"
                          [class.active-seat]="isSeatSelected(seat.seatNumber)"
                          [class.booked-seat]="getSeatStatus(seat.seatNumber) !== 0"
                          [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                          (click)="toggleSeat(seat.seatNumber)">
                          {{ seat.seatNumber }}
                        </button>
                      }
                    </div>
                  </div>
                }
                @if (seatRows.length === 0 && !loading) {
                  <p class="muted">Seat layout unavailable for this schedule.</p>
                }
              </div>
            </div>
            <div class="actions">
              <span class="muted">Selected: {{ selectedSeats.join(', ') || '-' }}</span>
              <button class="btn-primary" (click)="proceedWithSeatHold()" [disabled]="selectedSeats.length === 0 || loading">Proceed</button>
            </div>
          }

          @if (popupStage === 'passengers') {
            <div class="section">
              <h3>Enter passenger details for selected seats</h3>
              @for (seat of selectedSeats; track seat) {
                <div class="passenger-grid">
                  <strong>{{ seat }}</strong>
                  <input [(ngModel)]="passengerBySeat[seat].name" placeholder="Name" />
                  <input [(ngModel)]="passengerBySeat[seat].age" type="number" min="1" placeholder="Age" />
                  <select [(ngModel)]="passengerBySeat[seat].gender">
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                  </select>
                </div>
              }
              <div class="actions">
                <button class="btn-secondary" (click)="popupStage = 'seats'">Back</button>
                <button class="btn-primary" (click)="confirmBooking()" [disabled]="loading">Confirm Booking</button>
              </div>
            </div>
          }

          @if (popupStage === 'summary' && paymentId) {
            <div class="section">
              <h3>Booking Summary</h3>
              <p><strong>Transaction ID:</strong> {{ paymentId }}</p>
              <p><strong>Amount:</strong> Rs {{ paymentAmount }}</p>
              <ul>
                @for (seat of selectedSeats; track seat) {
                  <li>{{ seat }} - {{ passengerBySeat[seat].name }}, {{ passengerBySeat[seat].age }}, {{ passengerBySeat[seat].gender }}</li>
                }
              </ul>
              <div class="actions">
                <button class="btn-secondary" (click)="cancelCurrentFlow()">Cancel</button>
                <button class="btn-primary" (click)="payNow()" [disabled]="loading">Pay Now</button>
              </div>
            </div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .dashboard-container { padding: 24px; min-height: 100vh; }
    .glass-card { max-width: 1100px; margin: 0 auto; }
    .header { display: flex; justify-content: space-between; align-items: center; gap: 12px; }
    .subtitle { color: var(--text-muted); margin-bottom: 1rem; }
    .tabs { display: flex; gap: 10px; margin-bottom: 1rem; }
    .tab-btn { background: #1e293b; color: #cbd5e1; border: 1px solid #334155; border-radius: 10px; padding: 10px 14px; cursor: pointer; }
    .tab-btn.active { background: #2563eb; color: #fff; border-color: #60a5fa; }
    .section { background: rgba(15, 23, 42, 0.55); border: 1px solid var(--border); border-radius: 16px; padding: 1rem; margin-top: 1rem; }
    .search-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(170px, 1fr)); gap: 8px; margin-bottom: 10px; }
    input, select { background: #0f172a; color: #e2e8f0; border: 1px solid #334155; border-radius: 8px; padding: 10px; }
    .bus-list { display: flex; flex-direction: column; gap: 8px; }
    .bus-card, .history-card { width: 100%; display: flex; justify-content: space-between; align-items: center; background: #0b1220; border: 1px solid #334155; border-radius: 12px; padding: 10px; text-align: left; color: #e2e8f0; }
    .selected-card { border-color: #60a5fa; }
    .right { text-align: right; }
    .muted { color: #94a3b8; font-size: 0.9rem; }
    .seat-shell { border: 1px solid #1e293b; border-radius: 18px; overflow: hidden; background: #020617; }
    .seat-head { font-size: 11px; text-transform: uppercase; letter-spacing: 0.1em; padding: 12px 14px; border-bottom: 1px solid #1e293b; color: #94a3b8; }
    .seat-layout { padding: 16px; }
    .seat-row { display: flex; justify-content: center; align-items: center; gap: 16px; margin-bottom: 10px; }
    .seat-group { display: flex; gap: 8px; min-width: 110px; justify-content: center; }
    .aisle { width: 24px; border-top: 1px dashed #334155; }
    .seat-node { width: 48px; height: 48px; background: linear-gradient(145deg, #1e293b, #0f172a); border: 1px solid #334155; border-radius: 12px; color: #cbd5e1; font-size: 12px; font-weight: 700; cursor: pointer; }
    .seat-node:hover { border-color: #818cf8; }
    .active-seat { background: #1d4ed8; border-color: #60a5fa; color: #fff; }
    .booked-seat { background: #475569; color: #94a3b8; cursor: not-allowed; }
    .actions { margin-top: 12px; display: flex; justify-content: space-between; align-items: center; }
    .passenger-grid { display: grid; grid-template-columns: 60px repeat(3, minmax(120px, 1fr)); gap: 8px; margin-bottom: 8px; align-items: center; }
    .btn-primary, .btn-secondary, .btn-link { border: none; border-radius: 8px; padding: 10px 14px; cursor: pointer; }
    .btn-primary { background: #2563eb; color: white; }
    .btn-secondary { background: #334155; color: #e2e8f0; }
    .btn-link { background: transparent; color: #93c5fd; text-decoration: underline; padding: 0; }
    .history-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
    .detail-card { margin-top: 10px; background: #0b1220; border: 1px solid #334155; border-radius: 8px; padding: 10px; }
    .message { color: #34d399; margin: 0.5rem 0; }
    .error { color: #f87171; margin: 0.5rem 0; }
    .popup-backdrop { position: fixed; inset: 0; background: rgba(2, 6, 23, 0.7); display: flex; justify-content: center; align-items: center; z-index: 1000; padding: 20px; }
    .popup-card { width: min(900px, 95vw); max-height: 90vh; overflow-y: auto; background: #0f172a; border: 1px solid #334155; border-radius: 16px; padding: 16px; }
    .popup-header { display: flex; justify-content: space-between; align-items: center; gap: 10px; margin-bottom: 6px; }
    .popup-actions { display: flex; align-items: center; gap: 8px; }
    .timer-chip { background: #1e3a8a; color: #dbeafe; border: 1px solid #60a5fa; border-radius: 999px; padding: 6px 10px; font-size: 12px; font-weight: 700; }
  `]
})
export class UserDashboardComponent implements OnInit {
  userName = localStorage.getItem('userName');
  private authService = inject(AuthService);
  private busService = inject(BusService);
  private bookingService = inject(BookingService);
  private cdr = inject(ChangeDetectorRef);

  loading = false;
  activeTab: 'book' | 'checkout' | 'history' = 'book';
  checkoutStep: 'passengers' | 'summary' = 'passengers';
  showSeatPopup = false;
  popupStage: 'seats' | 'passengers' | 'summary' = 'seats';
  message = '';
  errorMessage = '';

  search = { source: '', destination: '', date: '' };
  buses: BusResult[] = [];
  selectedBus: BusResult | null = null;

  seatStatusMap: Record<string, number> = {};
  seatRows: Array<{ rowNumber: number; left: SeatLayoutItem[]; right: SeatLayoutItem[] }> = [];
  selectedSeats: string[] = [];

  holdId: number | null = null;
  holdExpiresAt: string | null = null;
  holdCountdownSeconds = 0;
  private holdTimer: ReturnType<typeof setInterval> | null = null;
  passengerBySeat: Record<string, { name: string; age: number; gender: string }> = {};
  paymentId: number | null = null;
  paymentAmount = 0;

  bookingHistory: any[] = [];
  selectedHistoryDetail: any = null;

  ngOnInit() {
    this.loadAllBuses();
    this.loadHistory();
  }

  logout() { this.authService.logout(); }

  loadAllBuses() {
    this.startLoading();
    this.busService.getAllUpcomingBuses().subscribe({
      next: (res: any) => {
        this.buses = this.extractList(res);
        this.done('Upcoming buses loaded.');
      },
      error: (err: any) => this.fail(err, 'Failed to load upcoming buses')
    });
  }

  searchBuses() {
    if (!this.search.source || !this.search.destination || !this.search.date) {
      this.errorMessage = 'Please enter source, destination and date.';
      return;
    }
    this.startLoading();
    this.busService.searchBuses(this.search).subscribe({
      next: (res: any) => {
        this.buses = this.extractList(res);
        this.done('Search results loaded.');
      },
      error: (err: any) => this.fail(err, 'Failed to search buses')
    });
  }

  selectBus(bus: BusResult) {
    this.activeTab = 'book';
    this.selectedBus = bus;
    this.selectedSeats = [];
    this.holdId = null;
    this.holdExpiresAt = null;
    this.paymentId = null;
    this.selectedHistoryDetail = null;
    this.passengerBySeat = {};
    this.checkoutStep = 'passengers';
    this.popupStage = 'seats';
    this.showSeatPopup = true;
    this.stopHoldCountdown();
    this.loadSeatMap(bus.scheduleId);
    this.cdr.detectChanges();
  }

  closeSeatPopup() {
    this.cancelCurrentFlow();
  }

  loadSeatMap(scheduleId: number) {
    this.startLoading();
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
        this.done('Seat layout loaded.');
      },
      error: (err: any) => this.fail(err, 'Failed to load seat layout')
    });
  }

  buildSeatRows(layout: SeatLayoutItem[], statuses: Array<{ seatNumber: string; status: any }>) {
    if (!layout.length && statuses.length) {
      layout = statuses.map((s) => this.parseSeatNumber(s.seatNumber)).filter((s): s is SeatLayoutItem => !!s);
    }

    const rowMap = new Map<number, SeatLayoutItem[]>();
    layout.forEach((seat) => {
      if (!rowMap.has(seat.row)) rowMap.set(seat.row, []);
      rowMap.get(seat.row)!.push(seat);
    });

    return Array.from(rowMap.entries())
      .sort((a, b) => a[0] - b[0])
      .map(([rowNumber, seats]) => {
        const ordered = seats.sort((a, b) => a.col - b.col);
        return {
          rowNumber,
          left: ordered.filter((s) => s.col <= 2),
          right: ordered.filter((s) => s.col > 2)
        };
      });
  }

  toggleSeat(seatNumber: string) {
    if (this.getSeatStatus(seatNumber) !== 0) return;
    if (this.selectedSeats.includes(seatNumber)) {
      this.selectedSeats = this.selectedSeats.filter((s) => s !== seatNumber);
      delete this.passengerBySeat[seatNumber];
      return;
    }
    this.selectedSeats.push(seatNumber);
    this.passengerBySeat[seatNumber] = { name: '', age: 0, gender: 'Male' };
  }

  isSeatSelected(seatNumber: string) {
    return this.selectedSeats.includes(seatNumber);
  }

  getSeatStatus(seatNumber: string) {
    return this.seatStatusMap[seatNumber] ?? 0;
  }

  proceedWithSeatHold() {
    if (!this.selectedBus || this.selectedSeats.length === 0) return;
    this.startLoading();
    this.bookingService.holdSeats(this.selectedBus.scheduleId, this.selectedSeats).subscribe({
      next: (res: any) => {
        this.holdId = res.data?.holdId;
        this.holdExpiresAt = res.data?.expiresAt;
        this.startHoldCountdown(this.holdExpiresAt);
        this.popupStage = 'passengers';
        this.done(`Seats held. Expires at ${res.data?.expiresAt}.`);
      },
      error: (err: any) => this.fail(err, 'Seat hold failed.')
    });
  }

  confirmBooking() {
    if (!this.holdId) return;
    const passengers = this.selectedSeats.map((seat) => ({
      seatNumber: seat,
      name: this.passengerBySeat[seat]?.name?.trim(),
      age: Number(this.passengerBySeat[seat]?.age),
      gender: this.passengerBySeat[seat]?.gender
    }));

    if (passengers.some((p) => !p.name || !p.age || !p.gender)) {
      this.errorMessage = 'Please fill passenger details for all seats.';
      return;
    }

    this.startLoading();
    this.bookingService.confirmBooking(this.holdId, passengers).subscribe({
      next: (res: any) => {
        this.holdId = null;
        this.holdExpiresAt = null;
        this.stopHoldCountdown();
        this.paymentId = res.data?.paymentId;
        this.paymentAmount = res.data?.amount || 0;
        this.popupStage = 'summary';
        this.checkoutStep = 'summary';
        this.done(`Booking initialized. Transaction id: ${this.paymentId}`);
      },
      error: (err: any) => this.fail(err, 'Booking confirmation failed')
    });
  }

  payNow() {
    if (!this.paymentId) return;
    this.startLoading();
    this.bookingService.pay(this.paymentId).subscribe({
      next: () => {
        this.done('Payment successful. Booking confirmed.');
        this.loadHistory();
        this.resetBookingFlow();
        this.showSeatPopup = false;
        this.activeTab = 'history';
      },
      error: (err: any) => this.fail(err, 'Payment failed')
    });
  }

  cancelCurrentFlow() {
    if (this.loading) return;

    if (this.holdId) {
      this.startLoading();
      this.bookingService.releaseHold(this.holdId).subscribe({
        next: () => {
          this.done('Seat hold cancelled and seats released.');
          this.resetBookingFlow();
          this.showSeatPopup = false;
        },
        error: (err: any) => this.fail(err, 'Failed to release hold')
      });
      return;
    }

    if (this.paymentId) {
      this.startLoading();
      this.bookingService.abortPayment(this.paymentId).subscribe({
        next: () => {
          this.done('Checkout cancelled and seats released.');
          this.loadHistory();
          this.resetBookingFlow();
          this.showSeatPopup = false;
        },
        error: (err: any) => this.fail(err, 'Failed to cancel checkout flow')
      });
      return;
    }

    this.resetBookingFlow();
    this.showSeatPopup = false;
  }

  loadHistory() {
    this.bookingService.getBookingHistory().subscribe({
      next: (res: any) => {
        this.bookingHistory = res.data || [];
        this.cdr.detectChanges();
      },
      error: () => {
        this.bookingHistory = [];
        this.cdr.detectChanges();
      }
    });
  }

  loadBookingDetail(bookingId: number) {
    this.bookingService.getBookingDetail(bookingId).subscribe({
      next: (res: any) => {
        this.selectedHistoryDetail = res.data;
        this.cdr.detectChanges();
      },
      error: (err: any) => this.fail(err, 'Failed to load booking detail')
    });
  }

  statusLabel(status: number) {
    if (status === 0) return 'Pending Payment';
    if (status === 1) return 'Confirmed';
    if (status === 2) return 'Cancelled';
    return String(status);
  }

  private parseSeatNumber(seatNumber: string): SeatLayoutItem | null {
    const match = /^(\d+)([A-Z])$/i.exec(seatNumber || '');
    if (!match) return null;
    const row = Number(match[1]);
    const col = match[2].toUpperCase().charCodeAt(0) - 64;
    return { row, col, seatNumber };
  }

  private normalizeSeatStatus(status: any): number {
    if (typeof status === 'number') return status;
    const val = String(status || '').toLowerCase();
    if (val === 'available') return 0;
    if (val === 'held') return 1;
    if (val === 'booked') return 2;
    return 2;
  }

  private resetBookingFlow() {
    this.stopHoldCountdown();
    this.selectedSeats = [];
    this.holdId = null;
    this.holdExpiresAt = null;
    this.holdCountdownSeconds = 0;
    this.paymentId = null;
    this.paymentAmount = 0;
    this.checkoutStep = 'passengers';
    this.popupStage = 'seats';
    this.passengerBySeat = {};
    if (this.selectedBus) this.loadSeatMap(this.selectedBus.scheduleId);
  }

  private startHoldCountdown(expiresAt: string | null) {
    this.stopHoldCountdown();
    if (!expiresAt) return;

    const expiryTime = new Date(expiresAt).getTime();
    this.holdCountdownSeconds = Math.max(0, Math.floor((expiryTime - Date.now()) / 1000));

    this.holdTimer = setInterval(() => {
      const secondsLeft = Math.max(0, Math.floor((expiryTime - Date.now()) / 1000));
      this.holdCountdownSeconds = secondsLeft;
      if (secondsLeft === 0) {
        this.stopHoldCountdown();
        this.errorMessage = 'Seat hold expired. Please select seats again.';
        if (this.holdId) {
          this.cancelCurrentFlow();
        } else {
          this.resetBookingFlow();
        }
      }
      this.cdr.detectChanges();
    }, 1000);
  }

  private stopHoldCountdown() {
    if (this.holdTimer) {
      clearInterval(this.holdTimer);
      this.holdTimer = null;
    }
  }

  get holdTimerLabel() {
    const mins = Math.floor(this.holdCountdownSeconds / 60);
    const secs = this.holdCountdownSeconds % 60;
    return `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;
  }

  private startLoading() {
    this.loading = true;
    this.errorMessage = '';
    this.message = '';
  }

  private done(message: string) {
    this.loading = false;
    this.message = message;
    this.cdr.detectChanges();
  }

  private fail(err: any, fallback: string) {
    this.loading = false;
    this.errorMessage = err?.error?.message || fallback;
    this.cdr.detectChanges();
  }

  private extractList(res: any): BusResult[] {
    const list = Array.isArray(res) ? res : Array.isArray(res?.data) ? res.data : [];
    const now = Date.now();
    return list.filter((bus: BusResult) => new Date(bus.departureTime).getTime() > now);
  }

  private extractData(res: any): any {
    if (res?.data) return res.data;
    return res;
  }
}
