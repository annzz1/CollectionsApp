using CollectionsApp.Enums;
using CollectionsApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.ViewModels
{
    public class CollectionVM
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Category category { get; set; }

        public List<CustomFieldVM> CustomFields { get; set; } = new List<CustomFieldVM>();

        // Property to hold the uploaded photo
        public IFormFile Photo { get; set; }
    }

}
