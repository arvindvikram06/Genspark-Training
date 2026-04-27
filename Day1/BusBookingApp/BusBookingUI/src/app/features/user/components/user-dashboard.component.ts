import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { BusService } from '../../../core/services/bus.service';
import { AuthService } from '../../../core/services/auth.service';
import { BookingService } from '../../../core/services/booking.service';
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
  busType?: string;
  amenities?: string[];
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
    <div class="min-h-screen bg-slate-950 text-slate-200">
      <div class="max-w-7xl mx-auto px-6 py-8">
        <div class="mb-10">
          <h1 class="text-4xl font-extrabold tracking-tight text-white mb-2">User Dashboard</h1>
          <p class="text-slate-400 font-medium">Hello, {{ userName }}! Book your seats and manage trips.</p>
        </div>

        <div class="mb-6">
          <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-2 mr-2" [class.!bg-indigo-600]="activeTab === 'book'" [class.!text-white]="activeTab === 'book'" (click)="activeTab = 'book'">Book Tickets</button>
          <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-2" [class.!bg-indigo-600]="activeTab === 'history'" [class.!text-white]="activeTab === 'history'" (click)="activeTab = 'history'">Booking History</button>
        </div>

        @if (message) { <p class="text-emerald-400 mb-4">{{ message }}</p> }
        @if (errorMessage) { <p class="text-rose-400 mb-4">{{ errorMessage }}</p> }

        @if (activeTab === 'book') {
          <div class="py-8">
            <h2 class="text-2xl font-bold tracking-tight text-white mb-6">Find Buses</h2>
            <div class="flex gap-4 mb-4">
              <div class="flex-1 relative">
                <input [(ngModel)]="searchQuery" (input)="onSearchInput()" placeholder="Search by destination or travels name..." class="w-full bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 pl-12 focus:outline-none focus:border-indigo-500" />
                <svg class="w-5 h-5 text-slate-500 absolute left-4 top-1/2 -translate-y-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path></svg>
              </div>
              <button (click)="toggleFilters()" class="!bg-slate-800 hover:!bg-slate-700 text-white rounded-lg px-4 py-3 transition-colors">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"></path></svg>
              </button>
              <button class="!bg-indigo-600 !text-white !rounded-lg px-4 py-3 flex-1" (click)="searchBuses()" [disabled]="loadingBuses">Search</button>
              <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-3 flex-1" (click)="loadAllBuses()" [disabled]="loadingBuses">List All</button>
            </div>

            @if (showFilters) {
              <div class="border-t border-slate-800 pt-4 mt-4 mb-8">
                <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
                  <input [(ngModel)]="filters.source" placeholder="Source" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                  <input [(ngModel)]="filters.destination" placeholder="Destination" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                  <input [(ngModel)]="filters.date" type="date" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                  <input [(ngModel)]="filters.minPrice" type="number" placeholder="Min Price" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                </div>
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
                  <input [(ngModel)]="filters.maxPrice" type="number" placeholder="Max Price" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                  <input [(ngModel)]="filters.departureAfter" type="time" placeholder="Departure After" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                  <input [(ngModel)]="filters.departureBefore" type="time" placeholder="Departure Before" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-3 focus:outline-none focus:border-indigo-500" />
                </div>
              </div>
            }

            <div class="grid grid-cols-1 gap-4">
              @if (loadingBuses) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading buses...</p>
                </div>
              } @else {
                @for (bus of buses; track bus.scheduleId) {
                  <div class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl overflow-hidden cursor-pointer hover:!border-slate-700 transition-all" (click)="selectBus(bus)">
                    <div class="flex justify-between items-start">
                      <div class="flex-1">
                        <div class="flex items-center gap-3 mb-2">
                          <h3 class="text-lg font-bold text-white">{{ bus.busName }}</h3>
                          <span class="bg-indigo-500/10 text-indigo-400 text-xs px-2 py-1 rounded-full">{{ bus.busType || 'Standard' }}</span>
                        </div>
                        <p class="text-slate-400 text-sm mb-3">{{ bus.operatorName }}</p>
                        <div class="flex items-center gap-4 text-slate-300 text-sm">
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
                      </div>
                    </div>
                  </div>
                }
                @if (buses.length === 0) {
                  <div class="text-center py-20 border-2 border-dashed border-slate-800 rounded-3xl">
                    <div class="text-6xl mb-4">🚌</div>
                    <p class="text-slate-500 text-lg">No buses found</p>
                    <p class="text-slate-500 text-sm mt-2">Try different dates or routes</p>
                  </div>
                }
              }
            </div>
          </div>
        }

        @if (activeTab === 'history') {
          <div class="py-8">
            <div class="flex justify-between items-center mb-6">
              <h2 class="text-2xl font-bold tracking-tight text-white">Booking History</h2>
              <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-2" (click)="loadHistory()" [disabled]="loadingHistory">Refresh</button>
            </div>
            <div class="grid grid-cols-1 gap-4">
              @if (loadingHistory) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading booking history...</p>
                </div>
              } @else {
                @if (bookingHistory.length === 0) {
                  <div class="text-center py-20 border-2 border-dashed border-slate-800 rounded-3xl">
                    <div class="text-6xl mb-4">📋</div>
                    <p class="text-slate-500 text-lg">No bookings yet</p>
                    <p class="text-slate-500 text-sm mt-2">Your booking history will appear here</p>
                  </div>
                }
                @for (booking of bookingHistory; track booking.id) {
                  <div class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl overflow-hidden" [class.!border-emerald-500/30]="booking.status === 1" [class.!border-rose-500/30]="booking.status === 2" [class.!border-amber-500/30]="booking.status === 0">
                    <div class="flex justify-between items-start">
                      <div class="flex-1">
                        <div class="flex items-center gap-3 mb-2">
                          <span class="text-white font-bold">{{ booking.source }} → {{ booking.destination }}</span>
                          <span [class]="'px-2 py-0.5 rounded text-[10px] font-bold uppercase ' + (booking.status === 1 ? 'bg-emerald-500/10 text-emerald-500' : booking.status === 2 ? 'bg-rose-500/10 text-rose-500' : 'bg-amber-500/10 text-amber-500')">
                            {{ statusLabel(booking.status) }}
                          </span>
                        </div>
                        <div class="flex items-center gap-4 text-slate-300 text-sm">
                          <span class="text-emerald-400 font-bold">Rs {{ booking.totalAmount }}</span>
                          <span>{{ booking.departureTime | date:'short' }}</span>
                        </div>
                      </div>
                      <div class="flex gap-2">
                        @if (booking.status === 1 && canCancelBooking(booking.departureTime)) {
                          <button class="!bg-slate-800 !text-rose-500 !rounded-lg px-3 py-2 text-sm" (click)="cancelBooking(booking.id)">Cancel</button>
                        }
                        <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-3 py-2 text-sm" (click)="loadBookingDetail(booking.id)">Details</button>
                      </div>
                    </div>
                  </div>
                }
              }
            </div>

            @if (selectedHistoryDetail) {
              <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" (click)="closeDetailModal()">
                <div class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
                  <div class="flex justify-between items-center mb-6">
                    <h3 class="text-xl font-bold text-white">Booking Details</h3>
                    <button class="text-slate-500 hover:text-white" (click)="closeDetailModal()">×</button>
                  </div>
                  <div class="space-y-6">
                    <div>
                      <h4 class="text-slate-400 text-sm font-bold uppercase mb-4">Trip Information</h4>
                      <div class="grid grid-cols-2 gap-4">
                        <div>
                          <p class="text-slate-500 text-sm">Booking ID</p>
                          <p class="text-white font-bold">#{{ selectedHistoryDetail.id }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Bus</p>
                          <p class="text-white font-bold">{{ selectedHistoryDetail.busName }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Route</p>
                          <p class="text-white font-bold">{{ selectedHistoryDetail.source }} → {{ selectedHistoryDetail.destination }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Departure</p>
                          <p class="text-white font-bold">{{ selectedHistoryDetail.departureTime | date:'full' }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Boarding Point</p>
                          <p class="text-white font-bold">{{ selectedHistoryDetail.boardingPoint }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Drop Point</p>
                          <p class="text-white font-bold">{{ selectedHistoryDetail.dropPoint }}</p>
                        </div>
                      </div>
                    </div>

                    <div>
                      <h4 class="text-slate-400 text-sm font-bold uppercase mb-4">Payment & Status</h4>
                      <div class="grid grid-cols-2 gap-4">
                        <div>
                          <p class="text-slate-500 text-sm">Total Amount</p>
                          <p class="text-emerald-400 font-bold">Rs {{ selectedHistoryDetail.totalAmount }}</p>
                        </div>
                        <div>
                          <p class="text-slate-500 text-sm">Status</p>
                          <p [class]="'font-bold ' + (selectedHistoryDetail.status === 1 ? 'text-emerald-500' : selectedHistoryDetail.status === 2 ? 'text-rose-500' : 'text-amber-500')">
                            {{ statusLabel(selectedHistoryDetail.status) }}
                          </p>
                        </div>
                      </div>
                    </div>

                    <div>
                      <h4 class="text-slate-400 text-sm font-bold uppercase mb-4">Passenger Details</h4>
                      <div class="space-y-3">
                        @for (p of selectedHistoryDetail.passengers; track p.seatNumber) {
                          <div class="!bg-slate-800 !p-4 !rounded-lg flex items-center gap-4">
                            <div class="!bg-slate-700 !text-white !w-10 !h-10 !rounded-lg flex items-center justify-center font-bold">{{ p.seatNumber }}</div>
                            <div>
                              <p class="text-white font-bold">{{ p.name }}</p>
                              <p class="text-slate-400 text-sm">{{ p.age }} years • {{ p.gender }}</p>
                            </div>
                          </div>
                        }
                      </div>
                    </div>

                    <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-3 w-full" (click)="closeDetailModal()">Close</button>
                  </div>
                </div>
              </div>
            }
          </div>
        }
      </div>
    </div>

    @if (showSeatPopup && selectedBus) {
      <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50" (click)="closeSeatPopup()">
        <div class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
          <div class="flex justify-between items-center mb-6">
            <h2 class="text-xl font-bold text-white">
              @if (popupStage === 'seats') { Select Seats }
              @if (popupStage === 'passengers') { Passenger Details }
              @if (popupStage === 'summary') { Review & Pay }
              - {{ selectedBus.busName }}
            </h2>
            <div class="flex items-center gap-3">
              @if (holdId) {
                <span class="!bg-amber-500/10 !text-amber-500 !px-3 !py-1 !rounded-lg text-sm">Hold: {{ holdTimerLabel }}</span>
              }
              <button class="!bg-slate-800 !text-slate-300 !rounded-lg px-4 py-2" (click)="cancelCurrentFlow()">Cancel</button>
            </div>
          </div>
          <p class="text-slate-400 mb-6">{{ selectedBus.source }} -> {{ selectedBus.destination }} | {{ selectedBus.departureTime | date:'medium' }}</p>

          @if (popupStage === 'seats') {
            <div class="!bg-slate-800 !p-6 !rounded-2xl">
              <h3 class="text-white font-bold mb-4">Live Seat Layout</h3>
              <div class="space-y-2">
                @for (row of seatRows; track row.rowNumber) {
                  <div class="flex items-center gap-2">
                    <div class="flex gap-2">
                      @for (seat of row.left; track seat.seatNumber) {
                        <button class="!w-12 !h-12 !rounded-lg !border-2 !font-bold transition-all"
                          [class.!bg-indigo-600]="isSeatSelected(seat.seatNumber)"
                          [class.!border-indigo-600]="isSeatSelected(seat.seatNumber)"
                          [class.!text-white]="isSeatSelected(seat.seatNumber)"
                          [class.!bg-slate-700]="!isSeatSelected(seat.seatNumber)"
                          [class.!border-slate-600]="!isSeatSelected(seat.seatNumber)"
                          [class.!text-slate-400]="!isSeatSelected(seat.seatNumber)"
                          [class.!bg-slate-900]="getSeatStatus(seat.seatNumber) !== 0"
                          [class.!border-slate-800]="getSeatStatus(seat.seatNumber) !== 0"
                          [class.!text-slate-600]="getSeatStatus(seat.seatNumber) !== 0"
                          [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                          (click)="toggleSeat(seat.seatNumber)">
                          {{ seat.seatNumber }}
                        </button>
                      }
                    </div>
                    <div class="w-8"></div>
                    <div class="flex gap-2">
                      @for (seat of row.right; track seat.seatNumber) {
                        <button class="!w-12 !h-12 !rounded-lg !border-2 !font-bold transition-all"
                          [class.!bg-indigo-600]="isSeatSelected(seat.seatNumber)"
                          [class.!border-indigo-600]="isSeatSelected(seat.seatNumber)"
                          [class.!text-white]="isSeatSelected(seat.seatNumber)"
                          [class.!bg-slate-700]="!isSeatSelected(seat.seatNumber)"
                          [class.!border-slate-600]="!isSeatSelected(seat.seatNumber)"
                          [class.!text-slate-400]="!isSeatSelected(seat.seatNumber)"
                          [class.!bg-slate-900]="getSeatStatus(seat.seatNumber) !== 0"
                          [class.!border-slate-800]="getSeatStatus(seat.seatNumber) !== 0"
                          [class.!text-slate-600]="getSeatStatus(seat.seatNumber) !== 0"
                          [disabled]="getSeatStatus(seat.seatNumber) !== 0"
                          (click)="toggleSeat(seat.seatNumber)">
                          {{ seat.seatNumber }}
                        </button>
                      }
                    </div>
                  </div>
                }
                @if (seatRows.length === 0 && !loading) {
                  <p class="text-slate-500">Seat layout unavailable for this schedule.</p>
                }
              </div>
            </div>
            <div class="flex justify-between items-center mt-6">
              <span class="text-slate-400">Selected: {{ selectedSeats.join(', ') || '-' }}</span>
              <button class="!bg-indigo-600 !text-white !rounded-lg px-6 py-3" (click)="proceedWithSeatHold()" [disabled]="selectedSeats.length === 0 || loading">Proceed</button>
            </div>
          }

          @if (popupStage === 'passengers') {
            <div class="!bg-slate-800 !p-6 !rounded-2xl">
              <h3 class="text-white font-bold mb-4">Enter passenger details for selected seats</h3>
              <div class="space-y-4">
                @for (seat of selectedSeats; track seat) {
                  <div class="grid grid-cols-4 gap-4 items-center">
                    <span class="text-white font-bold">{{ seat }}</span>
                    <input [(ngModel)]="passengerBySeat[seat].name" placeholder="Name" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-2 focus:outline-none focus:border-indigo-500" />
                    <input [(ngModel)]="passengerBySeat[seat].age" type="number" min="1" placeholder="Age" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-2 focus:outline-none focus:border-indigo-500" />
                    <select [(ngModel)]="passengerBySeat[seat].gender" class="bg-slate-900 text-white border border-slate-700 rounded-lg px-4 py-2 focus:outline-none focus:border-indigo-500">
                      <option value="Male">Male</option>
                      <option value="Female">Female</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>
                }
              </div>
              <div class="flex justify-between mt-6">
                <button class="!bg-slate-700 !text-slate-300 !rounded-lg px-6 py-3" (click)="popupStage = 'seats'">Back</button>
                <button class="!bg-indigo-600 !text-white !rounded-lg px-6 py-3" (click)="confirmBooking()" [disabled]="loading">Confirm Booking</button>
              </div>
            </div>
          }

          @if (popupStage === 'summary' && paymentId) {
            @if (paymentProcessing) {
              <div class="!bg-slate-800 !p-6 !rounded-2xl">
                <div class="flex items-center gap-4 mb-6">
                  <div class="!bg-indigo-600 !w-12 !h-12 !rounded-lg flex items-center justify-center">
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                      <circle cx="12" cy="12" r="10" fill="white"/>
                      <path d="M12 6L12 18M6 12L18 12" stroke="#6366f1" stroke-width="2" stroke-linecap="round"/>
                    </svg>
                  </div>
                  <div>
                    <h3 class="text-white font-bold">UPI Payment</h3>
                    <p class="text-slate-400 text-sm">Secure payment gateway</p>
                  </div>
                </div>

                <div class="mb-6">
                  <p class="text-slate-400 text-sm">Amount to Pay</p>
                  <h2 class="text-3xl font-bold text-emerald-400">Rs {{ paymentAmount }}</h2>
                </div>

                <div class="mb-4">
                  <p class="text-slate-400 text-sm">Merchant</p>
                  <p class="text-white font-bold">Bus Booking App</p>
                </div>

                <div class="mb-4">
                  <p class="text-slate-400 text-sm">Transaction ID</p>
                  <p class="text-white font-bold">{{ paymentId }}</p>
                </div>

                <div class="flex flex-col items-center py-8">
                  <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-400">Processing payment...</p>
                </div>

                <div class="flex items-center justify-center gap-2 text-slate-400 text-sm">
                  <svg class="w-4 h-4 text-slate-500" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
                  <span>Secured by SSL</span>
                </div>
              </div>
            } @else {
              <div class="!bg-slate-800 !p-6 !rounded-2xl">
                <h3 class="text-white font-bold mb-4">Booking Summary</h3>
                <div class="flex justify-between items-center mb-6">
                  <div>
                    <p class="text-slate-400 text-sm">Transaction ID: <span class="text-white font-bold">{{ paymentId }}</span></p>
                    <p class="text-slate-400 text-sm">Amount: <span class="text-emerald-400 font-bold">Rs {{ paymentAmount }}</span></p>
                  </div>
                  @if (paymentExpiresAt) {
                    <span class="!bg-amber-500/10 !text-amber-500 !px-3 !py-1 !rounded-lg text-sm">Pay in: {{ paymentTimerLabel }}</span>
                  }
                </div>
                <div class="space-y-2 mb-6">
                  @for (seat of selectedSeats; track seat) {
                    <div class="!bg-slate-900 !p-3 !rounded-lg text-slate-300">
                      {{ seat }} - {{ passengerBySeat[seat].name }}, {{ passengerBySeat[seat].age }}, {{ passengerBySeat[seat].gender }}
                    </div>
                  }
                </div>
                <div class="flex justify-between">
                  <button class="!bg-slate-700 !text-slate-300 !rounded-lg px-6 py-3" (click)="cancelCurrentFlow()" [disabled]="loading">Cancel</button>
                  @if (errorMessage && errorMessage.includes('Payment failed', undefined)) {
                    <button class="!bg-indigo-600 !text-white !rounded-lg px-6 py-3" (click)="retryPayment()" [disabled]="loading">Retry Payment</button>
                  } @else {
                    <button class="!bg-indigo-600 !text-white !rounded-lg px-6 py-3" (click)="payNow()" [disabled]="loading">Pay Now</button>
                  }
                </div>
                @if (errorMessage && errorMessage.includes('Payment failed', undefined)) {
                  <p class="text-rose-400 mt-4">Payment failed. You can retry or cancel to book again.</p>
                }
              </div>
            }
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .dashboard-container { padding: 24px; min-height: 100vh; }
    .glass-card { max-width: 1400px; margin: 0 auto; }
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
    .btn-primary, .btn-secondary, .btn-link, .btn-icon { border: none; border-radius: 8px; padding: 10px 14px; cursor: pointer; }
    .btn-primary { background: #2563eb; color: white; }
    .btn-secondary { background: #334155; color: #e2e8f0; }
    .btn-link { background: transparent; color: #93c5fd; text-decoration: underline; padding: 0; }
    .btn-icon { background: transparent; color: #60a5fa; padding: 8px; border-radius: 8px; display: flex; align-items: center; justify-content: center; transition: all 0.2s; }
    .btn-icon:hover { background: rgba(96, 165, 250, 0.1); color: #93c5fd; }
    .btn-cancel { background: #dc2626; color: white; padding: 6px 12px; font-size: 12px; border-radius: 6px; display: flex; align-items: center; gap: 4px; transition: all 0.2s; }
    .btn-cancel:hover { background: #b91c1c; }
    .history-actions { display: flex; gap: 8px; align-items: center; }
    .history-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
    .history-list { max-height: 500px; overflow-y: auto; padding-right: 8px; }
    .history-list::-webkit-scrollbar { width: 8px; }
    .history-list::-webkit-scrollbar-track { background: #0f172a; border-radius: 4px; }
    .history-list::-webkit-scrollbar-thumb { background: #334155; border-radius: 4px; }
    .history-list::-webkit-scrollbar-thumb:hover { background: #475569; }
    .empty-state { text-align: center; padding: 40px 20px; background: #0b1220; border: 1px dashed #334155; border-radius: 12px; }
    .empty-icon { font-size: 48px; margin-bottom: 12px; }
    .history-card { width: 100%; background: #0b1220; border: 1px solid #334155; border-radius: 12px; padding: 14px 16px; margin-bottom: 12px; }
    .history-card.booking-confirmed { border-left: 4px solid #22c55e; }
    .history-card.booking-cancelled { border-left: 4px solid #ef4444; }
    .history-card.booking-pending { border-left: 4px solid #f59e0b; }
    .history-card-content { display: flex; flex-direction: column; gap: 8px; }
    .history-card-top { display: flex; justify-content: space-between; align-items: center; }
    .history-card-bottom { display: flex; justify-content: space-between; align-items: center; }
    .route-text { color: #e2e8f0; font-size: 0.9rem; font-weight: 600; }
    .status-badge { padding: 4px 10px; border-radius: 9999px; font-size: 0.7rem; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; }
    .status-badge.status-confirmed { background: rgba(34, 197, 94, 0.2); color: #22c55e; }
    .status-badge.status-cancelled { background: rgba(239, 68, 68, 0.2); color: #ef4444; }
    .status-badge.status-pending { background: rgba(245, 158, 11, 0.2); color: #f59e0b; }
    .history-card-body { display: flex; flex-direction: column; gap: 8px; }
    .info-row { display: flex; justify-content: space-between; align-items: center; padding: 8px 0; border-bottom: 1px solid #1e293b; }
    .info-row:last-child { border-bottom: none; }
    .info-label { color: #94a3b8; font-size: 0.875rem; }
    .info-value { color: #e2e8f0; font-size: 0.875rem; font-weight: 500; }
    .history-card-footer .amount { color: #22c55e; font-weight: 700; font-size: 1rem; }
    .detail-modal { position: fixed; inset: 0; background: rgba(2, 6, 23, 0.8); z-index: 1000; }
    .detail-modal-content { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); background: #0f172a; border: 1px solid #334155; border-radius: 16px; padding: 24px; max-width: 600px; width: calc(100% - 40px); max-height: 90vh; overflow-y: auto; }
    .detail-modal-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; padding-bottom: 16px; border-bottom: 1px solid #1e293b; }
    .detail-modal-header h3 { margin: 0; font-size: 1.25rem; color: #e2e8f0; }
    .close-btn { background: transparent; border: none; color: #94a3b8; font-size: 24px; cursor: pointer; padding: 0; width: 32px; height: 32px; display: flex; align-items: center; justify-content: center; border-radius: 8px; }
    .close-btn:hover { background: #1e293b; color: #e2e8f0; }
    .detail-modal-body { display: flex; flex-direction: column; gap: 20px; }
    .detail-section { background: #0b1220; border: 1px solid #1e293b; border-radius: 12px; padding: 16px; }
    .detail-section-title { font-size: 0.875rem; font-weight: 600; color: #60a5fa; text-transform: uppercase; letter-spacing: 0.05em; margin-bottom: 12px; }
    .detail-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 12px; }
    .detail-item { display: flex; flex-direction: column; gap: 4px; }
    .detail-label { color: #94a3b8; font-size: 0.75rem; }
    .detail-value { color: #e2e8f0; font-size: 0.875rem; font-weight: 500; }
    .detail-value.amount { font-size: 1.125rem; font-weight: 700; color: #22c55e; }
    .detail-value.status { font-weight: 600; padding: 4px 12px; border-radius: 9999px; display: inline-block; width: fit-content; }
    .detail-value.status.status-confirmed { background: rgba(34, 197, 94, 0.2); color: #22c55e; }
    .detail-value.status.status-cancelled { background: rgba(239, 68, 68, 0.2); color: #ef4444; }
    .detail-value.status.status-pending { background: rgba(245, 158, 11, 0.2); color: #f59e0b; }
    .passenger-list { display: flex; flex-direction: column; gap: 8px; }
    .passenger-item { display: flex; align-items: center; gap: 12px; padding: 12px; background: #1e293b; border-radius: 8px; }
    .passenger-seat { width: 40px; height: 40px; background: #2563eb; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-weight: 700; color: white; }
    .passenger-info { flex: 1; }
    .passenger-name { color: #e2e8f0; font-weight: 600; }
    .passenger-meta { color: #94a3b8; font-size: 0.875rem; }
    .message { color: #34d399; margin: 0.5rem 0; }
    .error { color: #f87171; margin: 0.5rem 0; }
    .popup-backdrop { position: fixed; inset: 0; background: rgba(2, 6, 23, 0.7); display: flex; justify-content: center; align-items: center; z-index: 1000; padding: 20px; }
    .popup-card { width: min(900px, 95vw); max-height: 90vh; overflow-y: auto; background: #0f172a; border: 1px solid #334155; border-radius: 16px; padding: 16px; }
    .popup-header { display: flex; justify-content: space-between; align-items: center; gap: 10px; margin-bottom: 6px; }
    .popup-actions { display: flex; align-items: center; gap: 8px; }
    .timer-chip { background: #1e3a8a; color: #dbeafe; border: 1px solid #60a5fa; border-radius: 999px; padding: 6px 10px; font-size: 12px; font-weight: 700; }
    .upi-payment-sandbox { background: linear-gradient(135deg, #1e1b4b 0%, #312e81 100%); border-radius: 20px; padding: 32px; text-align: center; border: 2px solid #4f46e5; box-shadow: 0 20px 40px rgba(79, 70, 229, 0.3); }
    .upi-header { display: flex; align-items: center; justify-content: center; gap: 16px; margin-bottom: 32px; }
    .upi-logo { width: 60px; height: 60px; background: linear-gradient(135deg, #6366f1, #8b5cf6); border-radius: 16px; display: flex; align-items: center; justify-content: center; box-shadow: 0 8px 20px rgba(99, 102, 241, 0.4); }
    .upi-header h3 { margin: 0; color: #fff; font-size: 1.5rem; font-weight: 700; }
    .upi-header .muted { margin: 0; color: #a5b4fc; font-size: 0.875rem; }
    .upi-amount { background: rgba(255, 255, 255, 0.1); border-radius: 16px; padding: 24px; margin-bottom: 20px; border: 1px solid rgba(255, 255, 255, 0.2); }
    .upi-amount .muted { margin: 0; color: #a5b4fc; font-size: 0.875rem; margin-bottom: 8px; }
    .upi-amount h2 { margin: 0; color: #fff; font-size: 2.5rem; font-weight: 800; }
    .upi-merchant, .upi-txn-id { background: rgba(0, 0, 0, 0.2); border-radius: 12px; padding: 16px; margin-bottom: 12px; text-align: left; }
    .upi-merchant .muted, .upi-txn-id .muted { margin: 0; color: #a5b4fc; font-size: 0.75rem; margin-bottom: 4px; }
    .upi-merchant p, .upi-txn-id p { margin: 0; color: #fff; }
    .upi-loading { margin: 32px 0; }
    .upi-spinner { width: 48px; height: 48px; border: 4px solid rgba(255, 255, 255, 0.2); border-top-color: #8b5cf6; border-radius: 50%; animation: upi-spin 1s linear infinite; margin: 0 auto 16px; }
    @keyframes upi-spin { to { transform: rotate(360deg); } }
    .upi-loading p { margin: 0; color: #a5b4fc; font-size: 0.875rem; }
    .upi-security { display: flex; align-items: center; justify-content: center; gap: 8px; margin-top: 24px; padding-top: 24px; border-top: 1px solid rgba(255, 255, 255, 0.1); }
    .upi-security .security-icon { color: #22c55e; }
    .upi-security .muted { color: #a5b4fc; font-size: 0.75rem; }
  `]
})
export class UserDashboardComponent implements OnInit, OnDestroy {
  userName = localStorage.getItem('userName');
  private authService = inject(AuthService);
  private busService = inject(BusService);
  private bookingService = inject(BookingService);
  private cdr = inject(ChangeDetectorRef);
  private destroy$ = new Subject<void>();
  private searchSubject$ = new Subject<string>();
  private router = inject(Router);

  loading = false;
  loadingBuses = false;
  loadingHistory = false;
  paymentProcessing = false;
  private _activeTab: 'book' | 'history' = 'book';

  set activeTab(value: 'book' | 'history') {
    this._activeTab = value;
    localStorage.setItem('activeTab', value);
  }
  get activeTab() {
    return this._activeTab;
  }
  checkoutStep: 'passengers' | 'summary' = 'passengers';
  showSeatPopup = false;
  popupStage: 'seats' | 'passengers' | 'summary' = 'seats';
  message = '';
  errorMessage = '';

  searchQuery = '';
  showFilters = false;
  filters = { source: '', destination: '', date: '', minPrice: null, maxPrice: null, departureAfter: '', departureBefore: '' };
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
  paymentExpiresAt: string | null = null;
  paymentCountdownSeconds = 0;
  private paymentTimer: ReturnType<typeof setInterval> | null = null;
  paymentIdempotencyKey: string = '';

  bookingHistory: any[] = [];
  selectedHistoryDetail: any = null;

  ngOnInit() {
    // Restore tab state from localStorage
    const savedTab = localStorage.getItem('activeTab');
    if (savedTab && ['book', 'history'].includes(savedTab)) {
      this._activeTab = savedTab as 'book' | 'history';
    } else if (savedTab === 'checkout') {
      this._activeTab = 'book';
    }

    // Set up debounced search
    this.searchSubject$.pipe(
      debounceTime(300),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.searchBuses();
    });

    this.loadAllBuses();
    this.loadHistory();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchInput() {
    this.searchSubject$.next(this.searchQuery);
  }

  logout() { this.authService.logout(); }

  loadHistory() {
    this.loadingHistory = true;
    this.bookingService.getBookingHistory().subscribe({
      next: (res: any) => {
        this.bookingHistory = [...(res.data || [])];
        this.loadingHistory = false;
        this.done('');
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.loadingHistory = false;
        this.fail(err, 'Failed to load booking history')
      }
    });
  }

  loadBookingDetail(bookingId: number) {
    this.startLoading();
    this.bookingService.getBookingDetail(bookingId).subscribe({
      next: (res: any) => {
        this.selectedHistoryDetail = res.data;
        this.done('');
        this.cdr.detectChanges();
      },
      error: (err: any) => this.fail(err, 'Failed to load booking detail')
    });
  }

  loadAllBuses() {
    this.loadingBuses = true;
    this.busService.getAllUpcomingBuses().subscribe({
      next: (res: any) => {
        this.buses = [...(this.extractList(res) || [])];
        this.loadingBuses = false;
        this.done('Upcoming buses loaded.');
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.loadingBuses = false;
        this.fail(err, 'Failed to load upcoming buses')
      }
    });
  }

  searchBuses() {
    this.loadingBuses = true;
    this.errorMessage = '';
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
        this.buses = [...(this.extractList(res) || [])];
        this.loadingBuses = false;
        this.done('Search results loaded.');
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.loadingBuses = false;
        this.fail(err, 'Failed to search buses')
      }
    });
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  selectBus(bus: BusResult) {
    this.router.navigate(['/user/book', bus.scheduleId]);
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
        this.checkForActiveHold(scheduleId);
      },
      error: (err: any) => this.fail(err, 'Failed to load seat layout')
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
          this.startHoldCountdown(this.holdExpiresAt);
          this.popupStage = 'passengers';
          this.message = 'Your previously selected seats have been restored.';
          this.cdr.detectChanges();
        }
      },
      error: () => {
        // No active hold, continue normally
      }
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
        this.paymentIdempotencyKey = this.generateIdempotencyKey();
        this.paymentExpiresAt = new Date(Date.now() + 10 * 60 * 1000).toISOString();
        this.startPaymentCountdown(this.paymentExpiresAt);
        this.popupStage = 'summary';
        this.checkoutStep = 'summary';
        this.done(`Booking initialized. Transaction id: ${this.paymentId}. Please complete payment within 10 minutes.`);
      },
      error: (err: any) => this.fail(err, 'Booking confirmation failed')
    });
  }

  payNow() {
    if (!this.paymentId) return;
    this.paymentProcessing = true;
    this.bookingService.pay(this.paymentId, 'SIMULATED_REF', this.paymentIdempotencyKey).subscribe({
      next: () => {
        this.paymentProcessing = false;
        this.stopPaymentCountdown();
        this.done('Payment successful. Booking confirmed.');
        this.loadHistory();
        this.resetBookingFlow();
        this.showSeatPopup = false;
        this.activeTab = 'history';
      },
      error: (err: any) => {
        this.paymentProcessing = false;
        const errorMsg = err?.error?.message || err?.message || 'Payment failed';
        if (errorMsg.includes('expired', undefined)) {
          this.fail(err, 'Payment has expired. Please try booking again.');
          this.stopPaymentCountdown();
          this.cancelCurrentFlow();
        } else {
          this.fail(err, 'Payment failed. You can retry or cancel.');
        }
      }
    });
  }

  retryPayment() {
    if (!this.paymentId) return;
    this.startLoading();
    this.bookingService.retryPayment(this.paymentId).subscribe({
      next: (res: any) => {
        this.paymentIdempotencyKey = this.generateIdempotencyKey();
        this.paymentExpiresAt = new Date(Date.now() + 10 * 60 * 1000).toISOString();
        this.startPaymentCountdown(this.paymentExpiresAt);
        this.done('Payment retry successful. Please complete payment within 10 minutes.');
      },
      error: (err: any) => {
        const errorMsg = err?.error?.message || err?.message || 'Retry failed';
        if (errorMsg.includes('Maximum retry', undefined)) {
          this.fail(err, 'Maximum retry attempts exceeded. Please book again.');
        } else {
          this.fail(err, 'Retry failed. Please try again later.');
        }
      }
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

  statusLabel(status: number) {
    if (status === 0) return 'Pending Payment';
    if (status === 1) return 'Confirmed';
    if (status === 2) return 'Cancelled';
    return String(status);
  }

  canCancelBooking(departureTime: string): boolean {
    const departure = new Date(departureTime);
    const now = new Date();
    const hoursUntilDeparture = (departure.getTime() - now.getTime()) / (1000 * 60 * 60);
    return hoursUntilDeparture >= 12;
  }

  cancelBooking(bookingId: number) {
    if (!confirm('Are you sure you want to cancel this booking?')) return;

    this.startLoading();
    this.bookingService.cancelBooking(bookingId).subscribe({
      next: (res: any) => {
        this.done('Booking cancelled successfully. Seats have been released.');
        this.loadHistory();
      },
      error: (err: any) => {
        const errorMsg = err?.error?.message || err?.message || 'Failed to cancel booking';
        if (errorMsg.includes('12 hours')) {
          this.fail(err, 'Bookings can only be cancelled at least 12 hours before departure time');
        } else {
          this.fail(err, errorMsg);
        }
      }
    });
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
    this.stopPaymentCountdown();
    this.selectedSeats = [];
    this.holdId = null;
    this.holdExpiresAt = null;
    this.holdCountdownSeconds = 0;
    this.paymentId = null;
    this.paymentAmount = 0;
    this.paymentExpiresAt = null;
    this.paymentCountdownSeconds = 0;
    this.paymentIdempotencyKey = '';
    this.checkoutStep = 'passengers';
    this.popupStage = 'seats';
    this.passengerBySeat = {};
    if (this.selectedBus) this.loadSeatMap(this.selectedBus.scheduleId);
  }

  closeDetailModal() {
    this.selectedHistoryDetail = null;
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

  get paymentTimerLabel() {
    const mins = Math.floor(this.paymentCountdownSeconds / 60);
    const secs = this.paymentCountdownSeconds % 60;
    return `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;
  }

  private generateIdempotencyKey(): string {
    return `${Date.now()}-${Math.random().toString(36).substring(2, 15)}`;
  }

  private startPaymentCountdown(expiresAt: string | null) {
    this.stopPaymentCountdown();
    if (!expiresAt) return;

    const expiryTime = new Date(expiresAt).getTime();
    this.paymentCountdownSeconds = Math.max(0, Math.floor((expiryTime - Date.now()) / 1000));

    this.paymentTimer = setInterval(() => {
      const secondsLeft = Math.max(0, Math.floor((expiryTime - Date.now()) / 1000));
      this.paymentCountdownSeconds = secondsLeft;
      if (secondsLeft === 0) {
        this.stopPaymentCountdown();
        this.errorMessage = 'Payment time expired. Please retry booking.';
        this.cancelCurrentFlow();
      }
      this.cdr.detectChanges();
    }, 1000);
  }

  private stopPaymentCountdown() {
    if (this.paymentTimer) {
      clearInterval(this.paymentTimer);
      this.paymentTimer = null;
    }
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
