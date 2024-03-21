using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
    