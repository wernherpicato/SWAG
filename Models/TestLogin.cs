using System.ComponentModel.DataAnnotations;

namespace PH_Swag.Models
{
    public class TestLogin
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CERS ID should be numeric.")]
        public int username { get; set; }

        [Required]
        public string? password { get; set; }

        public string? strLoginMessage { get; set; }

    }
}
