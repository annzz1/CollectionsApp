using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    public class Comment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [ForeignKey("Item")]
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; }
        public virtual AppUser appuser { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }=DateTime.Now;
    }
}
