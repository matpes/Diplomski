import { Component, OnInit } from '@angular/core';
import { Store } from 'src/_models/store';
import { StoresService } from 'src/_services/stores.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  stores : Store[];

  constructor(private storesService:StoresService) { }

  ngOnInit(): void {
    this.loadStores();
  }

  loadStores(){
    this.storesService.getAllStores().subscribe((ret:Store[])=>{
      this.stores = ret;
    }, err =>{
      console.log(err);
    })
  }

  changedState(store:Store, event:any){
    console.log(event);
    store.crawling = event;
    this.storesService.updateStoreCrawler(store).subscribe(()=>{
      this.loadStores();
    });
  }

  deleteArticles(id){
    this.storesService.deleteArticlesFromStore(id);
  }

}
