using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Exception;

namespace RegistryManagementV3.Models.Repository
{
    public class CatalogRepository : Repository<Catalog>
    {
        public CatalogRepository(SecurityDbContext context) : base(context)
        {
           
        }

        public List<Catalog> FindRootCatalogsFilteredBySecurityLevel(int securityLevel)
        {
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalog == null)
                .Where(catalog => catalog.SecurityLevel >= securityLevel)
                .ToList();
        }

        public override Catalog GetById(long id)
        {
            var catalog = Context.Catalogs.Find(id);
            if (catalog == null)
            {
                throw new EntityNotFoundException($"Catalog entity with id [{id}] was not found");
            }

            return catalog;
        }

        public List<Catalog> GetAllChildCatalogs(long catalogId)
        { 
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalogId == catalogId)
                .ToList();
        }
        
        public List<Catalog> GetChildCatalogsFilteredBySecurityLevel(long catalogId, int securityLevel)
        {
            //var catalogs = userGroup.Catalogs.Select(catalog => catalog.CatalogId).ToArray();
            return Context.Catalogs
                .Where(catalog => catalog.SuperCatalogId == catalogId)
                .Where(catalog => catalog.SecurityLevel >= securityLevel)
                .ToList();
        }
    }
}