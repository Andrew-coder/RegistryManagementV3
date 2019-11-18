using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public class UserResourceManagementService : ResourceManagementService
    {
        private readonly IUnitOfWork _uow;

        public UserResourceManagementService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        
        public override List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user)
        {
            if (parentCatalogId.HasValue)
            {
                return _uow.CatalogRepository.GetChildCatalogsFilteredBySecurityLevel(parentCatalogId.Value, user.SecurityLevel);
            }
            return GetRootCatalogsForUserFilteredBySecurityLevel(user.SecurityLevel);
        }

        public override List<Resource> GetResourcesByParentCatalog(long? parentCatalogId, ApplicationUser user)
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
        
        public override IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user)
        {
            var predicate = PredicateBuilder.New<Resource>(false)
                .Or(MatchResourceWithQuery(query))
                .Or(MatchResourceTagsWithQuery(query))
                .And(HasStatus(ResourceStatus.Approved))
                .And(SecurityLevelIsLesserThan(user.SecurityLevel));
            
            return _uow.ResourceRepository.FindByPredicate(predicate)
                .AsNoTracking()
                .OrderByDescending(resource => resource.Priority)
                .ToList();
        }

        public override IList<Resource> SearchResourcesByFilterObject(ResourceFilter resourceFilter)
        {
            var predicate = PredicateBuilder.New<Resource>(true)
                .And(MatchResourceWithQuery(resourceFilter.Query))
                .And(MatchResourceTagsWithTagsCollection(resourceFilter.Tags))
                .And(MatchResourceWithAuthorName(resourceFilter.AuthorName))
                .And(MatchResourceWithCreationDateRange(resourceFilter.CreationDateRange))
                .And(MatchResourceWithApprovalDateRange(resourceFilter.ApprovalDateRange));

            return _uow.ResourceRepository.FindByPredicate(predicate)
                .AsNoTracking()
                .OrderBy(resource => resource.Format).ToList();
        }
        
        private List<Catalog> GetRootCatalogsForUserFilteredBySecurityLevel(int securityLevel)
        {
            return _uow.CatalogRepository.FindRootCatalogsFilteredBySecurityLevel(securityLevel);
        }
        private static Expression<Func<Resource, bool>> SecurityLevelIsLesserThan(int securityLevel)
        {
            return resource => resource.SecurityLevel <= securityLevel;
        }
        private static Expression<Func<Resource, bool>> HasStatus(ResourceStatus status)
        {
            return resource => resource.ResourceStatus == status;
        }
    }
}