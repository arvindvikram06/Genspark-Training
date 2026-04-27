import { Routes } from '@angular/router';
import { OperatorDashboardComponent } from './components/operator-dashboard.component';
import { SeatLayoutComponent } from './components/seat-layout.component';
import { AddBusComponent } from './components/add-bus.component';

export const OPERATOR_ROUTES: Routes = [
    { path: 'dashboard', component: OperatorDashboardComponent },
    { path: 'add-bus', component: AddBusComponent },
    { path: 'seat-layout/:id', component: SeatLayoutComponent },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
