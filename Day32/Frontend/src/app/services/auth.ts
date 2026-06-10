import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { User } from '../components/models/user.models';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class Auth {
    
    private userSubject = new BehaviorSubject<User | null>(null);


    user$ = this.userSubject.asObservable();

    constructor(private http: HttpClient){
      const user = localStorage.getItem('user');

      if (user) {
        this.userSubject.next(
          JSON.parse(user)
        );
      }
    }

    login(username: string,password: string): Observable<any>{
        return this.http.post<any>('https://dummyjson.com/auth/login',{username,password})
        .pipe(
          tap(user =>{
            console.log(user)
            this.userSubject.next(user)
            localStorage.setItem('user',JSON.stringify(user));
          })
        );
    }
    getProfile(): Observable<any> {

      const user = this.getCurrentUser();

      return this.http.get<any>(
        'https://dummyjson.com/auth/me',
        {
          headers: {
            Authorization: `Bearer ${user?.accessToken}`
          }
        });
      }

    getCurrentUser(){
      return this.userSubject.value;
    }

    

    logout(){
      this.userSubject.next(null)
      localStorage.removeItem('user');
    }

}
