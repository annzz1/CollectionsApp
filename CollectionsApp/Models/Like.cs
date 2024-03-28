using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    public class Like
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Item")]
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser appuser{ get; set; }
    }
}
