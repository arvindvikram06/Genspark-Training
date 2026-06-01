import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CustomerModel } from '../models/customer.model';

@Component({
  selector: 'app-customer',
  imports: [FormsModule],
  templateUrl: './customer.html',
  styleUrl: './customer.css',
})
export class Customer {

  customer:CustomerModel = new CustomerModel("johndoe", "John Doe", "john.doe@example.com", "123-456-7890", "active", new Date());
  //  customer:CustomerModel = new CustomerModel();
   styclass: string = "bi bi-balloon-heart-fill";
   tabclass: string ="tableclass";
  handleChangeClick(){
  this.customer.name = "arvind"
  alert("Customer Name: " + this.customer.username);
  }
  toggleLike(){
    if(this.styclass === "bi bi-balloon-heart-fill"){
      this.styclass = "bi bi-balloon-heart";
    } else {
      this.styclass = "bi bi-balloon-heart-fill";
    }
  }
  
}
