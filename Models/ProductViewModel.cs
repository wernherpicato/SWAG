using System.ComponentModel.DataAnnotations;


namespace PH_Swag.Models
{
    public partial class ProductViewModel
    {
        public int ID { get; set; }

        public double prodPrice { get; set; }

        public byte[] prodData { get; set; }


        [Required(ErrorMessage = "Name is required.")]
        public string? prodName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? prodDescription { get; set; }

       // [RequiredCustom(ErrorMessage = "Please check at least one checkbox.")]
        public  List<CheckboxModel> Checkboxes { get; set; }




        [Required(ErrorMessage = "At Least One Checkbox is required.")]
        //public List<string> sizeID { get; set; }   
        public string sizeIDs { get; set; }

        public string? catDescription { get; set; }


        //[Required(ErrorMessage = "Please select a fileWP.")]
        //[DataType(DataType.Upload)]
        //public IFormFile Photo { get; set; }
        ////public string Images { get; set; }  //to string





        //public int ID { get; set; }

        //[Display(Name = "Image")]
        //[Required(ErrorMessage = "Must enter an image")]
        //[Display(Name = "ImageWP")]
        //[Required(ErrorMessage = "Must enter an image")]
        //public byte[]? prodData { get; set; }

        //public string? proTitle { get; set; }

        //[Display(Name = "Description")]
        //[Required(ErrorMessage = "Must enter a descriptionWP.")]
        //public string prodDescription { get; set; }



        //public string? prodColors { get; set; }

        //public string? prodSizes { get; set; }



        // Image Properties
        //public string FileName { get; set; }
        //public string ContentType { get; set; }


        //[Required(ErrorMessage = "Please select a file.")]
        //[DataType(DataType.Upload)]
        //public IFormFile Photo { get; set; }

    }

   
        //protected override ValidationResult IsValid(object value, ValidationContext context)
        //{
        //    var vm = (ProductViewModel)context.ObjectInstance;

        //    if (vm.Checkboxes.Any(v => v.Checked))
        //    {
        //        return ValidationResult.Success;
        //    }

        //    return new ValidationResult(ErrorMessage);
        //}


        public class RequiredCustom : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var viewModel = (ProductViewModel)validationContext.ObjectInstance;

                var checkBoxCounter = 0;
                foreach (var plan in viewModel.Checkboxes)
                {
                    if (plan.Checked == true)
                    {
                        checkBoxCounter++;
                    }
                    if (plan.Checked == true && checkBoxCounter == 1)
                    {
                        return new ValidationResult(ErrorMessage = "You have selected checkbox!");

                    }
                    else
                    {
                        return new ValidationResult(ErrorMessage == null ? "Please check one checkbox!" : ErrorMessage);
                    }

                }

                return ValidationResult.Success;
            }
        }
}
