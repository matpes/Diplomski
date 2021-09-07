import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
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
  registerForm: FormGroup;
  maxdate : Date = new Date("12/31/2008");

  bsConfig: Partial<BsDatepickerConfig>;

  constructor(private http: HttpClient, private accountService: AccountService, 
    private toastr:ToastrService, private fb: FormBuilder, private router:Router) {

    this.bsConfig = {
      containerClass: 'theme-orange',
      dateInputFormat: 'DD/MM/YYYY'
    }

   }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(){
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      mail: ['', Validators.required],
      name: ['', Validators.required],
      surname: ['', Validators.required],
      gender: [' '],
      dateOfBirth: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmedPassword: ['', [Validators.required, this.matchValues('password')]]
    });
    this.registerForm.controls.password.valueChanges.subscribe(()=>{
      this.registerForm.controls.confirmedPassword.updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn{
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
    }
  }

  registerToggle(){
    this.registerMode = ! this.registerMode;
  }

  register(){
    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/');
      this.cancel();
    }, err =>{
      this.toastr.error(err.error);
    });
    
  }

  cancel(){
    this.registerMode = false;
    console.log('cancelled');
  }

  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe(users => this.users=users);
  }

}
