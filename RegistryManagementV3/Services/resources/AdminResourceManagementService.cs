using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public class AdminResourceManagementService : ResourceManagementService
    {
        private readonly IUnitOfWork _uow;

        public AdminResourceManagementService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public override List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user)
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

            return resources;
        }
        
        public override IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user)
        {
            var predicate = PredicateBuilder.New<Resource>(false)
                .Or(MatchResourceWithQuery(query))
                .Or(MatchResourceTagsWithQuery(query))
                .And(NotInStatus(ResourceStatus.Removed));

            return _uow.ResourceRepository.FindByPredicate(predicate)
                .AsNoTracking()
                .OrderByDescending(resource => resource.Priority)
                .ToList();
        }

        public override IList<Resource> SearchResourcesByFilterObject(ResourceFilter resourceFilter, ApplicationUser user)
        {
            var predicate = PredicateBuilder.New<Resource>(true)
                .And(MatchResourceWithQuery(resourceFilter.Query))
                .And(MatchResourceTagsWithTagsCollection(resourceFilter.Tags))
                .And(MatchResourceWithAuthorName(resourceFilter.AuthorName))
                .And(MatchResourceWithCreationDateRange(resourceFilter.CreationDateRange))
                .And(MatchResourceWithApprovalDateRange(resourceFilter.ApprovalDateRange))
                .And(NotInStatus(ResourceStatus.Removed));

            return _uow.ResourceRepository.FindByPredicate(predicate)
                .AsNoTracking()
                .OrderByDescending(KeyExtractors.GetValueOrDefault(resourceFilter.OrderBy, resource => resource.Priority))
                .ToList();
        }
    }
}