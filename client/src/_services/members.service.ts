import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppUser } from 'src/_models/appUser';

const httpOptions = {
  headers: new HttpHeaders({
    Autorization: 'Bearer' + JSON.parse(localStorage.getItem('user')).token
  })
}

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = "https://localhost:5001/api/";
  constructor(private http: HttpClient) { }


  getUsers(){
    return this.http.get<AppUser>(`${this.baseUrl}users`, httpOptions);
  }

  getUser(username){
    return this.http.get<AppUser>(`${this.baseUrl}users/${username}`, httpOptions);
  }


}
