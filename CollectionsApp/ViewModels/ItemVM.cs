using System.ComponentModel.DataAnnotations;

namespace CollectionsApp.ViewModels
{
    public class ItemVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Tags { get; set; } = string.Empty;
    }
}
