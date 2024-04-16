namespace PH_Swag.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int cartId { get; set; }
        public int prodId { get; set; }
        public int p_quantity { get; set; }
        public string? p_Size { get; set; }

       

        //public CartItem(int cartId, int productId)
        //{
        //    CartId = cartId;
        //    ProductId = productId;
        //    Quantity = 1;
          
        //}
    }
}
