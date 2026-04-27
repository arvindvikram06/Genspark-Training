import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, MatButtonModule, MatIconModule, MatMenuModule],
  template: `
    @if (showHeader) {
    <nav class="bg-slate-900 border-b border-slate-800 px-6 py-2 flex items-center justify-between sticky top-0 z-50 shadow-sm">
      <div class="flex items-center gap-10">
        <a routerLink="/" class="flex items-center gap-2 group decoration-transparent">
          <div class="w-10 h-10 bg-indigo-600 rounded-xl flex items-center justify-center text-white shadow-lg shadow-indigo-500/30 group-hover:rotate-6 transition-transform">
            <mat-icon class="!flex !items-center !justify-center">directions_bus</mat-icon>
          </div>
          <span class="text-2xl font-black text-white tracking-tighter hidden sm:inline">BusBooking</span>
        </a>

        <div class="hidden md:flex items-center gap-8">
          @if (!(authService.currentUser$ | async)) {
            <a routerLink="/public/search" routerLinkActive="!text-indigo-400 !font-bold"
               class="text-slate-400 hover:text-indigo-400 font-semibold text-sm uppercase tracking-wider transition-all">
              Search Buses
            </a>
          }
        </div>
      </div>

      <div class="flex items-center gap-3">
        @if (authService.currentUser$ | async; as user) {
          <div class="flex items-center gap-4">
            <span class="text-slate-400 hidden lg:inline font-medium">Hello, <span class="text-white font-bold">{{ user.name }}</span></span>

              <button mat-icon-button [matMenuTriggerFor]="userMenu" class="!bg-slate-800 !text-slate-300 hover:!bg-slate-700">
                <mat-icon>person</mat-icon>
              </button>

              <mat-menu #userMenu="matMenu" xPosition="before">
                <div class="px-4 py-3 border-b border-slate-700 min-w-[180px]">
                  <p class="text-[10px] text-slate-500 uppercase font-bold tracking-wider">Account</p>
                  <p class="text-xs text-slate-300 font-bold truncate">{{ user.email }}</p>
                </div>
                <!-- Simple buttons without overlapping experimental classes -->
                <button mat-menu-item [routerLink]="getDashboardRoute(user.role)">
                  <mat-icon>dashboard</mat-icon>
                  <span>My Portal</span>
                </button>
                <div class="border-t border-slate-700 my-1"></div>
                <button mat-menu-item (click)="logout()">
                  <mat-icon class="text-red-400">logout</mat-icon>
                  <span class="text-red-400">Sign Out</span>
                </button>
              </mat-menu>
          </div>
        } @else {
          <div class="flex items-center gap-2">
            <a mat-button routerLink="/auth/login" class="!text-slate-300 !font-bold !px-6 hover:!bg-slate-800 !rounded-lg">
              Log In
            </a>
            <a mat-flat-button routerLink="/auth/register-user"
               class="!bg-indigo-600 !text-white !rounded-lg !px-6 !font-bold hover:!bg-indigo-700 transition-all shadow-lg shadow-indigo-500/30">
              Join Now
            </a>
          </div>
        }
      </div>
    </nav>
    }
  `,
  styles: [`
    :host { display: block; }
    ::ng-deep .mat-mdc-menu-panel {
      background-color: #1e293b !important;
      min-width: 220px !important;
      margin-top: 8px !important;
      border-radius: 12px !important;
      overflow: hidden !important;
      border: 1px solid #334155 !important;
      box-shadow: 0 20px 25px -5px rgb(0 0 0 / 0.3), 0 8px 10px -6px rgb(0 0 0 / 0.2) !important;
    }
    ::ng-deep .mat-mdc-menu-content {
      background-color: #1e293b !important;
    }
    ::ng-deep .mat-mdc-menu-item {
      background-color: #1e293b !important;
      color: #e2e8f0 !important;
    }
    ::ng-deep .mat-mdc-menu-item:hover {
      background-color: #334155 !important;
    }
    ::ng-deep .mat-mdc-menu-item .mat-icon {
      color: #94a3b8 !important;
    }
    ::ng-deep .mat-mdc-menu-item:hover .mat-icon {
      color: #e2e8f0 !important;
    }
  `]
})
export class HeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);
  showHeader = true;

  constructor() {
    this.router.events.subscribe(() => {
      const currentUrl = this.router.url;
      // Show header on all pages
      this.showHeader = true;
    });
  }

  getDashboardRoute(role: string): string {
    switch (role) {
      case 'Admin': return '/admin/dashboard';
      case 'Operator': return '/operator/dashboard';
      default: return '/user/dashboard';
    }
  }

  logout() {
    this.authService.logout();
  }
}
