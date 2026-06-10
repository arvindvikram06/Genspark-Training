import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { Product } from '../components/models/product.models';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  
  constructor(private http:HttpClient){}

  getProducts(): Observable<Product[]>{
    return this.http.get<any>("https://dummyjson.com/products")
    .pipe(
      map(response => response.products),
      catchError(err => {
        console.log(err)
        return throwError(()=> new Error("unable to fetch products"))
    })
    )
  }

  getProductsById(id:string|number):Observable<Product>{
    return this.http.get<any>(`https://dummyjson.com/products/${id}`)
    .pipe(
      catchError(err => {
        console.log(err)
        return throwError(()=> new Error("unable to fetch products"))
    })
    )
  }
} 
