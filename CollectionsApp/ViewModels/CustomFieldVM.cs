using CollectionsApp.Enums;
using CollectionsApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.ViewModels
{
    public class CustomFieldVM
    {
        public CustomFieldTypes customFieldType { get; set; }
        public string Label { get; set; }
        public string ?Value { get; set; }
    }
}
