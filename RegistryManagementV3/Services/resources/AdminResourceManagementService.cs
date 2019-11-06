using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public class AdminResourceManagementService : IResourceManagementService
    {
        private readonly IUnitOfWork _uow;

        public AdminResourceManagementService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user)
        {
            List<Catalog> catalogs;
            if (parentCatalogId.HasValue)
            {
                catalogs = _uow.CatalogRepository.GetAllChildCatalogs(parentCatalogId.Value);
            }
            else
            {
                catalogs = _uow.CatalogRepository
                    .AllEntities
                    .Where(catalog => catalog.SuperCatalog == null)
                    .ToList();
            }
            return catalogs;
        }

        public List<Resource> GetResourcesByParentCatalog(long? parentCatalogId, ApplicationUser user)
        {
            List<Resource> resources;
            if (parentCatalogId.HasValue)
            {
                resources = _uow.ResourceRepository
                    .GetAllResourcesForCatalog(parentCatalogId.Value);
            }
            else
            {
                resources = _uow.ResourceRepository.FindAllResourcesForRootCatalog();
            }

            return resources;
        }
    }
}