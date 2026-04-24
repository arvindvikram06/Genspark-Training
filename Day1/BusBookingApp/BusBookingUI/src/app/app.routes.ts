import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'public/search', pathMatch: 'full' },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'public',
    loadChildren: () => import('./features/public/public.routes').then(m => m.PUBLIC_ROUTES)
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] },
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },
  {
    path: 'operator',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Operator'] },
    loadChildren: () => import('./features/operator/operator.routes').then(m => m.OPERATOR_ROUTES)
  },
  {
    path: 'user',
    canActivate: [authGuard, roleGuard],
    data: { roles: ['User'] },
    loadChildren: () => import('./features/user/user.routes').then(m => m.USER_ROUTES)
  }
];
