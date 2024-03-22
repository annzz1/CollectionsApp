using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.ViewModels
{
    public class UserVM
    {
      
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
    }
}
