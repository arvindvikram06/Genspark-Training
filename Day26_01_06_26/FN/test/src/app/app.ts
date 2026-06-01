import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Customer } from './customer/customer';
import { Product } from './product/product';

@Component({
  selector: 'app-root',
  imports: [Customer,Product],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('test');
}
