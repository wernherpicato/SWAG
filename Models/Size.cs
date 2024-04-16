using System.ComponentModel.DataAnnotations.Schema;

namespace PH_Swag.Models
{
    [Table("Size")]
    public class Size
    {
        public int id { get; set; } 
        public string size { get; set; }

        public int sortOrder { get; set; }
        public int isActive {  get; set; } 

    }
}
