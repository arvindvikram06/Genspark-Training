import { Component, inject, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AdminService, OperatorSummary, RouteDto, BusSummary } from '../../../core/services/admin.service';

// Inline Dialog Component for Routes
@Component({
  selector: 'app-route-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  template: `
    <div class="p-6 bg-slate-800 text-white min-w-[350px]">
      <h2 class="text-2xl font-bold mb-6 text-white">{{data.id ? 'Edit' : 'Add New'}} Route</h2>
      <div class="space-y-4">
        <mat-form-field appearance="outline" class="w-full custom-dark-field">
          <mat-label>Source</mat-label>
          <input matInput [(ngModel)]="data.source" placeholder="e.g. Chennai" required>
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-full custom-dark-field">
          <mat-label>Destination</mat-label>
          <input matInput [(ngModel)]="data.destination" placeholder="e.g. Madurai" required>
        </mat-form-field>
      </div>
      <div class="flex justify-end gap-3 mt-8">
        <button mat-button (click)="onCancel()" class="!text-slate-400">Cancel</button>
        <button mat-flat-button color="primary" (click)="onSave()" 
                [disabled]="!data.source || !data.destination" class="!px-6">
          {{data.id ? 'Save Changes' : 'Create Route'}}
        </button>
      </div>
    </div>
  `,
  styles: [`
    ::ng-deep .custom-dark-field .mat-mdc-text-field-wrapper { background: #1e293b !important; border: 1px solid #334155 !important; }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-label-wrapper label { color: #94a3b8 !important; }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-input-control { color: #e2e8f0 !important; background: #1e293b !important; }
    ::ng-deep .custom-dark-field .mat-mdc-select-panel { background: #1e293b !important; border: 1px solid #334155 !important; }
    ::ng-deep .custom-dark-field .mat-mdc-option { background: #1e293b !important; color: #e2e8f0 !important; }
    ::ng-deep .custom-dark-field .mat-calendar { background: #1e293b !important; border: 1px solid #334155 !important; }
  `]
})
export class RouteDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<RouteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  onCancel() { this.dialogRef.close(); }
  onSave() { this.dialogRef.close(this.data); }
}

// Operator Details Dialog Component
@Component({
  selector: 'app-operator-details-dialog',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatCardModule, MatDialogModule, MatSnackBarModule, MatTooltipModule],
  template: `
    <div class="p-8 bg-slate-900 text-white w-full max-h-[85vh] overflow-y-auto">
      <div class="flex justify-between items-start mb-8">
        <div>
          <h2 class="text-3xl font-black text-white mb-2">{{data.op.name}}</h2>
          <p class="text-slate-400 font-mono tracking-tighter">{{data.op.email}} | {{data.op.phone || 'No Phone'}}</p>
        </div>
        <div class="text-right">
          <p class="text-xs text-slate-500 uppercase font-bold tracking-widest mb-1">Total Revenue</p>
          <p class="text-2xl font-black text-emerald-400 font-mono">₹{{data.op.totalRevenue.toLocaleString()}}</p>
        </div>
      </div>

      <div class="grid grid-cols-3 gap-4 mb-8">
        <div class="bg-slate-900 p-4 rounded-xl border border-slate-800">
          <p class="text-[10px] text-slate-500 uppercase font-bold mb-1">Status</p>
          <p class="font-bold text-white uppercase text-sm tracking-widest">{{data.op.status === 1 ? 'Active' : 'Pending'}}</p>
        </div>
        <div class="bg-slate-900 p-4 rounded-xl border border-slate-800">
          <p class="text-[10px] text-slate-500 uppercase font-bold mb-1">Base District</p>
          <p class="font-bold text-white text-sm uppercase">{{data.op.headOfficeDistrict}}</p>
        </div>
        <div class="bg-slate-900 p-4 rounded-xl border border-slate-800">
          <p class="text-[10px] text-slate-500 uppercase font-bold mb-1">Fleet Strength</p>
          <p class="font-bold text-white text-sm uppercase">{{data.op.busCount}} Buses</p>
        </div>
      </div>

      <h3 class="text-xl font-bold mb-4 flex items-center gap-2">
        <mat-icon class="text-indigo-500">directions_bus</mat-icon> Fleet Inventory
      </h3>
      
      <mat-card class="!bg-slate-900 !border-slate-800 !rounded-xl overflow-hidden p-0">
        <table mat-table [dataSource]="buses" class="!bg-transparent w-full">
          <ng-container matColumnDef="name">
            <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !border-slate-700 !w-1/2">Bus Name</th>
            <td mat-cell *matCellDef="let b" class="!text-white !border-slate-700 font-bold !py-3">{{b.name || 'No Name'}}</td>
          </ng-container>
          <ng-container matColumnDef="seats">
            <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !border-slate-700 !w-1/4 text-center">Capacity</th>
            <td mat-cell *matCellDef="let b" class="!text-slate-300 !border-slate-700 font-mono !py-3 text-center">{{b.totalSeats}} Seats</td>
          </ng-container>
          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !border-slate-700 !w-1/6 text-right pr-4">Status</th>
            <td mat-cell *matCellDef="let b" class="!border-slate-700 !py-3 text-right pr-4">
              <span [class]="'px-2 py-0.5 rounded text-[10px] font-bold uppercase ' + (b.status === 1 ? 'bg-emerald-500/10 text-emerald-500' : 'bg-amber-500/10 text-amber-500')">
                {{b.status === 1 ? 'Approved' : 'Pending'}}
              </span>
            </td>
          </ng-container>
          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !border-slate-700 !w-1/6 text-right pr-4">Actions</th>
            <td mat-cell *matCellDef="let b" class="!border-slate-700 !py-3 text-right pr-4">
              <div class="flex justify-end gap-1">
                @if(b.status !== 1) {
                  <button mat-icon-button class="!text-emerald-500 scale-75" (click)="approveBus(b.id)" matTooltip="Approve Bus">
                    <mat-icon>check_circle</mat-icon>
                  </button>
                } @else {
                  <button mat-icon-button class="!text-rose-500 scale-75" (click)="disableBus(b.id)" matTooltip="Disable Bus">
                    <mat-icon>block</mat-icon>
                  </button>
                }
              </div>
            </td>
          </ng-container>
          <tr mat-header-row *matHeaderRowDef="['name', 'seats', 'status', 'actions']" class="!bg-slate-900/40"></tr>
          <tr mat-row *matRowDef="let row; columns: ['name', 'seats', 'status', 'actions'];" class="hover:bg-slate-700/20"></tr>
        </table>
        @if(buses.length === 0) {
          <div class="p-8 text-center text-slate-500 border-t border-slate-700 font-medium">
            No registered vehicles found in this operator's fleet
          </div>
        }
      </mat-card>

      <div class="mt-8 flex justify-end">
        <button mat-flat-button color="primary" (click)="dialogRef.close()" class="!px-8 !rounded-lg">Close Archive</button>
      </div>
    </div>
  `
})
export class OperatorDetailsDialogComponent implements OnInit {
  buses: BusSummary[] = [];
  private adminService = inject(AdminService);
  private snack = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);
  constructor(
    public dialogRef: MatDialogRef<OperatorDetailsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }
  ngOnInit() {
    this.loadBuses();
  }
  loadBuses() {
    console.log('Loading buses for operator ID:', this.data.op.id);
    this.adminService.getOperatorBuses(this.data.op.id).subscribe({
      next: (res) => {
        console.log('Buses response:', res);
        this.buses = res.data || [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error loading buses:', err);
        this.snack.open('Failed to load buses', 'OK', { duration: 2000 });
      }
    });
  }
  approveBus(id: number) {
    this.adminService.approveBus(id).subscribe(() => {
      this.snack.open('Bus approved', 'OK', { duration: 2000 });
      this.loadBuses();
    });
  }
  disableBus(id: number) {
    this.adminService.disableBus(id).subscribe(() => {
      this.snack.open('Bus grounded', 'OK', { duration: 2000 });
      this.loadBuses();
    });
  }
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTabsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatDialogModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatInputModule,
    MatTooltipModule
  ],
  template: `
    <div class="p-6 min-h-screen bg-slate-950 text-slate-200">
      <div class="max-w-7xl mx-auto">
        <div class="mb-10">
          <h1 class="text-4xl font-extrabold tracking-tight text-white mb-2">Admin Command Center</h1>
          <p class="text-slate-400 font-medium">System orchestration and operator lifecycle management</p>
        </div>

        <mat-tab-group dynamicHeight class="custom-tabs backdrop-blur-sm" [(selectedIndex)]="selectedTabIndex">
          <!-- Operators -->
          <mat-tab>
            <ng-template mat-tab-label>
              <mat-icon class="mr-2">business</mat-icon> Operators
            </ng-template>
            <div class="py-8">
              @if (loadingOperators) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading operators...</p>
                </div>
              } @else {
                <div class="mb-6">
                  <mat-form-field appearance="outline" class="!w-full !bg-slate-900">
                    <mat-label class="!text-slate-400">Search operators</mat-label>
                    <input matInput [(ngModel)]="operatorSearchQuery" placeholder="Search by name, email, or district..." class="!text-white">
                    <mat-icon matPrefix class="!text-slate-500">search</mat-icon>
                  </mat-form-field>
                </div>

                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl overflow-hidden shadow-2xl p-0">
                  <table mat-table [dataSource]="filteredOperators" class="!bg-transparent w-full">
                  <ng-container matColumnDef="name">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !py-4">Operator Entity</th>
                    <td mat-cell *matCellDef="let op" class="!text-white !border-slate-700 font-bold !py-4">{{op.name}}</td>
                  </ng-container>

                  <ng-container matColumnDef="email">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !py-4">Compliance Contact</th>
                    <td mat-cell *matCellDef="let op" class="!text-slate-300 !border-slate-700 !py-4">
                      <div class="flex flex-col">
                        <span class="font-medium">{{op.email}}</span>
                        <span class="text-xs text-slate-500">{{op.phone || 'No phone provided'}}</span>
                      </div>
                    </td>
                  </ng-container>

                  <ng-container matColumnDef="district">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !py-4">Base Operations</th>
                    <td mat-cell *matCellDef="let op" class="!text-slate-300 !border-slate-700 !py-4">{{op.headOfficeDistrict}}</td>
                  </ng-container>

                  <ng-container matColumnDef="status">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !py-4">Protocol Status</th>
                    <td mat-cell *matCellDef="let op" class="!border-slate-700 !py-4">
                      <div [class]="'inline-flex items-center px-3 py-1 rounded-full text-[10px] font-black uppercase tracking-wider ' + getStatusClass(op.status)">
                        <span class="w-1.5 h-1.5 rounded-full mr-2" [class]="getStatusDotClass(op.status)"></span>
                        {{op.status === 0 ? 'Pending' : op.status === 1 ? 'Approved' : 'Disabled'}}
                      </div>
                    </td>
                  </ng-container>

                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !py-4">Control Plane</th>
                    <td mat-cell *matCellDef="let op" class="!border-slate-700 !py-4 text-right pr-6">
                      <div class="flex justify-end gap-2">
                        <button mat-mini-fab class="!bg-indigo-600/20 !text-indigo-400" (click)="viewOperatorDetails(op)" matTooltip="View Detailed Profile">
                           <mat-icon>visibility</mat-icon>
                        </button>
                        @if(op.status === 0) {
                          <button mat-mini-fab color="primary" class="!bg-emerald-600" (click)="approveOperator(op.id)" matTooltip="Execute Approval">
                            <mat-icon>how_to_reg</mat-icon>
                          </button>
                        }
                        @if(op.status !== 2) {
                          <button mat-mini-fab color="warn" class="!bg-rose-600" (click)="disableOperator(op.id)" matTooltip="Terminate Session">
                            <mat-icon>block</mat-icon>
                          </button>
                        }
                      </div>
                    </td>
                  </ng-container>

                  <tr mat-header-row *matHeaderRowDef="operatorColumns" class="!bg-slate-900/40"></tr>
                  <tr mat-row *matRowDef="let row; columns: operatorColumns;" class="hover:bg-slate-700/20 transition-all duration-200"></tr>
                </table>
              </mat-card>
              }
            </div>
          </mat-tab>

          <!-- Global Fleet -->
          <mat-tab>
            <ng-template mat-tab-label>
              <mat-icon class="mr-2">directions_bus</mat-icon> Global Fleet
            </ng-template>
            <div class="py-8">
              @if (loadingBuses) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-emerald-500/20 border-t-emerald-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading fleet data...</p>
                </div>
              } @else {
                <div class="mb-6">
                  <mat-form-field appearance="outline" class="!w-full !bg-slate-900">
                    <mat-label class="!text-slate-400">Search fleet</mat-label>
                    <input matInput [(ngModel)]="busSearchQuery" placeholder="Search by bus name or operator..." class="!text-white">
                    <mat-icon matPrefix class="!text-slate-500">search</mat-icon>
                  </mat-form-field>
                </div>

                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl overflow-hidden shadow-2xl p-0">
                  <table mat-table [dataSource]="filteredBuses" class="!bg-transparent w-full border-collapse">
                  <ng-container matColumnDef="name">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !w-1/4">Vehicle Entity</th>
                    <td mat-cell *matCellDef="let b" class="!text-white !border-slate-700 font-bold !py-4">
                      {{b.name}}
                      <p class="text-[10px] text-slate-500 uppercase tracking-widest font-mono">ID: {{b.id}}</p>
                    </td>
                  </ng-container>
                  <ng-container matColumnDef="operator">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !w-1/4">Assigned Operator</th>
                    <td mat-cell *matCellDef="let b" class="!text-slate-300 !border-slate-700 !py-4">{{b.operatorName}}</td>
                  </ng-container>
                  <ng-container matColumnDef="seats">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !w-1/6">Capacity</th>
                    <td mat-cell *matCellDef="let b" class="!text-emerald-400 !border-slate-700 font-mono font-bold">{{b.totalSeats}} Seats</td>
                  </ng-container>
                  <ng-container matColumnDef="status">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !w-1/6">Auth Status</th>
                    <td mat-cell *matCellDef="let b" class="!border-slate-700">
                      <div [class]="'inline-flex items-center px-3 py-1 rounded-full text-[10px] font-black uppercase ' + getStatusClass(b.status)">
                        <span class="w-1.5 h-1.5 rounded-full mr-2" [class]="getStatusDotClass(b.status)"></span>
                        {{b.status === 1 ? 'Approved' : 'Pending'}}
                      </div>
                    </td>
                  </ng-container>
                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 !text-right pr-6">Actions</th>
                    <td mat-cell *matCellDef="let b" class="!border-slate-700 text-right pr-6">
                      <div class="flex justify-end gap-2">
                        @if(b.status !== 1) {
                          <button mat-mini-fab color="primary" class="!bg-emerald-600" (click)="approveBus(b.id)">
                            <mat-icon>check_circle</mat-icon>
                          </button>
                        } @else {
                          <button mat-mini-fab color="warn" class="!bg-rose-600" (click)="disableBus(b.id)">
                            <mat-icon>block</mat-icon>
                          </button>
                        }
                      </div>
                    </td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="busColumns" class="!bg-slate-900/40"></tr>
                  <tr mat-row *matRowDef="let row; columns: busColumns;" class="hover:bg-slate-700/20 transition-all duration-200"></tr>
                </table>
                @if(buses.length === 0) {
                  <div class="p-12 text-center text-slate-500 border-t border-slate-700">
                    <mat-icon class="!w-12 !h-12 text-slate-700/50 block mx-auto mb-2" style="font-size: 48px">directions_bus</mat-icon>
                    No fleet assets registered in the system
                  </div>
                }
              </mat-card>
              }
            </div>
          </mat-tab>

          <!-- Schedule Requests -->
          <mat-tab>
            <ng-template mat-tab-label>
              <mat-icon class="mr-2">pending_actions</mat-icon> Schedule Requests
            </ng-template>
            <div class="py-8">
              @if (loadingSchedules) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-amber-500/20 border-t-amber-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading schedule requests...</p>
                </div>
              } @else {
                <div class="mb-6">
                  <mat-form-field appearance="outline" class="!w-full !bg-slate-900">
                    <mat-label class="!text-slate-400">Search schedule requests</mat-label>
                    <input matInput [(ngModel)]="scheduleSearchQuery" placeholder="Search by operator, route, or bus..." class="!text-white">
                    <mat-icon matPrefix class="!text-slate-500">search</mat-icon>
                  </mat-form-field>
                </div>

                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl overflow-hidden shadow-2xl p-0">
                  <table mat-table [dataSource]="filteredSchedules" class="!bg-transparent w-full">
                  <ng-container matColumnDef="operator">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Operator</th>
                    <td mat-cell *matCellDef="let s" class="!text-white !border-slate-700 font-bold !py-4">{{s.operatorName}}</td>
                  </ng-container>
                  <ng-container matColumnDef="route">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Transit Path</th>
                    <td mat-cell *matCellDef="let s" class="!text-slate-300 !border-slate-700">
                      <div class="flex flex-col">
                        <span class="font-bold text-indigo-400">{{s.route}}</span>
                        <span class="text-[10px] font-mono text-slate-500 uppercase">{{s.busName}}</span>
                      </div>
                    </td>
                  </ng-container>
                  <ng-container matColumnDef="departure">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Execution Time</th>
                    <td mat-cell *matCellDef="let s" class="!text-slate-300 !border-slate-700 font-mono">{{s.departureTime | date:'MMM d, h:mm a'}}</td>
                  </ng-container>
                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700 text-right pr-6">Decision</th>
                    <td mat-cell *matCellDef="let s" class="!border-slate-700 text-right pr-6">
                      <button mat-flat-button color="primary" class="!bg-emerald-600 !rounded-lg" (click)="approveSchedule(s.id)">
                        APPROVE TRIP
                      </button>
                    </td>
                  </ng-container>
                  <tr mat-header-row *matHeaderRowDef="pendingScheduleColumns" class="!bg-slate-900/40"></tr>
                  <tr mat-row *matRowDef="let row; columns: pendingScheduleColumns;" class="hover:bg-slate-700/20 transition-all duration-200"></tr>
                </table>
              </mat-card>
              }
            </div>
          </mat-tab>

          <!-- Routes -->
          <mat-tab>
            <ng-template mat-tab-label>
              <mat-icon class="mr-2">map</mat-icon> Routes
            </ng-template>
            <div class="py-8">
              @if (loadingRoutes) {
                <div class="flex flex-col items-center justify-center py-20">
                  <div class="w-12 h-12 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin mb-4"></div>
                  <p class="text-slate-500">Loading routes...</p>
                </div>
              } @else {
                <div class="flex justify-between items-center mb-6 px-2">
                  <h3 class="text-2xl font-bold tracking-tight">Active Topology</h3>
                  <button mat-flat-button class="!bg-indigo-600 !text-white !rounded-xl !px-6 !py-6" (click)="openRouteDialog()">
                    <mat-icon>add_location</mat-icon> Add New Route
                  </button>
                </div>

                <div class="mb-6">
                  <mat-form-field appearance="outline" class="!w-full !bg-slate-900">
                    <mat-label class="!text-slate-400">Search routes</mat-label>
                    <input matInput [(ngModel)]="routeSearchQuery" placeholder="Search by source or destination..." class="!text-white">
                    <mat-icon matPrefix class="!text-slate-500">search</mat-icon>
                  </mat-form-field>
                </div>

                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl overflow-hidden shadow-2xl p-0">
                  <table mat-table [dataSource]="filteredRoutes" class="!bg-transparent w-full">
                  <ng-container matColumnDef="id">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Node ID</th>
                    <td mat-cell *matCellDef="let r" class="!text-slate-500 !border-slate-700 font-mono">#{{r.id.toString().padStart(4, '0')}}</td>
                  </ng-container>

                  <ng-container matColumnDef="source">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Origin Node</th>
                    <td mat-cell *matCellDef="let r" class="!text-white !border-slate-700 font-bold uppercase tracking-tight">{{r.source}}</td>
                  </ng-container>

                  <ng-container matColumnDef="destination">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Destination Point</th>
                    <td mat-cell *matCellDef="let r" class="!text-white !border-slate-700 font-bold uppercase tracking-tight">{{r.destination}}</td>
                  </ng-container>

                  <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef class="!text-slate-400 !border-slate-700">Modifications</th>
                    <td mat-cell *matCellDef="let r" class="!border-slate-700 text-right pr-6">
                      <div class="flex justify-end gap-2">
                        <button mat-icon-button class="!text-slate-400" (click)="openRouteDialog(r)">
                          <mat-icon>edit_note</mat-icon>
                        </button>
                        <button mat-icon-button class="!text-slate-400" (click)="deleteRoute(r.id)">
                          <mat-icon>delete_forever</mat-icon>
                        </button>
                      </div>
                    </td>
                  </ng-container>

                  <tr mat-header-row *matHeaderRowDef="routeColumns" class="!bg-slate-900/40"></tr>
                  <tr mat-row *matRowDef="let row; columns: routeColumns;" class="hover:bg-slate-700/20 transition-all duration-200"></tr>
                </table>
              </mat-card>
              }
            </div>
          </mat-tab>

          <!-- Settings -->
          <mat-tab>
            <ng-template mat-tab-label>
              <mat-icon class="mr-2">tune</mat-icon> Fees
            </ng-template>
            <div class="py-8 max-w-2xl">
              <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-10 shadow-2xl backdrop-blur-xl border border-slate-800/50">
                <h3 class="text-2xl font-extrabold mb-8 flex items-center gap-3">
                  <span class="w-8 h-8 rounded bg-amber-500/20 flex items-center justify-center">
                    <mat-icon class="!text-amber-500">payments</mat-icon>
                  </span>
                  Economic Protocol
                </h3>
                
                <div class="p-8 bg-slate-900/80 rounded-2xl border border-slate-700/50">
                  <label class="block text-[10px] font-black text-slate-500 uppercase tracking-widest mb-4">Platform Convenience Fee</label>
                  <div class="flex gap-4 items-start">
                    <mat-form-field appearance="outline" class="flex-1 custom-dark-field !mb-0">
                      <span matPrefix class="px-3 text-slate-500 font-bold">₹</span>
                      <input matInput type="number" [(ngModel)]="convenienceFee">
                    </mat-form-field>
                    <button mat-flat-button class="!bg-emerald-600 !text-white h-[56px] !rounded-xl !px-8 font-bold shadow-lg shadow-emerald-500/20" 
                            (click)="updateFee()" [disabled]="loadingFee">
                      {{loadingFee ? 'Syncing...' : 'Deploy Config'}}
                    </button>
                  </div>
                </div>
              </mat-card>
            </div>
          </mat-tab>
        </mat-tab-group>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; background: #0f172a; }
    ::ng-deep .custom-tabs .mat-mdc-tab-header { border-bottom: 2px solid #1e293b; background: rgba(15, 23, 42, 0.3); }
    ::ng-deep .custom-tabs .mat-mdc-tab { color: #64748b !important; font-weight: 700 !important; height: 64px !important; text-transform: uppercase; letter-spacing: 0.1em; font-size: 12px !important; min-width: 120px !important; }
    ::ng-deep .custom-tabs .mat-mdc-tab.mdc-tab--active { color: #fff !important; }
    ::ng-deep .custom-tabs .mat-mdc-tab:not(.mdc-tab--active) .mat-mdc-tab-indicator {
      display: none !important;
    }
    ::ng-deep .custom-tabs .mat-mdc-tab.mdc-tab--active .mat-mdc-tab-indicator {
      height: 3px !important;
      background-color: #6366f1 !important;
    }
    ::ng-deep .custom-tabs .mat-mdc-tab-content { color: #94a3b8 !important; }
    ::ng-deep .custom-tabs .mat-mdc-tab-ripple { display: none !important; }
    ::ng-deep .custom-dark-field .mat-mdc-text-field-wrapper { background: rgba(15, 23, 42, 0.4) !important; }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-input-control { color: #fff !important; font-weight: 600 !important; }
  `]
})
export class AdminDashboardComponent implements OnInit {
  private adminService = inject(AdminService);
  private snack = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  private cdr = inject(ChangeDetectorRef);

  operators: OperatorSummary[] = [];
  routes: RouteDto[] = [];
  buses: BusSummary[] = [];
  pendingSchedules: any[] = [];
  convenienceFee = 0;
  loadingFee = false;
  loadingOperators = false;
  loadingRoutes = false;
  loadingBuses = false;
  loadingSchedules = false;
  operatorSearchQuery = '';
  busSearchQuery = '';
  scheduleSearchQuery = '';
  routeSearchQuery = '';
  private _selectedTabIndex = 0;

  get filteredOperators(): OperatorSummary[] {
    if (!this.operatorSearchQuery) return this.operators;
    const query = this.operatorSearchQuery.toLowerCase();
    return this.operators.filter(op =>
      op.name?.toLowerCase().includes(query) ||
      op.email?.toLowerCase().includes(query) ||
      op.headOfficeDistrict?.toLowerCase().includes(query)
    );
  }

  get filteredBuses(): BusSummary[] {
    if (!this.busSearchQuery) return this.buses;
    const query = this.busSearchQuery.toLowerCase();
    return this.buses.filter(bus =>
      bus.name?.toLowerCase().includes(query) ||
      bus.operatorName?.toLowerCase().includes(query)
    );
  }

  get filteredSchedules(): any[] {
    if (!this.scheduleSearchQuery) return this.pendingSchedules;
    const query = this.scheduleSearchQuery.toLowerCase();
    return this.pendingSchedules.filter(schedule =>
      schedule.operatorName?.toLowerCase().includes(query) ||
      schedule.route?.toLowerCase().includes(query) ||
      schedule.busName?.toLowerCase().includes(query)
    );
  }

  get filteredRoutes(): RouteDto[] {
    if (!this.routeSearchQuery) return this.routes;
    const query = this.routeSearchQuery.toLowerCase();
    return this.routes.filter(route =>
      route.source?.toLowerCase().includes(query) ||
      route.destination?.toLowerCase().includes(query)
    );
  }

  set selectedTabIndex(value: number) {
    this._selectedTabIndex = value;
    localStorage.setItem('adminTabIndex', value.toString());
  }
  get selectedTabIndex() {
    return this._selectedTabIndex;
  }

  operatorColumns = ['name', 'email', 'district', 'status', 'actions'];
  routeColumns = ['id', 'source', 'destination', 'actions'];
  busColumns = ['name', 'operator', 'seats', 'status', 'actions'];
  pendingScheduleColumns = ['operator', 'route', 'departure', 'actions'];

  ngOnInit() {
    // Restore tab state from localStorage
    const savedTabIndex = localStorage.getItem('adminTabIndex');
    if (savedTabIndex !== null) {
      this._selectedTabIndex = parseInt(savedTabIndex, 10);
    }
    this.refresh();
  }

  refresh() {
    this.loadOperators();
    this.loadRoutes();
    this.loadConfig();
    this.loadAllBuses();
    this.loadPendingSchedules();
  }

  loadOperators() {
    this.loadingOperators = true;
    this.adminService.getOperators().subscribe((res: any) => {
      this.operators = [...(res.data || [])];
      this.loadingOperators = false;
      this.cdr.detectChanges();
    });
  }

  loadRoutes() {
    this.loadingRoutes = true;
    this.adminService.getRoutes().subscribe((res: any) => {
      this.routes = [...(res.data || [])];
      this.loadingRoutes = false;
      this.cdr.detectChanges();
    });
  }

  loadAllBuses() {
    this.loadingBuses = true;
    this.adminService.getAllBuses().subscribe((res: any) => {
      this.buses = [...(res.data || [])];
      this.loadingBuses = false;
      this.cdr.detectChanges();
    });
  }

  loadPendingSchedules() {
    this.loadingSchedules = true;
    this.adminService.getPendingSchedules().subscribe((res: any) => {
      this.pendingSchedules = [...(res.data || [])];
      this.loadingSchedules = false;
      this.cdr.detectChanges();
    });
  }

  loadConfig() {
    this.adminService.getConfig().subscribe(res => {
      this.convenienceFee = parseFloat(res.data);
      this.cdr.detectChanges();
    });
  }

  approveOperator(id: number) {
    this.adminService.approveOperator(id).subscribe(() => {
      this.snack.open('Operator authorized', 'OK', { duration: 2000 });
      this.loadOperators();
    });
  }

  disableOperator(id: number) {
    if (confirm('Disable operator access?')) {
      this.adminService.disableOperator(id).subscribe(() => {
        this.snack.open('Operator disabled', 'OK', { duration: 2000 });
        this.loadOperators();
      });
    }
  }

  approveBus(id: number) {
    this.adminService.approveBus(id).subscribe(() => {
      this.snack.open('Bus approved for operations', 'OK', { duration: 2000 });
      this.loadAllBuses();
    });
  }

  disableBus(id: number) {
    this.adminService.disableBus(id).subscribe(() => {
      this.snack.open('Bus grounded', 'OK', { duration: 2000 });
      this.loadAllBuses();
    });
  }

  approveSchedule(id: number) {
    this.adminService.approveSchedule(id).subscribe(() => {
      this.snack.open('Schedule activated', 'OK', { duration: 2000 });
      this.loadPendingSchedules();
    });
  }

  viewOperatorDetails(op: any) {
    this.dialog.open(OperatorDetailsDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      data: { op },
      panelClass: 'dark-system-dialog'
    });
  }

  // ... rest of existing methods (deleteRoute, updateFee, openRouteDialog, helpers)
  deleteRoute(id: number) {
    if (confirm('Delete route?')) {
      this.adminService.deleteRoute(id).subscribe(() => {
        this.snack.open('Route removed', 'OK', { duration: 2000 });
        this.loadRoutes();
      });
    }
  }

  updateFee() {
    this.loadingFee = true;
    this.adminService.updateConvenienceFee(this.convenienceFee).subscribe(() => {
      this.snack.open('Fee updated', 'OK', { duration: 2000 });
      this.loadingFee = false;
    });
  }

  openRouteDialog(route?: any) {
    const dialogRef = this.dialog.open(RouteDialogComponent, {
      width: '450px',
      data: route ? { ...route } : { source: '', destination: '' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (route) {
          this.adminService.updateRoute(route.id, result.source, result.destination).subscribe(() => this.loadRoutes());
        } else {
          this.adminService.createRoute(result.source, result.destination).subscribe(() => this.loadRoutes());
        }
      }
    });
  }

  getStatusClass(status: string | number): string {
    const s = Number(status);
    if (s === 1) return 'bg-emerald-500/10 text-emerald-500 border border-emerald-500/20';
    if (s === 0) return 'bg-amber-500/10 text-amber-500 border border-amber-500/20';
    return 'bg-rose-500/10 text-rose-500 border border-rose-500/20';
  }

  getStatusDotClass(status: string | number): string {
    const s = Number(status);
    if (s === 1) return 'bg-emerald-500 shadow-[0_0_8px_rgba(16,185,129,0.6)]';
    if (s === 0) return 'bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.6)] animate-pulse';
    return 'bg-rose-500 shadow-[0_0_8px_rgba(244,63,94,0.6)]';
  }
}

