using CollectionsApp.Enums;
using CollectionsApp.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    public class Collection
    {
        public Collection()
        {
            CustomFields = new List<CustomField>();
            Items = new List<Item>();
        }

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Category category { get; set; }

        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }

        public virtual AppUser appUser { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public virtual List<CustomField> CustomFields { get; set; }

        // Property to hold the URL of the uploaded photo
        public string PhotoUrl { get; set; }
        /////
        //TO DO OTHER OPTIONAL FIELDS
    }

   
}
