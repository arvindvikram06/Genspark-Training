import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { ApiService } from './api.service';
import { DecodedToken } from '../models/user.model';
import { jwtDecode } from 'jwt-decode';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private api = inject(ApiService);
    private router = inject(Router);
    private readonly TOKEN_KEY = 'bb_token';

    private currentUserSubject = new BehaviorSubject<DecodedToken | null>(null);
    public currentUser$ = this.currentUserSubject.asObservable();

    constructor() {
        this.loadToken();
    }

    login(email: string, password: string): Observable<any> {
        return this.api.post<any>('Auth/login', { email, password }).pipe(
            tap(res => {
                if (res.success && res.data.token) {
                    this.setToken(res.data.token);
                }
            })
        );
    }

    registerUser(userData: any): Observable<any> {
        return this.api.post('Auth/register/user', userData);
    }

    registerOperator(operatorData: any): Observable<any> {
        return this.api.post('Auth/register/operator', operatorData);
    }

    logout() {
        localStorage.removeItem(this.TOKEN_KEY);
        this.currentUserSubject.next(null);
        this.router.navigate(['/auth/login']);
    }

    private setToken(token: string) {
        localStorage.setItem(this.TOKEN_KEY, token);
        const decoded = this.decodeToken(token);
        this.currentUserSubject.next(decoded);
    }

    private loadToken() {
        const token = this.getToken();
        if (token) {
            const decoded = this.decodeToken(token);
            if (decoded && decoded.exp * 1000 > Date.now()) {
                this.currentUserSubject.next(decoded);
            } else {
                this.logout();
            }
        }
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }

    getRole(): 'User' | 'Operator' | 'Admin' | null {
        return this.currentUserSubject.value?.role || null;
    }

    getUserId(): string | null {
        return this.currentUserSubject.value?.userId || null;
    }

    private decodeToken(token: string): DecodedToken | null {
        try {
            const decoded: any = jwtDecode(token);
            // Map .NET claim types to simple properties if necessary
            return {
                userId: decoded.userId || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
                email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
                name: decoded.name || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || decoded['unique_name'],
                role: decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
                exp: decoded.exp
            } as DecodedToken;
        } catch {
            return null;
        }
    }
}
