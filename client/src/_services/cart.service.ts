import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppUser } from 'src/_models/appUser';
import { Article } from 'src/_models/article';
import { Cart } from 'src/_models/cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  private baseUrl = "https://localhost:5001/api";
  constructor(private http: HttpClient) { }

  public addToCart(cart:Cart){
    return this.http.post(`${this.baseUrl}/cart/add`, cart);
  }

  public getCartsForUser(id : number){
    return this.http.get(`${this.baseUrl}/cart/get/${id}`);
  }

  public buyAll(carts:Cart[]){
    return this.http.post(`${this.baseUrl}/cart/buyAll`, carts);
  }

  public removeCart(cartId:number){
    return this.http.get(`${this.baseUrl}/cart/remove/${cartId}`);
  }



}
