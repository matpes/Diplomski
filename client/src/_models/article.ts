import { ArticleImages } from "./articleImages";
import { Store } from "./store";

export interface Article {
    id : string;
    href: string;
    name : string;
    price : string;
    gender: string;
    type: string;
    imgSources : ArticleImages[];
    storeId: Store;
}