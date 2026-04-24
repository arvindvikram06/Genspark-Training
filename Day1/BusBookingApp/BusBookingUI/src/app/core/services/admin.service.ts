import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface OperatorSummary {
    id: number;
    name: string;
    email: string;
    phone?: string;
    status: number;
    headOfficeDistrict: string;
    busCount: number;
    totalRevenue: number;
}

export interface BusSummary {
    id: number;
    name: string;
    operatorName?: string;
    totalSeats: number;
    status: number;
    createdAt: string;
}

export interface RouteDto {
    id: number;
    source: string;
    destination: string;
}

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    private api = inject(ApiService);

    // Operators
    getOperators(): Observable<any> {
        return this.api.get('Admin/operators');
    }

    approveOperator(id: number): Observable<any> {
        return this.api.put(`Admin/operators/${id}/approve`, {});
    }

    disableOperator(id: number): Observable<any> {
        return this.api.put(`Admin/operators/${id}/disable`, {});
    }

    getOperatorBuses(id: number): Observable<any> {
        return this.api.get(`Admin/operators/${id}/buses`);
    }

    getAllBuses(): Observable<any> {
        return this.api.get('Admin/buses');
    }

    approveBus(id: number): Observable<any> {
        return this.api.put(`Admin/buses/${id}/approve`, {});
    }

    disableBus(id: number): Observable<any> {
        return this.api.put(`Admin/buses/${id}/disable`, {});
    }

    // Routes
    getRoutes(): Observable<any> {
        return this.api.get('Admin/routes');
    }

    createRoute(source: string, destination: string): Observable<any> {
        return this.api.post('Admin/routes', { source, destination });
    }

    updateRoute(id: number, source: string, destination: string): Observable<any> {
        return this.api.put(`Admin/routes/${id}`, { source, destination });
    }

    deleteRoute(id: number): Observable<any> {
        return this.api.delete(`Admin/routes/${id}`);
    }

    // Schedules
    getPendingSchedules(): Observable<any> {
        return this.api.get('Admin/schedules/pending');
    }

    approveSchedule(id: number): Observable<any> {
        return this.api.put(`Admin/schedules/${id}/approve`, {});
    }

    // Config
    getConfig(): Observable<any> {
        return this.api.get('Admin/config');
    }

    updateConvenienceFee(fee: number): Observable<any> {
        return this.api.put('Admin/config/convenience-fee', { fee });
    }
}
