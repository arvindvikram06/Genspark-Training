import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiService } from './api.service';

@Injectable({
    providedIn: 'root'
})
export class BusService {
    private api = inject(ApiService);

    getAllUpcomingBuses(): Observable<any> {
        return this.api.get<any>('buses/all').pipe(
            catchError(() => this.api.get<any>('buses').pipe(
                catchError(() => this.api.get<any>('PublicBus'))
            ))
        );
    }

    searchBuses(params: any): Observable<any> {
        return this.api.get<any>('buses/search', params).pipe(
            catchError(() => this.api.get<any>('PublicBus/search', params))
        );
    }

    getSeatMap(scheduleId: number): Observable<any> {
        return this.api.get<any>(`buses/${scheduleId}/seats`).pipe(
            catchError(() => this.api.get<any>(`PublicBus/seats/${scheduleId}`))
        );
    }
}
