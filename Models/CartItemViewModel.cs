namespace PH_Swag.Models
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int prodId { get; set; }
        public string? prodName { get; set; }
        public string? prodDescription { get; set; }
        public int quantity { set; get; }

        public byte[] prodData { get; set; }

        public string? prodSizes {  get; set; } 


    }
}
