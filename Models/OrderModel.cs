namespace Shopping_Coffee.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string Ordercode { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string PaymentMethod { get; set; }
    }
}
