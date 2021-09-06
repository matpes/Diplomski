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
  appUser: AppUser;
  user: User;

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

}
