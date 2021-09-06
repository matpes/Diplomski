import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/_guards/auth.guard';
import { HomeComponent } from './articles/home/home.component';
import { SingleArticleComponent } from './articles/home/single-article/single-article.component';
import { UserEditComponent } from './members/user-edit/user-edit.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children:[
      { path: 'user/edit', component: UserEditComponent},
    ]
  },
  { path: 'register', component: RegisterComponent },
  { path: 'article/:id', component: SingleArticleComponent },
  { path: '**', component: HomeComponent, pathMatch: 'full'},
  // {
  //   path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [AuthGuard],

  // }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
