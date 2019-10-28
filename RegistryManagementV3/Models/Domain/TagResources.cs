namespace RegistryManagementV3.Models.Domain
{
    public class TagResources
    {
        public long TagId { get; set; }
        public Tag Tag { get; set; }
 
        public long ResourceId { get; set; }
        public Resource Resource { get; set; }
    }
}