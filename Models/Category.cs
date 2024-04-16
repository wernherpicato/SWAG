using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PH_Swag.Models
{
    [Table("Category")]
    public class Category
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Description")]
        public string? catDescription { get; set; }

        [StringLength(50)]
        [Display(Name = "Name")]
        public string? catName { get; set; }

        public int sortOrder { get; set; }

        public int isActive { get; set; }
    }
}
