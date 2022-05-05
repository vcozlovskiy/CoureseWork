using System.ComponentModel.DataAnnotations;

namespace Begenjov_B.Models.Account
{
    public class RegisterModel
    {
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "last name")]
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
