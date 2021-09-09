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
  allArticles : Article[];
  categories : Object [] = [];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 4;
  gender : any;
  sort: number = 0;
  constructor(private articlesService: ArticlesService) {
    this.loadCategories();
    this.loadArticles();
   }

  ngOnInit(): void {
  }

  sortArticles(event :any){
    this.sort = event;
    console.log(this.sort);
    this.filterArticles();
  }

  loadCategories(){
    this.articlesService.getCategories().subscribe((response:string[]) => {
      response.forEach((res)=>{
        let data = {
          category : res,
          selected : false
        }
        this.categories.push(data);
      })
    });
  }

  loadArticles(){
    this.articlesService.getAllArticles(this.pageNumber, this.pageSize).subscribe(response => {
      this.articles = response.result;
      this.pagination = response.pagination;
      console.log(this.articles);
    }, err =>{
      console.log(err);
    })
  }

  pageChanged(event:any){
    this.pageNumber = event.page;
    this.filterArticles();
  }

  numberChanged(event:any){
    this.pageSize = event;
    this.filterArticles();
  }

  filterArticles(){

    var selectedCategories: string [] = [];
    this.categories.filter(x => x['selected']==true).map( x => selectedCategories.push(x['category']));
    this.articlesService.getAllArticles(this.pageNumber, this.pageSize, this.gender, selectedCategories, this.sort).subscribe(response => {
      this.articles = response.result;
      this.pagination = response.pagination;
    }, err =>{
      console.log(err);
    });
  }

  resetFilters(){
    this.categories.forEach(data=>data['selected'] = false);
    this.gender = undefined;
  }

}
