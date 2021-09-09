import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private toastr: ToastrService){
  }
  
  canActivate(): boolean {
    console.log(localStorage.getItem("user"));
    if (localStorage.getItem("user") !== null && JSON.parse(localStorage.user)['username'] == "admin") {
      return true;
    }
    this.toastr.error("You shall not pass!");
    return false;
  }
  
}
