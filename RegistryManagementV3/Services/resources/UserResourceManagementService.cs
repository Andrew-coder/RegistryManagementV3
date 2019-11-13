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
        
        public IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user)
        {
            var queryLowerInvariant = query.ToLowerInvariant();
            return _uow.ResourceRepository.FindAllResources()
                .Where(resource => MatchResourceWithQuery(resource, queryLowerInvariant))
                .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
                .Where(resource => user.SecurityLevel >= resource.SecurityLevel)
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
        
//        public IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user)
//        {
//            var queryLowerInvariant = query.ToLowerInvariant();
//            var predicate = PredicateBuilder.New<Resource>(true);
//            predicate = predicate.And(MatchResourceWithQuery(queryLowerInvariant));
//            predicate = predicate.And(HasStatus(ResourceStatus.Approved));
//            predicate = predicate.And(SecurityLevelIsLesserThan(user.SecurityLevel));
//                
//            return _uow.ResourceRepository.FindByPredicate(predicate)
//                .OrderByDescending(resource => resource.Priority)
//                .ToList();
//        }
        
        private List<Catalog> GetRootCatalogsForUserFilteredBySecurityLevel(int securityLevel)
        {
            return _uow.CatalogRepository.FindRootCatalogsFilteredBySecurityLevel(securityLevel);
        }

//        private static Expression<Func<Resource, bool>> MatchResourceWithQuery(string query)
//        {
//            var predicate = PredicateBuilder.New<Resource>(false);
//            predicate = predicate.Or(resource => resource.Description.ToLowerInvariant().Contains(query));
//            predicate = predicate.Or(resource => resource.Title.ToLowerInvariant().Contains(query));
//            predicate.Or(resource => resource.FileName.ToLowerInvariant().Contains(query));
//            return predicate.Or(resource => resource.Tags.Any(tag => tag.TagValue.ToLowerInvariant().Contains(query)));
//        }
//        
//        private static Expression<Func<Resource, bool>> SecurityLevelIsLesserThan(int securityLevel)
//        {
//            return resource => resource.SecurityLevel <= securityLevel;
//        }
//        private static Expression<Func<Resource, bool>> HasStatus(ResourceStatus status)
//        {
//            return resource => resource.ResourceStatus == status;
//        }
    }
}