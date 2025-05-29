namespace Shopping_Coffee.Models
{
    public class CartItemModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Total { 
            get {return Quantity * Price;}
        }
        public string Image { get; set; }
        public CartItemModel() { 

        }
        public CartItemModel(ProductModel product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Image;

        }


    }
}
