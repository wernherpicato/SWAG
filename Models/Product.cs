using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PH_Swag.Models
{
    [Table("Product")]
    public class Product
    {

        public int id { get; set; }

      //  public int prodCategoryID { get; set; }

      //  [ForeignKey("Color")]
      //  public int colorID { get; set; }

      //  public int sizeID { get; set; }

      //  public int prodGenderID { get; set; }

        public string? prodName { get; set; }    

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Must enter an description.")]
        public string? prodDescription { get; set; }

        public double prodPrice { get; set; }

        public string? prodSizes { get; set; }  

        //  public int prodQuantity { get; set; }

        // public string? prodImage { get; set; }

        //  public string? prodImageContentType { get; set; }

        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Must enter an image")]
       // public byte[]? prodData { get; set; }

        public int isActive { get; set; }


       // public string? catDescription { get; set; }

      //  public virtual Color Colors { get; set; }
        //public virtual ListChoice PayorSource { get; set; }
        //public virtual ListChoice ManagedCare { get; set; }
        //public virtual ListChoice DeliveryMethod { get; set; }


        
     //   public virtual ICollection<Image> Images { get; set; }


    }
}
