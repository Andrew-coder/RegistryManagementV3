using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
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
                .Or(MatchResourceTagsWithQuery(query));

            return _uow.ResourceRepository.FindByPredicate(predicate)
                .OrderByDescending(resource => resource.Priority)
                .ToList();
        }

        public override IList<Resource> SearchResourcesByFilterObject(ResourceFilter resourceFilter)
        {
            var predicate = PredicateBuilder.New<Resource>(true)
                .And(MatchResourceWithQuery(resourceFilter.Query))
                .And(MatchResourceTagsWithTagsCollection(resourceFilter.Tags))
                .And(MatchResourceWithCreationDateRange(resourceFilter.CreationDateRange))
                .And(MatchResourceWithApprovalDateRange(resourceFilter.ApprovalDateRange));

            return _uow.ResourceRepository.FindByPredicate(predicate)
                .OrderBy(resource => resource.Format).ToList();
        }
    }
}