import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
    providedIn: 'root'
})
export class BookingService {
    private api = inject(ApiService);

    holdSeats(scheduleId: number, seatNumbers: string[]): Observable<any> {
        return this.api.post('Booking/hold', { scheduleId, seatNumbers });
    }

    confirmBooking(holdId: number, passengers: any[]): Observable<any> {
        return this.api.post('Booking/confirm', { holdId, passengers });
    }

    getBookingHistory(): Observable<any> {
        return this.api.get('Booking/my');
    }

    getBookingDetail(bookingId: number): Observable<any> {
        return this.api.get(`Booking/${bookingId}`);
    }

    pay(paymentId: number, transactionRef?: string): Observable<any> {
        const suffix = transactionRef ? `?transactionRef=${encodeURIComponent(transactionRef)}` : '';
        return this.api.post(`Payment/pay/${paymentId}${suffix}`, {});
    }

    abortPayment(paymentId: number): Observable<any> {
        return this.api.post(`Payment/abort/${paymentId}`, {});
    }

    releaseHold(holdId: number): Observable<any> {
        return this.api.delete(`Booking/hold/${holdId}`);
    }

    cancelBooking(bookingId: number): Observable<any> {
        return this.api.post(`Booking/${bookingId}/cancel`, {});
    }
}
