import { Component, OnInit } from '@angular/core';
import { Article } from 'src/_models/article';
import { Pagination } from 'src/_models/pagination';
import { ArticlesService } from 'src/_services/articles.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  articles : Article[]; 
  allArticles : Article[]
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 4;
  constructor(private articlesService: ArticlesService) {
    this.loadArticles();
   }

  ngOnInit(): void {
  }

  loadArticles(){
    this.articlesService.getAllArticles(this.pageNumber, this.pageSize).subscribe(response => {
      this.articles = response.result;
      this.pagination = response.pagination;
    }, err =>{
      console.log(err);
    })
  }

  selectGender(gender){
    this.articles = this.allArticles.filter(x=>x.gender==gender);
  }

  pageChanged(event:any){
    this.pageNumber = event.page;
    this.loadArticles();
  }

  numberChanged(event:any){
    this.pageSize = event;
    this.loadArticles();
  }

}
