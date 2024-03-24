using CollectionsApp.Enums;
using CollectionsApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.ViewModels
{
    public class CustomFieldVM
    {
        public string Id {  get; set; }=string.Empty;
        [Required(ErrorMessage = "Please select a field type.")]
        public CustomFieldTypes customFieldType { get; set; }
        public string Label { get; set; }
        
        public string Value { get; set; } = string.Empty;
    }
}
