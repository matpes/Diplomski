import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from 'src/_models/user';
import { AccountService } from 'src/_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  constructor(public accountService: AccountService, private toastr:ToastrService, private router : Router) {
  }

  ngOnInit(): void {
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      //console.log(response);
    }, err => {
      console.log(err);
      this.toastr.error(err.error);
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigate(['/']);
  }

}
