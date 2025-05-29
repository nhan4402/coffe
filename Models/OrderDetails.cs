using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Coffee.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Ordercode { get; set; }
       
        public int ProductId { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
       [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
