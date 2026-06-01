export class ProductModel{
    constructor(public title: string = "", public price: string = "", public description: string = "", public thumbnail: string = "",){
        this.title = title;
        this.price = price;
        this.description = description;
        this.thumbnail = thumbnail;
    }
}