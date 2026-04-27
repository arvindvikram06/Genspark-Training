import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
    providedIn: 'root'
})
export class OperatorService {
    private api = inject(ApiService);

    getBuses(): Observable<any> {
        return this.api.get('operator/buses');
    }

    createBus(data: any): Observable<any> {
        return this.api.post('operator/buses', data);
    }

    deleteBus(id: number): Observable<any> {
        return this.api.delete(`operator/buses/${id}`);
    }

    disableBus(data: any): Observable<any> {
        return this.api.post('operator/buses/disable', data);
    }

    getSchedules(): Observable<any> {
        return this.api.get('operator/schedules');
    }

    createSchedule(data: any): Observable<any> {
        return this.api.post('operator/schedules', data);
    }

    getScheduleSeats(scheduleId: number): Observable<any> {
        return this.api.get(`operator/schedules/${scheduleId}/seats`);
    }

    getRoutes(): Observable<any> {
        return this.api.get('operator/schedules/routes');
    }

    getDistricts(): Observable<any> {
        return this.api.get('operator/districts');
    }

    getOffices(): Observable<any> {
        return this.api.get('operator/offices');
    }

    submitOffices(offices: any[]): Observable<any> {
        return this.api.post('operator/offices', offices);
    }

    getProfile(): Observable<any> {
        return this.api.get('operator/profile');
    }
}
