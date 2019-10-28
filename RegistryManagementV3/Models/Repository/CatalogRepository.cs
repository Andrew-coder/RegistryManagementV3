using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public class CatalogRepository : Repository<Catalog>
    {
        public CatalogRepository(SecurityDbContext context) : base(context)
        {
           
        }

        public List<Catalog> FindRootCatalogs(UserGroup userGroup)
        {
            var catalogs = userGroup.Catalogs.Select(catalog => catalog.CatalogId).ToArray();
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalog == null)
                .Where(catalog => catalogs.Contains(catalog.Id))
                .ToList();
        }
        
        public List<Catalog> GetAllChildCatalogs(long catalogId)
        { 
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalogId == catalogId)
                .ToList();
        }
        
        public List<Catalog> GetChildCatalogsByUserGroup(long catalogId, UserGroup userGroup)
        {
            var catalogs = userGroup.Catalogs.Select(catalog => catalog.CatalogId).ToArray();
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalogId == catalogId)
                .Where(catalog => catalogs.Contains(catalog.Id))
                .ToList();
        }
    }
}