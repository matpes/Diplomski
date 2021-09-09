import { AppUser } from "./appUser";
import { Article } from "./article";

export interface Cart{
    article :Article;
    appUser : AppUser;
    kolicina: number;
    bought: boolean;
    id:number;
}