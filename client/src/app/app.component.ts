import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from 'src/_models/user';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Shopping center app';
  users:any;
  constructor(private http: HttpClient, private accountService: AccountService){

  }

  ngOnInit() {
    this.setCurrentUser();
  }

  setCurrentUser(){
    if(localStorage.getItem('user')){
      const user: User = JSON.parse(localStorage.user);
      this.accountService.setCurrentUser(user);
    }
  
  }

}
