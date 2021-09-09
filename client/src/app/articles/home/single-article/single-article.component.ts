import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppUser } from 'src/_models/appUser';
import { Article } from 'src/_models/article';
import { Cart } from 'src/_models/cart';
import { AccountService } from 'src/_services/account.service';
import { ArticlesService } from 'src/_services/articles.service';
import { CartService } from 'src/_services/cart.service';
import { MembersService } from 'src/_services/members.service';

@Component({
  selector: 'app-single-article',
  templateUrl: './single-article.component.html',
  styleUrls: ['./single-article.component.css']
})
export class SingleArticleComponent implements OnInit {

  id: string;
  article: Article;
  user: AppUser;
  constructor(private route: ActivatedRoute, private articlesService: ArticlesService,
    private memberService: MembersService, private cartService: CartService, private router: Router,
    private toastr: ToastrService) {
    this.id = this.route.snapshot.paramMap.get('id');
    this.articlesService.getArticleById(this.id).subscribe((art: Article) => {
      this.article = art
    }, err => {
      console.log(err);
    });
    if (localStorage.getItem("user") != null) {
      this.memberService.getUser(JSON.parse(localStorage.user)['username']).subscribe(ret => {
        this.user = ret;
      });
    } else {
      this.user = null;
    }
  }

  ngOnInit(): void {

  }

  AddToCart(article: Article) {
    if (this.user == null) {
      this.router.navigateByUrl("/");
    } else {
      var cart: Cart = {
        article: article,
        appUser: this.user,
        kolicina: 1,
        bought: false,
        id: undefined
      };
      this.cartService.addToCart(cart).subscribe(ret => {
        this.toastr.success("Artikal dodat u korpu");
      }, err => {
        this.toastr.error("Artikal nije kupljen");
      });
    }
  }

}
