using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryManagementV3.Models.Domain
{
    public class Resource
    {
        public long Id { get; set; }
        
        [Column(TypeName = "NVARCHAR(50)")]
        
        public string Title { get; set; }
        
        [Column(TypeName = "NVARCHAR(50)")]
        public string Description { get; set; }
        
        [Column(TypeName = "NVARCHAR(50)")]
        public string Language { get; set; }
        
        [Column(TypeName = "NVARCHAR(50)")]
        public string Format { get; set; }
        [DefaultValue(5)]
        [Range(1, 10)]
        public int SecurityLevel { get; set; } = 5;
        public int? Priority { get; set; }
        public ResourceStatus ResourceStatus { get; set; }
        
        [Column(TypeName = "NVARCHAR(50)")]
        public string Location { get; set; }
        public long? CatalogId { get; set; }
        [ForeignKey("CatalogId")]
        public virtual Catalog Catalog { get; set; }
        public virtual ICollection<TagResources> Tags { get; set; }
    }
}