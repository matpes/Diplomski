import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ArticlesService {

  baseUrl = "https://localhost:5001/api";
  constructor(private http: HttpClient) { }

  getAllArticles(){
    return this.http.get(`${this.baseUrl}/articles`);
  }

  getArticleById(id:string){
    return this.http.get(`${this.baseUrl}/articles/${id}`);
  }

}
