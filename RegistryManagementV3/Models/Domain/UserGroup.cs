using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryManagementV3.Models.Domain
{
    public sealed class UserGroup
    {
        public long Id { get; set; }
        [StringLength(30)]
        [Display(Name = "Імя")]
        [Column(TypeName = "NVARCHAR(50)")]
        public string Name { get; set; }
        [DefaultValue(5)]
        [Range(1, 10)]
        public int SecurityLevel { get; set; } = 5;
        public ICollection<UserGroupCatalogs> Catalogs { get; set; }
    }
}