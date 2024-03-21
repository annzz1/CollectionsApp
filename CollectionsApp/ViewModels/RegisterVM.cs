using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Enter your firstname")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter your lastname")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Enter your username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}
