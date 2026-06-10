import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Auth } from '../../services/auth';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  username = '';
  password = '';

  loading = false;

  constructor(
    private authService:Auth,
    private router: Router
  ){}

  login(){
    this.loading = true
    this.authService.login(
      this.username,
      this.password
    ).subscribe({
      next : () =>{
        this.loading = false
        this.router.navigate(["/dashboard"])
        alert('login successful');
      },

      error: ()=>{
        this.loading = false
        alert(
          'Invalid credentials'
        );
      }
    });
  }
}
