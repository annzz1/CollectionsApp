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
        [NotMapped]
        public Dictionary<string, object> customfields { get; set; } = new Dictionary<string, object>();
        [ForeignKey("Collection")]
        public string CollectionId { get; set; }

        public virtual Collection collection { get; set; }
       
    }
}