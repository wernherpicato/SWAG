using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PH_Swag.Models
{
    [Table("Color")]
    public class Color
    {
        public int ID { get; set; }

        [StringLength(15)]
        [Display(Name = "Color")]
        public string? color { get; set; }
    }
}
