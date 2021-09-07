import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { AppUser } from 'src/_models/appUser';
import { User } from 'src/_models/user';
import { AccountService } from 'src/_services/account.service';
import { MembersService } from 'src/_services/members.service';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.css']
})
export class UserEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm;
  @ViewChild('editForm') editPasswordForm: NgForm;
  
  appUser: AppUser;
  user: User;
  oldPass: string;
  newPass: string;
  confirmedPass;

  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) {
    accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  ngOnInit(): void {
    this.loadAppUser();
  }

  loadAppUser() {
    this.memberService.getUser(this.user.username).subscribe(member => {
      this.appUser = member;
    });
  }

  updateMember() {
    this.memberService.updateUser(this.appUser).subscribe(() => {
      this.toastr.success("Profi uspešno ažuriran");
      this.editForm.reset(this.appUser);
    });
  }

  updatePassword() {
    if (this.newPass!= undefined && this.newPass!= "" && this.newPass == this.confirmedPass) {
      this.memberService.updateUserPassword(this.user.username, this.oldPass, this.newPass).subscribe( () => {
        this.toastr.success("Lozinka uspešno promenjena");
        this.oldPass = this.newPass = this.confirmedPass = "";
      }, err => {
        console.log(err);
        this.oldPass = this.newPass = this.confirmedPass = "";
        this.toastr.error("Lozinka nije uspešno promenjena");
      });
    }else{
      this.toastr.error("Lozinka nije uspesno potvrdjena");
    }
    this.editPasswordForm.reset();
  }

}
