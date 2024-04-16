namespace PH_Swag.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime CheckoutTime { set; get; }
        public bool IsCheckedOut { get; set; }

        public int Quantity { get; set; }
        public double Value { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }

        public Cart()
        {
            CartItems = new List<CartItem>();
        }

        //public Cart(int customerId)
        //{
        //    CustomerId = customerId;
        //    CreationTime = DateTime.Now;
        //    IsCheckedOut = false;
        //    CartItems = new List<CartItem>();
        //}

    }
}
