using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PH_Swag.Models
{

    [Table("UserAccess")]
    public partial class UserAccess
    {
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "Employee")]
        public string? UserName { get; set; }
        [Display(Name = "User Management")]
        public bool Level1 { get; set; }
        [Display(Name = "Batch Entry")]
        public bool Level2 { get; set; }
        [Display(Name = "Add/Edit Cards")]
        public bool Level3 { get; set; }
        [Display(Name = "Create Appointments")]
        public bool Level4 { get; set; }
        [Display(Name = "Acknowledgement Forms")]
        public bool Level5 { get; set; }
        [Display(Name = "Delete Appointments")]
        public bool Level6 { get; set; }
        [Display(Name = "Mail Cards")]
        public bool Level7 { get; set; }
        [Display(Name = "Dosomething")]
        public bool Level8 { get; set; }
        [StringLength(50)]
        [Display(Name = "Added By")]
        public string? AddedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Added")]
        public DateTime? DateAdded { get; set; }
        public bool VoidFlag { get; set; }
    }

}
