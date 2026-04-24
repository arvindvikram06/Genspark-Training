import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule, MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { AuthService } from '../../../core/services/auth.service';
import { OperatorService } from '../../../core/services/operator.service';

// Inline Create Schedule Dialog
@Component({
  selector: 'app-create-schedule-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatDividerModule, MatDialogModule],
  template: `
    <div class="bg-slate-900 text-white w-full max-w-3xl max-h-[90vh] overflow-y-auto flex flex-col">
      <!-- Header -->
      <div class="flex justify-between items-center px-10 py-8 border-b border-slate-800">
        <div>
          <h2 class="text-3xl font-bold flex items-center gap-3">
            <mat-icon class="text-emerald-500 text-3xl">add_road</mat-icon> Plan New Trip
          </h2>
          <p class="text-slate-400 text-sm mt-2">Deploy a vehicle to an active route and start accepting bookings.</p>
        </div>
        <button mat-icon-button mat-dialog-close class="text-slate-500 hover:text-white hover:bg-slate-800 rounded-lg"><mat-icon>close</mat-icon></button>
      </div>

      <!-- Form Content -->
      <div class="flex-1 px-10 py-10 overflow-y-auto">
        <form [formGroup]="scheduleForm" (ngSubmit)="saveSchedule()" class="space-y-10">
          
          <!-- Section 1: Vehicle & Route Selection -->
          <div>
            <h3 class="text-xs font-bold text-emerald-500 uppercase tracking-widest mb-6">Step 1: Select Asset & Route</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <div class="space-y-3">
                <label class="text-sm font-semibold text-white block">Select Vehicle</label>
                <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                  <mat-label>Choose from your fleet</mat-label>
                  <mat-select formControlName="busId">
                    @for(bus of data.buses; track bus.id) {
                      <mat-option [value]="bus.id" [disabled]="bus.status !== 1">
                        {{bus.name}} {{bus.status !== 1 ? '(Not approved)' : ''}}
                      </mat-option>
                    }
                  </mat-select>
                  <mat-icon matPrefix class="mr-2 text-slate-500">directions_bus</mat-icon>
                </mat-form-field>
              </div>

              <div class="space-y-3">
                <label class="text-sm font-semibold text-white block">Assigned Route</label>
                <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                  <mat-label>Select destination route</mat-label>
                  <mat-select formControlName="routeId">
                    @for(route of routes; track route.id) {
                      <mat-option [value]="route.id">{{route.source | titlecase}} → {{route.destination | titlecase}}</mat-option>
                    }
                  </mat-select>
                  <mat-icon matPrefix class="mr-2 text-slate-500">near_me</mat-icon>
                </mat-form-field>
              </div>
            </div>
          </div>

          <div class="border-t border-slate-800"></div>

          <!-- Section 2: Departure & Arrival Times -->
          <div>
            <h3 class="text-xs font-bold text-blue-500 uppercase tracking-widest mb-6">Step 2: Schedule Timeline</h3>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
              <!-- Departure -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-white">Departure</h4>
                <div class="space-y-3">
                  <div>
                    <label class="text-xs text-slate-400 font-medium block mb-2">Date</label>
                    <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                      <mat-label>Select date</mat-label>
                      <input matInput [matDatepicker]="depDatePicker" formControlName="depDate" (click)="depDatePicker.open()" readonly>
                      <mat-datepicker-toggle matIconSuffix [for]="depDatePicker"></mat-datepicker-toggle>
                      <mat-datepicker #depDatePicker></mat-datepicker>
                      <mat-icon matPrefix class="mr-2 text-slate-500">calendar_today</mat-icon>
                    </mat-form-field>
                  </div>
                  <div>
                    <label class="text-xs text-slate-400 font-medium block mb-2">Time</label>
                    <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                      <mat-label>Departure time</mat-label>
                      <input matInput type="time" formControlName="depTime" (change)="logFieldChange('depTime')">
                      <mat-icon matPrefix class="mr-2 text-slate-500">schedule</mat-icon>
                    </mat-form-field>
                  </div>
                </div>
              </div>

              <!-- Arrival -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-white">Arrival</h4>
                <div class="space-y-3">
                  <div>
                    <label class="text-xs text-slate-400 font-medium block mb-2">Date</label>
                    <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                      <mat-label>Select date</mat-label>
                      <input matInput [matDatepicker]="arrDatePicker" formControlName="arrDate" (click)="arrDatePicker.open()" readonly>
                      <mat-datepicker-toggle matIconSuffix [for]="arrDatePicker"></mat-datepicker-toggle>
                      <mat-datepicker #arrDatePicker></mat-datepicker>
                      <mat-icon matPrefix class="mr-2 text-slate-500">event</mat-icon>
                    </mat-form-field>
                  </div>
                  <div>
                    <label class="text-xs text-slate-400 font-medium block mb-2">Time</label>
                    <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                      <mat-label>Arrival time</mat-label>
                      <input matInput type="time" formControlName="arrTime" (change)="logFieldChange('arrTime')">
                      <mat-icon matPrefix class="mr-2 text-slate-500">update</mat-icon>
                    </mat-form-field>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="border-t border-slate-800"></div>

          <!-- Section 3: Pricing -->
          <div>
            <h3 class="text-xs font-bold text-purple-500 uppercase tracking-widest mb-6">Step 3: Fare Configuration</h3>
            <div class="space-y-3">
              <label class="text-sm font-semibold text-white block">Price Per Seat</label>
              <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                <mat-label>Enter price in rupees (₹)</mat-label>
                <input type="number" matInput formControlName="pricePerSeat" placeholder="e.g., 650">
                <mat-icon matPrefix class="mr-2 text-slate-500">payments</mat-icon>
              </mat-form-field>
            </div>
          </div>

          <div class="border-t border-slate-800"></div>

          <!-- Info Box -->
          <div class="p-6 bg-gradient-to-r from-emerald-500/10 to-cyan-500/10 border border-emerald-500/20 rounded-2xl">
            <div class="flex items-start gap-4">
              <mat-icon class="text-emerald-500 text-2xl flex-shrink-0 mt-1">lightbulb</mat-icon>
              <div>
                <p class="text-sm font-semibold text-white mb-1">Auto-Resolution</p>
                <p class="text-xs text-slate-300 leading-relaxed">Boarding and Drop points will be automatically resolved from your registered office addresses in the selected route's source and destination districts.</p>
              </div>
            </div>
          </div>
        </form>
      </div>

      <!-- Footer with Actions -->
      <div class="px-10 py-8 border-t border-slate-800 flex justify-end gap-4 bg-slate-950/50">
        <button type="button" mat-button mat-dialog-close class="!text-slate-400 hover:!text-white">Cancel</button>
        <button type="submit" (click)="saveSchedule()" mat-flat-button class="!bg-gradient-to-r !from-emerald-600 !to-emerald-500 !text-white !px-8 !py-2.5 !rounded-xl !font-semibold" [disabled]="scheduleForm.invalid || loading">
          <mat-icon class="mr-2 text-lg">{{ loading ? 'hourglass_empty' : 'check_circle' }}</mat-icon>
          {{ loading ? 'Publishing...' : 'Publish Schedule' }}
        </button>
      </div>
    </div>
  `
})
export class CreateScheduleDialogComponent implements OnInit {
  fb = inject(FormBuilder);
  operatorService = inject(OperatorService);
  snack = inject(MatSnackBar);
  dialogRef = inject(MatDialogRef<CreateScheduleDialogComponent>);
  cdr = inject(ChangeDetectorRef);
  data = inject(MAT_DIALOG_DATA);

  scheduleForm: FormGroup;
  routes: any[] = [];
  loading = false;

  constructor() {
    this.scheduleForm = this.fb.group({
      busId: ['', Validators.required],
      routeId: ['', Validators.required],
      depDate: ['', Validators.required],
      depTime: ['06:00', Validators.required],
      arrDate: ['', Validators.required],
      arrTime: ['10:00', Validators.required],
      pricePerSeat: ['', [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit() {
    this.operatorService.getRoutes().subscribe(res => {
      this.routes = res.data;
      this.cdr.detectChanges();
    });

    // Diagnostic Logs
    this.scheduleForm.statusChanges.subscribe(status => {
      console.log('--- Schedule Form Status Update ---');
      console.log('Current Status:', status);
      console.log('Form Values:', this.scheduleForm.value);
      console.log('Form Errors:', this.getFormErrors());
    });
  }

  getFormErrors() {
    const errors: any = {};
    Object.keys(this.scheduleForm.controls).forEach(key => {
      const controlErrors = this.scheduleForm.get(key)?.errors;
      if (controlErrors) errors[key] = controlErrors;
    });
    return errors;
  }

  logFieldChange(field: string) {
    console.log(`[Field Change] ${field}:`, this.scheduleForm.get(field)?.value);
  }

  saveSchedule() {
    if (this.scheduleForm.invalid) return;
    this.loading = true;

    const val = this.scheduleForm.value;

    // Combine Date and Time
    const departureStr = `${this.formatDate(val.depDate)}T${val.depTime}:00`;
    const arrivalStr = `${this.formatDate(val.arrDate)}T${val.arrTime}:00`;

    const payload = {
      busId: val.busId,
      routeId: val.routeId,
      departureTime: departureStr,
      arrivalTime: arrivalStr,
      pricePerSeat: val.pricePerSeat
    };

    console.log('--- Submitting Schedule Payload ---');
    console.log(JSON.stringify(payload, null, 2));

    this.operatorService.createSchedule(payload).subscribe({
      next: () => {
        this.snack.open('Trip Schedule Published', 'OK', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err: any) => {
        const msg = err.error?.message || err.error || 'Failed to publish schedule';
        this.snack.open(msg, 'Error', { duration: 5000 });
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private formatDate(date: Date): string {
    const d = new Date(date);
    return d.toISOString().split('T')[0];
  }
}

// Inline Office Management Dialog
@Component({
  selector: 'app-office-management-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatDividerModule, MatDialogModule],
  template: `
    <div class="p-8 bg-slate-900 text-white w-full max-w-2xl max-h-[85vh] overflow-y-auto">
      <div class="flex justify-between items-center mb-8">
        <div>
          <h2 class="text-2xl font-bold flex items-center gap-3">
            <mat-icon class="text-amber-500">business</mat-icon> Manage Service Centers
          </h2>
          <p class="text-slate-500 text-sm">Register your branch offices to enable boarding point resolution.</p>
        </div>
        <button mat-icon-button mat-dialog-close class="text-slate-500 hover:text-white"><mat-icon>close</mat-icon></button>
      </div>

      <form [formGroup]="officeForm" (ngSubmit)="saveOffices()" class="space-y-6">
        <div formArrayName="offices" class="space-y-4">
          @for(office of offices.controls; track $index) {
            <div [formGroupName]="$index" class="p-6 bg-slate-800/20 border border-slate-700/50 rounded-3xl relative group hover:border-amber-500/30 transition-colors">
              <button type="button" (click)="removeOffice($index)" class="absolute top-4 right-4 text-slate-600 hover:text-rose-500 opacity-0 group-hover:opacity-100 transition-opacity">
                <mat-icon>delete</mat-icon>
              </button>
              
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div class="space-y-2">
                  <label class="text-[10px] font-mono text-slate-500 uppercase tracking-widest ml-1">Geographic Region</label>
                  <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                    <mat-label>Select District</mat-label>
                    <mat-select formControlName="district">
                      @for(d of districts; track d.id) {
                        <mat-option [value]="d.name">{{d.name}}</mat-option>
                      }
                    </mat-select>
                    <mat-icon matPrefix class="mr-2 text-slate-500">map</mat-icon>
                  </mat-form-field>
                </div>

                <div class="space-y-2">
                  <label class="text-[10px] font-mono text-slate-500 uppercase tracking-widest ml-1">Contact Point</label>
                  <mat-form-field appearance="outline" class="w-full h-auto custom-dark-field">
                    <mat-label>Branch Address</mat-label>
                    <textarea matInput formControlName="address" rows="1" placeholder="e.g., Main St Depot"></textarea>
                    <mat-icon matPrefix class="mr-2 text-slate-500">location_on</mat-icon>
                  </mat-form-field>
                </div>
              </div>
            </div>
          }
        </div>

        <button type="button" mat-stroked-button (click)="addOffice()" class="w-full !py-6 !border-slate-700 !text-slate-400 hover:!border-amber-500/50 hover:!text-amber-500 !rounded-2xl border-2 border-dashed">
          <mat-icon class="mr-2">add_location</mat-icon> Add New Service Center
        </button>

        <div class="pt-6 flex justify-end gap-4 border-t border-slate-800">
          <button type="button" mat-button mat-dialog-close class="!text-slate-400">Cancel</button>
          <button type="submit" mat-flat-button class="!bg-amber-600 !text-white !px-8 !rounded-xl" [disabled]="officeForm.invalid || loading">
            {{ loading ? 'Saving Protocol...' : 'Save Configuration' }}
          </button>
        </div>
      </form>
    </div>
  `
})
export class OfficeManagementDialogComponent implements OnInit {
  fb = inject(FormBuilder);
  operatorService = inject(OperatorService);
  snack = inject(MatSnackBar);
  dialogRef = inject(MatDialogRef<OfficeManagementDialogComponent>);
  data = inject(MAT_DIALOG_DATA);

  officeForm: FormGroup;
  districts: any[] = [];
  loading = false;

  constructor() {
    this.officeForm = this.fb.group({
      offices: this.fb.array([])
    });
  }

  get offices() { return this.officeForm.get('offices') as FormArray; }

  ngOnInit() {
    this.operatorService.getDistricts().subscribe(res => this.districts = res.data);

    if (this.data.offices && this.data.offices.length > 0) {
      this.data.offices.forEach((o: any) => this.addOffice(o.district, o.address));
    } else {
      this.addOffice();
    }
  }

  addOffice(district = '', address = '') {
    this.offices.push(this.fb.group({
      district: [district, Validators.required],
      address: [address, Validators.required]
    }));
  }

  removeOffice(index: number) { this.offices.removeAt(index); }

  saveOffices() {
    if (this.officeForm.invalid) return;
    this.loading = true;
    this.operatorService.submitOffices(this.officeForm.value.offices).subscribe({
      next: () => {
        this.snack.open('Office Configuration Saved', 'OK', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: () => {
        this.snack.open('Failed to save configuration', 'Error', { duration: 3000 });
        this.loading = false;
      }
    });
  }
}

// Inline Add Bus Dialog with Layout Designer
@Component({
  selector: 'app-add-bus-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatDialogModule, MatTooltipModule],
  template: `
    <div class="p-8 bg-slate-900 text-white w-full max-w-4xl max-h-[90vh] overflow-y-auto">
      <div class="flex justify-between items-center mb-8 border-b border-slate-800 pb-6">
        <div>
          <h2 class="text-3xl font-black flex items-center gap-3">
            <mat-icon class="text-indigo-500 scale-125">directions_bus</mat-icon> Register Vehicle
          </h2>
          <p class="text-slate-500 text-sm mt-1">Configure your vehicle specs and seating topology.</p>
        </div>
        <button mat-icon-button mat-dialog-close class="text-slate-500 hover:text-white"><mat-icon>close</mat-icon></button>
      </div>

      <form [formGroup]="busForm" (ngSubmit)="saveBus()" class="flex flex-col lg:flex-row gap-8 h-full max-h-[75vh]">
        
        <!-- Control Workspace -->
        <div class="lg:w-[32%] space-y-8 bg-slate-900/50 p-8 rounded-[32px] border border-slate-800/50 flex flex-col">
          <div class="space-y-6 flex-1">
            <div class="space-y-4">
              <label class="text-[11px] font-bold text-slate-500 uppercase tracking-[0.2em] ml-1">Vehicle Identity</label>
              <mat-form-field appearance="outline" class="w-full custom-dark-field">
                <mat-label>Display Name</mat-label>
                <input matInput formControlName="name" placeholder="e.g., Luxury Express">
                <mat-icon matPrefix class="mr-3 text-indigo-400">badge</mat-icon>
              </mat-form-field>
            </div>

            <div class="space-y-4 pt-4 border-t border-slate-800/50">
              <label class="text-[11px] font-bold text-slate-500 uppercase tracking-[0.2em] ml-1">Fleet Configuration</label>
              <div class="space-y-6">
                <mat-form-field appearance="outline" class="w-full custom-dark-field">
                  <mat-label>Passenger Capacity</mat-label>
                  <input type="number" matInput formControlName="totalSeats" (input)="generatePreview()">
                  <mat-icon matPrefix class="mr-3 text-indigo-400">group</mat-icon>
                </mat-form-field>

                <mat-form-field appearance="outline" class="w-full custom-dark-field">
                  <mat-label>Interior Architecture</mat-label>
                  <mat-select formControlName="layoutType" (selectionChange)="generatePreview()">
                    <mat-option value="2x2">2 x 2 High Capacity</mat-option>
                    <mat-option value="2x1">2 x 1 Premium Cabin</mat-option>
                  </mat-select>
                  <mat-icon matPrefix class="mr-3 text-indigo-400">grid_view</mat-icon>
                </mat-form-field>
              </div>
            </div>
          </div>
          
          <div class="pt-6">
            <button type="submit" mat-flat-button class="w-full !bg-indigo-600 !text-white !h-16 !rounded-2xl !text-sm !font-black !shadow-2xl !shadow-indigo-500/20 hover:!bg-indigo-500 transition-all uppercase tracking-widest" [disabled]="busForm.invalid || loading">
              {{ loading ? 'Synchronizing...' : 'Authorize Vehicle' }}
            </button>
          </div>
        </div>

        <!-- Interior Blueprint Viewport -->
        <div class="lg:w-[68%] bg-slate-950 rounded-[40px] p-0 border border-slate-800 relative flex flex-col overflow-hidden">
          <div class="absolute -right-20 -top-20 w-80 h-80 bg-indigo-500/5 blur-[120px] rounded-full"></div>
          
          <div class="p-8 border-b border-slate-800/50 bg-slate-950/80 backdrop-blur-md relative z-20 flex justify-between items-center">
            <h3 class="text-[10px] font-mono text-slate-500 uppercase tracking-widest flex items-center gap-3">
              <span class="w-1.5 h-1.5 rounded-full bg-emerald-500 shadow-[0_0_15px_rgba(16,185,129,0.5)] animate-pulse"></span>
              Live Architecture Preview
            </h3>
            <span class="text-[10px] font-mono text-indigo-400 bg-indigo-500/10 px-3 py-1 rounded-full border border-indigo-500/20 uppercase">
               Active Cluster: {{ busForm.value.layoutType }}
            </span>
          </div>

          <!-- Dynamic Scroller -->
          <div class="flex-1 overflow-y-auto p-12 custom-scrollbar relative z-10">
             <div class="bus-body p-12 border-[2px] border-slate-800 rounded-[64px] bg-slate-900/10 backdrop-blur-sm mx-auto w-fit min-w-[400px]">
                <!-- Dash Component -->
                <div class="flex justify-between items-center mb-16 pb-10 border-b border-slate-800/30">
                  <div class="w-16 h-16 bg-slate-800 rounded-2xl flex items-center justify-center shadow-inner border border-slate-700/30">
                    <mat-icon class="text-indigo-400 opacity-60">airline_seat_recline_extra</mat-icon>
                  </div>
                  <div class="w-24 h-2 bg-slate-800/50 rounded-full"></div>
                </div>

                <!-- Seat Grid -->
                <div class="space-y-4">
                  @for(row of previewRows; track row.id) {
                    <div class="flex justify-center items-center gap-4">
                      <!-- Group Alpha -->
                      <div class="flex gap-2.5">
                        <div class="seat-node" [class.invisible]="!row.seats[0]">{{row.seats[0]}}</div>
                        <div class="seat-node" [class.invisible]="!row.seats[1]">{{row.seats[1]}}</div>
                      </div>
                      
                      <!-- Aisle Corridor -->
                      <div class="w-12 h-10 flex justify-center items-center">
                        <div class="h-6 w-[1.5px] bg-slate-800/40 rounded-full"></div>
                      </div>
                      
                      <!-- Group Beta -->
                      <div class="flex gap-2.5">
                        @if (busForm.value.layoutType === '2x2') {
                           <div class="seat-node" [class.invisible]="!row.seats[2]">{{row.seats[2]}}</div>
                           <div class="seat-node" [class.invisible]="!row.seats[3]">{{row.seats[3]}}</div>
                        } @else {
                           <div class="seat-node" [class.invisible]="!row.seats[2]">{{row.seats[2]}}</div>
                           <div class="w-[42px]"></div> <!-- Spacer for 2x1 -->
                        }
                      </div>
                    </div>
                  }
                </div>
             </div>
          </div>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .seat-node {
      width: 48px;
      height: 48px;
      background: linear-gradient(145deg, #1e293b, #0f172a);
      border: 1px solid theme('colors.slate.700');
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: theme('colors.slate.400');
      font-size: 12px;
      font-weight: 800;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      cursor: crosshair;
    }
    .seat-node:hover {
      background: theme('colors.indigo.600');
      color: white;
      border-color: theme('colors.indigo.400');
      transform: scale(1.1);
      box-shadow: 0 10px 25px -5px rgba(79, 70, 229, 0.4);
      z-index: 20;
    }
    .custom-scrollbar::-webkit-scrollbar { width: 5px; }
    .custom-scrollbar::-webkit-scrollbar-track { background: rgba(30, 41, 59, 0.5); border-radius: 10px; }
    .custom-scrollbar::-webkit-scrollbar-thumb { background: theme('colors.slate.700'); border-radius: 10px; }

    ::ng-deep .custom-dark-field .mdc-text-field--outlined {
      background-color: #0f172a !important;
      border-radius: 20px !important;
      min-height: 72px !important;
    }
    ::ng-deep .custom-dark-field .mdc-notched-outline__leading,
    ::ng-deep .custom-dark-field .mdc-notched-outline__notch,
    ::ng-deep .custom-dark-field .mdc-notched-outline__trailing {
      border-color: #1e293b !important;
    }
    ::ng-deep .custom-dark-field .mdc-text-field--focused .mdc-notched-outline__leading,
    ::ng_deep .custom-dark-field .mdc-text-field--focused .mdc-notched-outline__notch,
    ::ng_deep .custom-dark-field .mdc-text-field--focused .mdc-notched-outline__trailing {
      border-color: theme('colors.indigo.500') !important;
    }
    ::ng-deep .custom-dark-field .mdc-text-field--outlined .mdc-text-field__input {
      color: white !important;
      padding-top: 24px !important;
      font-weight: 600 !important;
      font-size: 16px !important;
    }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-label-wrapper {
      top: 22px !important;
      font-size: 15px !important;
      color: #94a3b8 !important;
    }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-infix {
      min-height: 72px !important;
      display: flex !important;
      align-items: center !important;
    }
    ::ng-deep .custom-dark-field .mat-mdc-form-field-prefix {
      padding-top: 10px !important;
    }
  `]
})
export class AddBusDialogComponent implements OnInit {
  fb = inject(FormBuilder);
  operatorService = inject(OperatorService);
  snack = inject(MatSnackBar);
  dialogRef = inject(MatDialogRef<AddBusDialogComponent>);
  cdr = inject(ChangeDetectorRef);

  busForm: FormGroup;
  loading = false;
  previewRows: any[] = [];

  constructor() {
    this.busForm = this.fb.group({
      name: ['', Validators.required],
      totalSeats: [40, [Validators.required, Validators.min(10), Validators.max(60)]],
      layoutType: ['2x2', Validators.required]
    });
  }

  ngOnInit() {
    this.generatePreview();
    this.cdr.detectChanges();
  }

  generatePreview() {
    const total = this.busForm.value.totalSeats;
    const layout = this.busForm.value.layoutType;
    const seatsPerRow = layout === '2x2' ? 4 : 3;
    const rowCount = Math.ceil(total / seatsPerRow);

    this.previewRows = [];
    const labels = ['A', 'B', 'C', 'D', 'E'];
    let seatCount = 0;

    for (let i = 1; i <= rowCount; i++) {
      const rowSeats = [];
      for (let j = 0; j < seatsPerRow; j++) {
        if (seatCount < total) {
          rowSeats.push(`${i}${labels[j]}`);
          seatCount++;
        } else {
          rowSeats.push(null);
        }
      }
      this.previewRows.push({ id: i, seats: rowSeats });
    }
  }

  saveBus() {
    if (this.busForm.invalid) return;
    this.loading = true;

    // Build the SeatLayout JSON
    const layout = this.busForm.value.layoutType;
    const total = this.busForm.value.totalSeats;
    const seatsPerRow = layout === '2x2' ? 4 : 3;
    const rowCount = Math.ceil(total / seatsPerRow);
    const labels = ['A', 'B', 'C', 'D', 'E'];

    const seatsArray = [];
    for (let i = 1; i <= rowCount; i++) {
      for (let j = 0; j < seatsPerRow; j++) {
        if (seatsArray.length < total) {
          seatsArray.push({
            row: i,
            col: j + 1,
            seatNumber: `${i}${labels[j]}`
          });
        }
      }
    }

    const payload = {
      name: this.busForm.value.name,
      totalSeats: total,
      seatLayout: {
        rows: rowCount,
        cols: layout === '2x2' ? 2 : 2, // simplified per user's 2 column request
        seats: seatsArray
      }
    };

    this.operatorService.createBus(payload).subscribe({
      next: () => {
        this.snack.open('Vehicle Registered for Review', 'OK', { duration: 3000 });
        this.dialogRef.close(true);
      },
      error: (err: any) => {
        const msg = err.error?.message || err.error || 'Failed to register vehicle';
        this.snack.open(msg, 'Error', { duration: 5000 });
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}

@Component({
  selector: 'app-operator-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatTabsModule,
    MatTableModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  template: `
    <div class="min-h-screen bg-slate-950 text-slate-200">
      <!-- Header -->
      <header class="bg-slate-900/50 border-b border-slate-800 backdrop-blur-md sticky top-0 z-50">
        <div class="max-w-7xl mx-auto px-6 h-20 flex justify-between items-center">
          <div class="flex items-center gap-4">
            <div class="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center shadow-lg shadow-indigo-500/20">
              <mat-icon class="text-white">hub</mat-icon>
            </div>
            <div>
              <h1 class="text-xl font-bold text-white tracking-tight">Operator Command</h1>
              <p class="text-xs text-slate-500 font-mono uppercase tracking-widest">{{ userName }} // ONLINE</p>
            </div>
          </div>
        </div>
      </header>

      <div class="max-w-7xl mx-auto px-6 py-8">
        @if(loading) {
          <div class="flex flex-col items-center justify-center py-32">
            <div class="w-16 h-16 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin mb-6"></div>
            <p class="text-slate-500 font-medium animate-pulse">Synchronizing fleet data...</p>
          </div>
        } @else if (operatorStatus === 1) {
          <mat-tab-group class="dark-tabs">
            <!-- Summary Tab -->
            <mat-tab>
              <ng-template mat-tab-label>
                <mat-icon class="mr-2">dashboard</mat-icon> Overview
              </ng-template>
              <div class="py-8">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                  <mat-card class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl">
                    <p class="text-slate-500 text-sm font-bold uppercase mb-2">Total Revenue</p>
                    <h2 class="text-4xl font-black text-white">₹0</h2>
                  </mat-card>
                  <mat-card class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl">
                    <p class="text-slate-500 text-sm font-bold uppercase mb-2">Fleet Strength</p>
                    <h2 class="text-4xl font-black text-white">0</h2>
                  </mat-card>
                  <mat-card class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl">
                    <p class="text-slate-500 text-sm font-bold uppercase mb-2">Active Trips</p>
                    <h2 class="text-4xl font-black text-white">0</h2>
                  </mat-card>
                </div>
                <!-- Quick Actions -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div class="p-8 border-2 border-dashed border-slate-800 rounded-3xl flex flex-col items-center justify-center text-center hover:border-indigo-500/50 transition-all cursor-pointer group">
                    <div class="w-16 h-16 bg-indigo-500/10 text-indigo-500 rounded-2xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                      <mat-icon style="font-size: 32px; width: 32px; height: 32px;">add_bus</mat-icon>
                    </div>
                    <h3 class="text-xl font-bold text-white mb-2">Register New Vehicle</h3>
                    <p class="text-slate-500 text-sm max-w-xs">Add a bus to your fleet and configure its custom seating layout.</p>
                  </div>
                  <div class="p-8 border-2 border-dashed border-slate-800 rounded-3xl flex flex-col items-center justify-center text-center hover:border-emerald-500/50 transition-all cursor-pointer group">
                    <div class="w-16 h-16 bg-emerald-500/10 text-emerald-500 rounded-2xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                      <mat-icon style="font-size: 32px; width: 32px; height: 32px;">add_road</mat-icon>
                    </div>
                    <h3 class="text-xl font-bold text-white mb-2">Plan a New Trip</h3>
                    <p class="text-slate-500 text-sm max-w-xs">Schedule a bus for an approved route and start accepting bookings.</p>
                  </div>
                </div>
              </div>
            </mat-tab>

            <!-- Fleet Tab -->
            <mat-tab>
              <ng-template mat-tab-label>
                <mat-icon class="mr-2">directions_bus</mat-icon> My Fleet
              </ng-template>
              <div class="py-8">
                <div class="flex justify-between items-center mb-6">
                  <h2 class="text-2xl font-bold text-white">Vehicle Inventory</h2>
                  <button mat-flat-button color="primary" (click)="addBus()" class="!rounded-xl !bg-indigo-600 !px-6"> + Add Bus </button>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  @for(bus of operatorBuses; track bus.id) {
                    <mat-card class="!bg-slate-900 !border-slate-800 !p-0 !rounded-2xl overflow-hidden group">
                      <div class="p-6">
                        <div class="flex justify-between items-start mb-4">
                          <div>
                            <h3 class="text-lg font-bold text-white mb-1">{{bus.name}}</h3>
                            <span [class]="'px-2 py-0.5 rounded text-[10px] font-bold uppercase ' + (bus.status === 1 ? 'bg-emerald-500/10 text-emerald-500' : 'bg-amber-500/10 text-amber-500')">
                              {{bus.status === 1 ? 'Operational' : 'Pending Review'}}
                            </span>
                          </div>
                          <div class="w-10 h-10 bg-slate-800 rounded-lg flex items-center justify-center text-slate-500">
                            <mat-icon>directions_bus</mat-icon>
                          </div>
                        </div>
                        
                        <div class="flex items-center gap-4 text-sm text-slate-400 mb-6">
                          <div class="flex items-center gap-1.5 italic">
                            <mat-icon class="text-xs">event_seat</mat-icon> {{bus.totalSeats}} Seats
                          </div>
                          <div class="flex items-center gap-1.5 italic">
                            <mat-icon class="text-xs">calendar_today</mat-icon> {{bus.createdAt | date:'shortDate'}}
                          </div>
                        </div>

                        <div class="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                          <button mat-button class="w-full !bg-slate-800 !text-rose-500 !rounded-lg" (click)="deleteBus(bus.id)">
                            <mat-icon class="mr-2">delete</mat-icon> Remove
                          </button>
                        </div>
                      </div>
                    </mat-card>
                  }
                  @if(operatorBuses.length === 0) {
                    <div class="col-span-full py-20 text-center border-2 border-dashed border-slate-800 rounded-3xl">
                      <mat-icon class="text-slate-700 w-16 h-16 text-[64px] mb-4">no_bus</mat-icon>
                      <p class="text-slate-500 font-medium">No vehicles registered yet.</p>
                      <button mat-button color="primary" (click)="addBus()" class="mt-4">Register First Bus</button>
                    </div>
                  }
                </div>
              </div>
            </mat-tab>

            <!-- Schedules Tab -->
            <mat-tab>
              <ng-template mat-tab-label>
                <mat-icon class="mr-2">event_note</mat-icon> Trip Schedules
              </ng-template>
              <div class="py-8">
                <div class="flex justify-between items-center mb-6">
                  <h2 class="text-2xl font-bold text-white">Active Operations</h2>
                  <button mat-flat-button color="primary" (click)="addSchedule()" class="!bg-emerald-600 !text-white !rounded-xl !px-6"> Schedule Trip </button>
                </div>
                
                <mat-card class="!bg-slate-900 !border-slate-800 !rounded-2xl p-0 overflow-hidden">
                  <table mat-table [dataSource]="operatorSchedules" class="!bg-transparent w-full">
                    <ng-container matColumnDef="bus">
                      <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !font-bold !py-6 uppercase text-xs">Vehicle</th>
                      <td mat-cell *matCellDef="let s" class="!text-slate-300">
                        <div class="font-bold text-white">{{s.busName}}</div>
                        <div class="text-[10px] text-slate-500 font-mono tracking-tighter">ID: {{s.id}}</div>
                      </td>
                    </ng-container>

                    <ng-container matColumnDef="route">
                      <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !font-bold !py-6 uppercase text-xs">Pathway</th>
                      <td mat-cell *matCellDef="let s" class="!text-slate-300">
                        <div class="flex items-center gap-2">
                          <span class="px-2 py-0.5 bg-slate-800 rounded text-[10px] font-bold">{{s.source}}</span>
                          <mat-icon class="text-[14px] !w-4 !h-4 text-slate-600">arrow_forward</mat-icon>
                          <span class="px-2 py-0.5 bg-slate-800 rounded text-[10px] font-bold">{{s.destination}}</span>
                        </div>
                      </td>
                    </ng-container>

                    <ng-container matColumnDef="time">
                      <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !font-bold !py-6 uppercase text-xs">Chronology</th>
                      <td mat-cell *matCellDef="let s" class="!text-slate-300">
                        <div class="flex flex-col">
                          <span class="text-xs font-bold text-emerald-500">DEP: {{s.departureTime | date:'MMM d, HH:mm'}}</span>
                          <span class="text-xs text-slate-500">ARR: {{s.arrivalTime | date:'MMM d, HH:mm'}}</span>
                        </div>
                      </td>
                    </ng-container>

                    <ng-container matColumnDef="status">
                      <th mat-header-cell *matHeaderCellDef class="!text-slate-500 !font-bold !py-6 uppercase text-xs">Status</th>
                      <td mat-cell *matCellDef="let s">
                        <span [class]="'px-3 py-1 rounded-full text-[10px] font-bold ' + 
                          (s.status === 1 ? 'bg-emerald-500/10 text-emerald-500' : 
                           s.status === 0 ? 'bg-amber-500/10 text-amber-500' : 'bg-rose-500/10 text-rose-500')">
                          {{ s.status === 1 ? 'ACTIVE' : s.status === 0 ? 'PENDING' : 'CANCELLED' }}
                        </span>
                      </td>
                    </ng-container>

                    <tr mat-header-row *matHeaderRowDef="['bus', 'route', 'time', 'status']" class="!border-slate-800"></tr>
                    <tr mat-row *matRowDef="let row; columns: ['bus', 'route', 'time', 'status'];" class="!border-slate-800 hover:!bg-slate-800/30 transition-colors"></tr>
                  </table>
                  @if(operatorSchedules.length === 0) {
                    <div class="p-12 text-center text-slate-500 italic">
                      No active schedules found for your fleet.
                    </div>
                  }
                </mat-card>
              </div>
            </mat-tab>

            <!-- Offices Tab -->
            <mat-tab>
              <ng-template mat-tab-label>
                <mat-icon class="mr-2">business</mat-icon> Office Locations
              </ng-template>
              <div class="py-8">
                <div class="flex justify-between items-center mb-6">
                  <h2 class="text-2xl font-bold text-white">Service Centers</h2>
                  <button mat-flat-button color="accent" (click)="manageOffices()" class="!rounded-xl !bg-amber-600 !text-white !px-6"> Manage Offices </button>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                  @for(office of operatorOffices; track office.id) {
                    <mat-card class="!bg-slate-900 !border-slate-800 !p-6 !rounded-2xl group border-l-4 !border-l-amber-500">
                      <div class="flex justify-between items-start mb-4">
                        <h3 class="text-lg font-bold text-white">{{office.district}}</h3>
                        <mat-icon class="text-amber-500/30 group-hover:text-amber-500 transition-colors">business</mat-icon>
                      </div>
                      <p class="text-slate-400 text-sm leading-relaxed mb-4">{{office.address}}</p>
                    </mat-card>
                  }
                  @if(operatorOffices.length === 0) {
                    <div class="col-span-full py-20 text-center border-2 border-dashed border-slate-800 rounded-3xl">
                      <mat-icon class="text-slate-700 w-16 h-16 text-[64px] mb-4">location_off</mat-icon>
                      <p class="text-slate-500 font-medium">No service centers registered.</p>
                      <button mat-button color="accent" (click)="manageOffices()" class="mt-4">Register Offices Now</button>
                    </div>
                  }
                </div>
              </div>
            </mat-tab>
          </mat-tab-group>
        } @else {
          <!-- Waiting for Approval -->
          <div class="max-w-xl mx-auto py-24 text-center">
            <div class="w-32 h-32 bg-amber-500/10 text-amber-500 rounded-full flex items-center justify-center mx-auto mb-10 shadow-3xl shadow-amber-500/5">
              <mat-icon style="font-size: 64px; width: 64px; height: 64px;" class="animate-pulse">hourglass_empty</mat-icon>
            </div>
            <h2 class="text-4xl font-black text-white mb-6">Credential Review Underway</h2>
            <p class="text-slate-400 text-lg mb-12 leading-relaxed">
              The administrative council is currently verifying your operator application. 
              Management tools will be provisioned once your credentials are confirmed.
            </p>
            <div class="px-8 py-4 bg-slate-900/50 border border-slate-800 rounded-full inline-block">
              <p class="text-sm font-mono text-slate-500 uppercase tracking-widest flex items-center gap-3">
                <span class="w-2 h-2 rounded-full bg-amber-500 animate-ping"></span>
                Status Protocol: {{ operatorStatus === 2 ? 'DISABLED' : 'PENDING_REVIEW' }}
              </p>
            </div>
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    ::ng-deep .dark-tabs .mat-mdc-tab-header {
      background: transparent !important;
      border-bottom: 1px solid theme('colors.slate.800') !important;
    }
    ::ng-deep .dark-tabs .mat-mdc-tab {
      color: theme('colors.slate.500') !important;
      font-weight: 600 !important;
      height: 60px !important;
    }
    ::ng-deep .dark-tabs .mat-mdc-tab-labels { gap: 8px; }
    ::ng-deep .dark-tabs .mdc-tab--active {
      color: white !important;
    }
    ::ng-deep .dark-tabs .mdc-tab-indicator__content--underline {
      border-top-width: 3px !important;
      border-color: theme('colors.indigo.500') !important;
      border-radius: 4px 4px 0 0;
    }
  `]
})
export class OperatorDashboardComponent implements OnInit {
  userName = localStorage.getItem('userName');
  operatorStatus: number = 0;
  operatorOffices: any[] = [];
  operatorBuses: any[] = [];
  operatorSchedules: any[] = [];
  loading = true;

  private authService = inject(AuthService);
  private operatorService = inject(OperatorService);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    // Only show full-page loading if we have no initial state
    if (this.operatorBuses.length === 0) this.loading = true;

    this.operatorService.getProfile().subscribe({
      next: (res) => {
        this.userName = res.data.name;
        this.operatorStatus = res.data.status;
        if (this.operatorStatus === 1) {
          this.loadOffices();
          this.loadBuses();
          this.loadSchedules();
        }
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadOffices() {
    this.operatorService.getOffices().subscribe(res => {
      this.operatorOffices = res.data || [];
      this.cdr.detectChanges();
    });
  }

  loadBuses() {
    this.operatorService.getBuses().subscribe(res => {
      this.operatorBuses = res.data || [];
      this.cdr.detectChanges();
    });
  }

  loadSchedules() {
    this.operatorService.getSchedules().subscribe(res => {
      this.operatorSchedules = res.data || [];
      this.cdr.detectChanges();
    });
  }

  addBus() {
    this.dialog.open(AddBusDialogComponent, {
      width: '70vw',
      panelClass: 'dark-system-dialog'
    }).afterClosed().subscribe(res => {
      if (res) this.loadBuses();
    });
  }

  deleteBus(id: number) {
    if (confirm('Are you sure you want to remove this vehicle from your fleet?')) {
      this.operatorService.deleteBus(id).subscribe(() => {
        this.snack.open('Vehicle removed from registry', 'OK', { duration: 3000 });
        this.loadBuses();
      });
    }
  }

  addSchedule() {
    this.dialog.open(CreateScheduleDialogComponent, {
      width: '70vw',
      data: { buses: this.operatorBuses },
      panelClass: 'dark-system-dialog'
    }).afterClosed().subscribe(res => {
      if (res) this.loadSchedules();
    });
  }

  manageOffices() {
    this.dialog.open(OfficeManagementDialogComponent, {
      width: '70vw',
      data: { offices: this.operatorOffices },
      panelClass: 'dark-system-dialog'
    }).afterClosed().subscribe(res => {
      if (res) this.loadOffices();
    });
  }

  logout() { this.authService.logout(); }
}
