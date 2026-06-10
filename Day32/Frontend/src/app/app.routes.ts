import { Routes } from '@angular/router';
import { Login } from './components/login/login';
import { Products } from './components/products/products';
import { Dashboard } from './components/dashboard/dashboard';
import { authGuard } from './guards/auth-guard';
import { ProductDetails } from './components/product-details/product-details';
import { Profile } from './components/profile/profile';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'products',
    component: Products,
    canActivate: [authGuard]
  },
  {
    path: 'dashboard',
    component: Dashboard,
    canActivate: [authGuard]
  },
  {
    path:'products/:id',
    component: ProductDetails,
    canActivate: [authGuard]
  },
  {
    path:'profile',
    component: Profile,
    canActivate: [authGuard]
  }
];