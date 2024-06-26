﻿using CollectionsApp.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    public class CustomField
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public CustomFieldTypes customFieldType { get; set; }
        public string Label { get; set; }
        
        [ForeignKey("Collection")]
        public string CollectionId { get; set; }

        public virtual Collection collection { get; set; }
        public virtual ICollection<ItemCustomFieldVal>CustomFieldVals { get; set; }
        public CustomField()
        {
            CustomFieldVals = new List<ItemCustomFieldVal>();
        }
    }
}
