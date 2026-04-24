import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface AuthResponse {
    success: boolean;
    data: {
        token: string;
        userId: number;
        email: string;
        name: string;
        role: string;
    };
    message: string;
}

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private http = inject(HttpClient);
    private router = inject(Router);
    private apiUrl = 'http://localhost:5134/api/Auth';

    constructor() { }

    login(credentials: any): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
            tap(res => {
                if (res.success) {
                    localStorage.setItem('token', res.data.token);
                    localStorage.setItem('role', res.data.role);
                    localStorage.setItem('email', res.data.email);
                    localStorage.setItem('userName', res.data.name);
                }
            })
        );
    }

    registerUser(userData: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register/user`, userData);
    }

    registerOperator(operatorData: any): Observable<any> {
        return this.http.post(`${this.apiUrl}/register/operator`, operatorData);
    }

    logout() {
        localStorage.clear();
        this.router.navigate(['/login']);
    }

    getRole(): string | null {
        return localStorage.getItem('role');
    }

    getToken(): string | null {
        return localStorage.getItem('token');
    }

    isLoggedIn(): boolean {
        return !!this.getToken();
    }
}
