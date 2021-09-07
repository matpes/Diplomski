import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppUser } from 'src/_models/appUser';


@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = "https://localhost:5001/api/";
  constructor(private http: HttpClient) { }


  getUsers(){
    return this.http.get<AppUser>(`${this.baseUrl}users`);
  }

  getUser(username){
    return this.http.get<AppUser>(`${this.baseUrl}users/${username}`);
  }

  updateUser(appUser: AppUser){
    return this.http.put(`${this.baseUrl}users`, appUser);
  }

  updateUserPassword(username:string, oldPass: string, newPass: string) {
    let data = {
      'Username': username,
      'Password' : oldPass,
      'NewPassword' : newPass
    }
    return this.http.post(`${this.baseUrl}users/password`, data);
  }

}
