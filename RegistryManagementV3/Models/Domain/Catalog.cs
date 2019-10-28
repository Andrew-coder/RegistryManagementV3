using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryManagementV3.Models.Domain
{
    public sealed class Catalog
    {
        public long Id { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Name { get; set; }
        [DefaultValue(5)] [Range(1, 10)] public int SecurityLevel { get; set; } = 5;
        public long? SuperCatalogId { get; set; }
        [ForeignKey("SuperCatalogId")]
        public Catalog SuperCatalog { get; set; }
        public ICollection<UserGroupCatalogs> UserGroups { get; set; }
    }
}