using CollectionsApp.Models;

namespace CollectionsApp.ViewModels
{
    public class ItemPageVM
    {
        public Collection collection { get; set; }
        public List<Item> items { get; set; }   = new List<Item>();
    }
}
