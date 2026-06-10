import { Component, signal } from '@angular/core';
import { Auth } from '../../services/auth';
import { User } from '../models/user.models';
import { single } from 'rxjs';

@Component({
  selector: 'app-profile',
  imports: [],
  templateUrl: './profile.html',
  styleUrl: './profile.css',
})
export class Profile {

  user = signal<any>(null);

  loading = signal(true);

  constructor(public authService:Auth){}

  ngOnInit():void{
    this.authService.getProfile()
    .subscribe({
      next: user =>{
        this.user.set(user);
        console.log(user)
        this.loading.set(false);
      },
      error : err =>{
        console.log(err);
        this.loading.set(false)
      }
    })
  }
}
