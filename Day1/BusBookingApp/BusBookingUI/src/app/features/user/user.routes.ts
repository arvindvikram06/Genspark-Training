import { Routes } from '@angular/router';
import { UserDashboardComponent } from './components/user-dashboard.component';
import { BusBookingComponent } from './components/bus-booking.component';

export const USER_ROUTES: Routes = [
    { path: 'dashboard', component: UserDashboardComponent },
    { path: 'book/:id', component: BusBookingComponent },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
