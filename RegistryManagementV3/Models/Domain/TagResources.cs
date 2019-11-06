using System.ComponentModel.DataAnnotations;

namespace RegistryManagementV3.Models.Domain
{
    public class TagResources
    {
        [Key]
        public long TagId { get; set; }
        public Tag Tag { get; set; }
        [Key]
        public long ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}