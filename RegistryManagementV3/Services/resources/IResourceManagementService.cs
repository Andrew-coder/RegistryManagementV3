using System.Collections.Generic;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public interface IResourceManagementService
    {
        List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user);
        List<Resource> GetResourcesByParentCatalog(long? parentCatalogId, ApplicationUser user);
        IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user);
    }
}