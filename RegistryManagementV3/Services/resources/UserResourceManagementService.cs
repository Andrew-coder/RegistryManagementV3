using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public class UserResourceManagementService : IResourceManagementService
    {
        private readonly IUnitOfWork _uow;

        public UserResourceManagementService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        
        public List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user)
        {
            if (parentCatalogId.HasValue)
            {
                return _uow.CatalogRepository.GetChildCatalogsFilteredBySecurityLevel(parentCatalogId.Value, user.SecurityLevel);
            }
            return GetRootCatalogsForUserFilteredBySecurityLevel(user.SecurityLevel);
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
            
            var securityLevel = user.SecurityLevel;
            resources = resources
                .Where(resource => resource.SecurityLevel <= securityLevel)
                .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
                .ToList();

            return resources;
        }
        
        private List<Catalog> GetRootCatalogsForUserFilteredBySecurityLevel(int securityLevel)
        {
            return _uow.CatalogRepository.FindRootCatalogsFilteredBySecurityLevel(securityLevel);
        }
    }
}