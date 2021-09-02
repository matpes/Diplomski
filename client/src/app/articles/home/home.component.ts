import { Component, OnInit } from '@angular/core';
import { Article } from 'src/_models/article';
import { ArticlesService } from 'src/_services/articles.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  articles : Article[]; 
  allArticles : Article[]
  constructor(private articlesService: ArticlesService) { }

  ngOnInit(): void {
    this.articlesService.getAllArticles().subscribe( (result:Article[]) =>{
      this.articles = result;
      this.allArticles = result
      // console.log(this.articles);
    }, err =>{
      console.log(err);
    })
  }

  selectGender(gender){
    this.articles = this.allArticles.filter(x=>x.gender==gender);
  }

}
