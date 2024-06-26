﻿using CollectionsApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    public class Item
    {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Tags { get; set; } = string.Empty;
        
        public DateTime AddedTime { get; set; }= DateTime.Now;
        [ForeignKey("Collection")]
        public string CollectionId { get; set; }

        public virtual Collection collection { get; set; }
        public virtual ICollection<ItemCustomFieldVal> ItemCustomFieldVals { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public Item()
        {
            Comments = new List<Comment>();
            ItemCustomFieldVals = new List<ItemCustomFieldVal>();
            Likes = new List<Like>();
        }
    }
}
