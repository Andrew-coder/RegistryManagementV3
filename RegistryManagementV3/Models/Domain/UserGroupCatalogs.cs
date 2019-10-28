namespace RegistryManagementV3.Models.Domain
{
    public class UserGroupCatalogs
    {
        public long CatalogId { get; set; }
        public Catalog Catalog { get; set; }
 
        public long UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }
    }
}