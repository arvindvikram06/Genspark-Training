import { Routes } from '@angular/router';
import { OperatorDashboardComponent } from './components/operator-dashboard.component';

export const OPERATOR_ROUTES: Routes = [
    { path: 'dashboard', component: OperatorDashboardComponent },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
