import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Article } from 'src/_models/article';
import { ArticlesService } from 'src/_services/articles.service';

@Component({
  selector: 'app-single-article',
  templateUrl: './single-article.component.html',
  styleUrls: ['./single-article.component.css']
})
export class SingleArticleComponent implements OnInit {

  id: string;
  article: Article;
  constructor(private route:ActivatedRoute, private articlesService:ArticlesService) {
    this.id = this.route.snapshot.paramMap.get('id');
    this.articlesService.getArticleById(this.id).subscribe((art:Article)=>{
      this.article = art
    }, err => {
      console.log(err);
    })
   }

  ngOnInit(): void {
    
  }

}
