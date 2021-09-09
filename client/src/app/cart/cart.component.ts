import { Component, OnInit } from '@angular/core';
import { AppUser } from 'src/_models/appUser';
import { Cart } from 'src/_models/cart';
import { AccountService } from 'src/_services/account.service';
import { CartService } from 'src/_services/cart.service';
import { MembersService } from 'src/_services/members.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit {

  carts: Cart[];
  user: AppUser;
  totalPrice: number = 0;
  constructor(private cartService:CartService, private membersService:MembersService, private accountService:AccountService) {

  }

  ngOnInit(): void {
    this.loadCarts();
  }

  loadCarts(){
    this.membersService.getUser(JSON.parse(localStorage.user)['username']).subscribe((ret:AppUser) =>{
      this.user = ret;
      this.cartService.getCartsForUser(this.user.id).subscribe((cartsReturned:Cart[])=>{
        this.carts = cartsReturned;
        this.totalPrice = 0;
        this.carts.forEach(cart =>{
          this.totalPrice += cart['article'].price;
        })
      });
    });
    
  }

  removeArticle(cart:Cart){
    this.cartService.removeCart(cart.id).subscribe(()=>{
      this.loadCarts();
    }, err =>{
      console.log(err);
    });
  }

  BuyAll(){
    this.cartService.buyAll(this.carts).subscribe(()=>{
      this.loadCarts();
    });

  }

}
