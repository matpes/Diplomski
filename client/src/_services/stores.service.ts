import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Store } from 'src/_models/store';

@Injectable({
  providedIn: 'root'
})
export class StoresService {

  private baseUrl = "https://localhost:5001/api";
  constructor(private http: HttpClient) { }

  public updateStoreCrawler(store: Store){
    return this.http.post(`${this.baseUrl}/stores/details/crawl`, store);
  }

  public getAllStores(){
    return this.http.get(`${this.baseUrl}/stores`);
  }
  
  deleteArticlesFromStore(id: any) {
    return this.http.get(`${this.baseUrl}/stores/delete/${id}`);
  }

}
