using CollectionsApp.Enums;
using CollectionsApp.Models;
using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.ViewModels
{
    public class CollectionVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public Category category { get; set; }
        public List<CustomFieldVM> CustomFields { get; set; } = new List<CustomFieldVM>();
    }
}
