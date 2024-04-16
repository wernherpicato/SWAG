using System.ComponentModel.DataAnnotations.Schema;

namespace PH_Swag.Models
{
    [Table("Images")]
    public class Image
    {
        public int ID { get; set; }

        [ForeignKey("Product")]
        public int? productID { get; set; }

        public string prodImageName { get; set; }

        public string prodImageContentType { get; set; }

        public byte[] prodData { get; set; }


       // public virtual Product Product { get; set; }

    }
}
