import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { Article } from 'src/_models/article';
import { PaginatedResult } from 'src/_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class ArticlesService {

  baseUrl = "https://localhost:5001/api";
  paginatedResult: PaginatedResult<Article[]> = new PaginatedResult<Article[]>();

  constructor(private http: HttpClient) { }

  getAllArticles(page?:number, itemsPerPage?:number){
    let params = new HttpParams();
    if(page != null && itemsPerPage != null){
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }
    return this.http.get<Article[]>(`${this.baseUrl}/articles`, {observe: 'response', params}).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if(response.headers.get('Pagination') !== null){
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return this.paginatedResult;
      })
    );
  }

  getArticleById(id:string){
    return this.http.get(`${this.baseUrl}/articles/${id}`);
  }

}
