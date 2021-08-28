import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model: any = {};
  users: any;
  registerMode = false;


  constructor(private http: HttpClient, private accountService: AccountService) { }

  ngOnInit(): void {
  }

  registerToggle(){
    this.registerMode = ! this.registerMode;
  }

  register(){
    this.accountService.register(this.model).subscribe(response => {
      console.log(response);
      this.cancel();
    }, err =>{
      console.log(err);
    });
    console.log(this.model);
  }

  cancel(){
    this.registerMode = false;
    console.log('cancelled');
  }

  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(users => this.users=users);
  }

}
