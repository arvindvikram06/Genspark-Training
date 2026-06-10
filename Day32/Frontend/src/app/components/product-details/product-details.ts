import { Component, signal } from '@angular/core';
import { single } from 'rxjs';
import { Product } from '../models/product.models';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductService } from '../../services/product-service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-details',
  imports: [RouterLink],
  templateUrl: './product-details.html',
  styleUrl: './product-details.css',
})
export class ProductDetails {


  product = signal<Product | null>(null);

  isLoading = signal<boolean>(true);

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
 
    const productId = this.route.snapshot.paramMap.get('id');

    if (productId) {
      this.productService.getProductsById(productId).subscribe({
        next: (data) => {
          this.product.set(data);   
          this.isLoading.set(false); 
        },
        error: (err) => {
          console.error(err);
          this.isLoading.set(false);
        }
      });
    }



}
}
