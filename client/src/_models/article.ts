import { ArticleImages } from "./articleImages";

export interface Article {
    id : string;
    href: string;
    name : string;
    price : string;
    gender: string;
    type: string;
    imgSources : ArticleImages[];
}