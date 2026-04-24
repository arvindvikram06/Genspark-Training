import { HttpInterceptorFn } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, throwError, Subject } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class ToastService {
    private toastSubject = new Subject<string>();
    public toast$ = this.toastSubject.asObservable();

    show(message: string) {
        this.toastSubject.next(message);
    }
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const toastService = inject(ToastService);

    return next(req).pipe(
        catchError(err => {
            if (err.status === 401) {
                authService.logout();
            } else if (err.status === 409) {
                if (err.error?.message?.includes('hold expired')) {
                    toastService.show('Session expired: Your seat hold has been released.');
                }
            }
            return throwError(() => err);
        })
    );
};
