import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core'; // 1. Import signal
import { RouterLink } from '@angular/router';
import { Product } from '../models/product.models';
import { ProductService } from '../../services/product-service';

@Component({
  selector: 'app-products',
  imports: [RouterLink],
  templateUrl: './products.html',
})
export class Products implements OnInit {

  products = signal<Product[]>([]);

  loading = signal(true);

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    
    this.productService.getProducts().subscribe({
      next: prod => {
        this.products.set(prod); 
        this.loading.set(false);
      },
      error: err => {
        console.log(err)
        this.loading.set(false);
      }
      
    });
   
  }
}