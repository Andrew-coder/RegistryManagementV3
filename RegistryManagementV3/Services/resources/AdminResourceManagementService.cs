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
        
        public IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user)
        {
            var queryLowerInvariant = query.ToLowerInvariant();
            return _uow.ResourceRepository.FindAllResources()
                .Where(resource => MatchResourceWithQuery(resource, queryLowerInvariant))
                .OrderByDescending(resource => resource.Priority)
                .ToList();
        }
        
        private static bool MatchResourceWithQuery(Resource resource, string query)
        {
            if (resource.Description.ToLowerInvariant().Contains(query))
            {
                return true;
            }
            if (resource.Title.ToLowerInvariant().Contains(query))
            {
                return true;
            }
            if (resource.FileName.ToLowerInvariant().Contains(query))
            {
                return true;
            }

            if (resource.Tags != null)
            {
                if (resource.Tags.Any(tag => tag.TagValue.ToLowerInvariant().Contains(query)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}