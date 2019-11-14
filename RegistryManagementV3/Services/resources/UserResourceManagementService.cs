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
        public IList<Resource> SearchResourcesByFilterObject(ResourceFilter resourceFilter)
        {
            var query = _uow.ResourceRepository.FindAllResources()
                .Where(resource => MatchResourceWithFilterObject(resource, resourceFilter));
            
//            if (!string.IsNullOrEmpty(resourceFilter.AuthorName))
//            {
//                query = query.Where()
//            }

            if (resourceFilter.CreationDateRange != null)
            {
                query = query.Where(resource =>
                    resource.CreationTimestamp < resourceFilter.CreationDateRange.Item2 &&
                    resource.CreationTimestamp > resourceFilter.CreationDateRange.Item1);
            }
            
            if (resourceFilter.ApprovalDateRange != null)
            {
                query = query.Where(resource =>
                    resource.ApprovalTimestamp < resourceFilter.ApprovalDateRange.Item2 &&
                    resource.ApprovalTimestamp > resourceFilter.ApprovalDateRange.Item1);
            }

            return query.ToList();
        }

        private static bool MatchResourceWithFilterObject(Resource resource, ResourceFilter resourceFilter)
        {
            var query = resourceFilter.Query.ToLowerInvariant();
            if (!string.IsNullOrEmpty(query))
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
            }

            if (resource.Tags != null && resourceFilter.Tags != null)
            {
                var hasSameElements = resource.Tags.Select(tag => tag.TagValue.ToLowerInvariant()).ToList()
                    .Intersect(resourceFilter.Tags.Select(tag => tag.ToLowerInvariant()).ToList()).Any();
                if (hasSameElements)
                {
                    return true;
                }
            }

            return false;
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

        private List<Catalog> GetRootCatalogsForUserFilteredBySecurityLevel(int securityLevel)
        {
            return _uow.CatalogRepository.FindRootCatalogsFilteredBySecurityLevel(securityLevel);
        }
    }
}