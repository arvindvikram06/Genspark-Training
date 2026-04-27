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

    pay(paymentId: number, transactionRef?: string, idempotencyKey?: string): Observable<any> {
        let suffix = transactionRef ? `?transactionRef=${encodeURIComponent(transactionRef)}` : '?';
        if (idempotencyKey) {
            suffix += suffix.includes('?') ? `&idempotencyKey=${encodeURIComponent(idempotencyKey)}` : `?idempotencyKey=${encodeURIComponent(idempotencyKey)}`;
        }
        return this.api.post(`Payment/pay/${paymentId}${suffix}`, {});
    }

    abortPayment(paymentId: number): Observable<any> {
        return this.api.post(`Payment/abort/${paymentId}`, {});
    }

    getPaymentStatus(paymentId: number): Observable<any> {
        return this.api.get(`Payment/status/${paymentId}`);
    }

    retryPayment(paymentId: number): Observable<any> {
        return this.api.post(`Payment/retry/${paymentId}`, {});
    }

    releaseHold(holdId: number): Observable<any> {
        return this.api.delete(`Booking/hold/${holdId}`);
    }

    cancelBooking(bookingId: number): Observable<any> {
        return this.api.post(`Booking/${bookingId}/cancel`, {});
    }

    getActiveSeatHold(scheduleId: number): Observable<any> {
        return this.api.get(`Booking/hold/active?scheduleId=${scheduleId}`);
    }
}
