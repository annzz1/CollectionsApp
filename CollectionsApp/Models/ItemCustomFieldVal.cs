using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollectionsApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ItemCustomFieldVal
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("Item")]
        public string ItemId { get; set; }
        public virtual Item Item { get; set; }

        [ForeignKey("CustomField")]
        public string CustomFieldId { get; set; } // Changed property name to avoid conflict
        public virtual CustomField CustomField { get; set; }

        public string Value { get; set; } = string.Empty;

        public ItemCustomFieldVal()
        {
            Id = Guid.NewGuid().ToString();
        }
    }


}
