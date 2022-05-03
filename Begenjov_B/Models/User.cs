using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Begenjov_B.Models
{
    public class User
    {
        public Guid Id { get; set; }
        [Display(Name = "First name")]
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "last name")]
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public List<Role> Roles { get; set;}

        public User()
        {
            Roles = new List<Role>();
            Roles.Add(new Role() { Name = "User"});
        }

        public bool IsValid()
        {
            return true;
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Id: {Id}" + Environment.NewLine);
            sb.Append($"Name: {FirstName} {LastName}" + Environment.NewLine);
            sb.Append($"Email: {Email}" + Environment.NewLine);
            sb.Append($"IsActive: {IsActive}" + Environment.NewLine);
            sb.Append($"PasswordHash: {PasswordHash}");

            return sb.ToString();
        }
    }
}
