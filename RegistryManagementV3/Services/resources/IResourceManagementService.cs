using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services.resources
{
    public abstract class IResourceManagementService
    {
        public abstract List<Catalog> GetCatalogsByParentCatalog(long? parentCatalogId, ApplicationUser user);
        public abstract List<Resource> GetResourcesByParentCatalog(long? parentCatalogId, ApplicationUser user);
        public abstract IList<Resource> SearchResourcesByQuery(string query, ApplicationUser user);

        public abstract IList<Resource> SearchResourcesByFilterObject(ResourceFilter resourceFilter);
        
        protected static Expression<Func<Resource, bool>> MatchResourceWithQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return PredicateBuilder.New<Resource>(true);
            }

            var queryInLowerCase = query.ToLower();
            return PredicateBuilder.New<Resource>(false)
                .Or(resource => resource.Title.ToLower().Contains(queryInLowerCase))
                .Or(resource => resource.Description.ToLower().Contains(queryInLowerCase))
                .Or(resource => resource.FileName.ToLower().Contains(queryInLowerCase));
        }

        protected static Expression<Func<Resource, bool>> MatchResourceTagsWithQuery(string tagsQuery)
        {
            if (string.IsNullOrEmpty(tagsQuery))
            {
                return PredicateBuilder.New<Resource>(true);
            }

            return PredicateBuilder.New<Resource>(false)
                .Or(resource =>
                    resource.TagResources.Any(tag => tag.Tag.TagValue.ToLower().Contains(tagsQuery.ToLower())));
        }
        
        protected static Expression<Func<Resource, bool>> MatchResourceTagsWithTagsCollection(IReadOnlyCollection<string> tagsQuery)
        {
            if (tagsQuery == null || !tagsQuery.Any())
            {
                return PredicateBuilder.New<Resource>(true);
            }
            var predicate = PredicateBuilder.New<Resource>(false);
            foreach (var tag in tagsQuery)
            {
                predicate = predicate.Or(resource =>
                    resource.TagResources.Any(tagResource =>
                        tagResource.Tag.TagValue.ToLower().Contains(tag.ToLower())));
            }

            return predicate;
        }

        protected static Expression<Func<Resource, bool>> MatchResourceWithAuthorName(string authorName)
        {
            if (string.IsNullOrEmpty(authorName))
            {
                return PredicateBuilder.New<Resource>(true);
            }
            
            return PredicateBuilder.New<Resource>(true)
                .And(resource => resource.Author.UserName.ToLower().Contains(authorName.ToLower()));
        }
        
        protected static Expression<Func<Resource, bool>> MatchResourceWithCreationDateRange(Tuple<DateTime, DateTime> creationDateRange)
        {
            if (creationDateRange == null)
            {
                return PredicateBuilder.New<Resource>(true);
            }
            return PredicateBuilder.New<Resource>(true)
                .And(resource => resource.CreationTimestamp > creationDateRange.Item1)
                .And(resource => resource.CreationTimestamp < creationDateRange.Item2);
        }
        
        protected static Expression<Func<Resource, bool>> MatchResourceWithApprovalDateRange(Tuple<DateTime, DateTime> approvalDateRange)
        {
            if (approvalDateRange == null)
            {
                return PredicateBuilder.New<Resource>(true);
            }
            return PredicateBuilder.New<Resource>(true)
                .And(resource => resource.ApprovalTimestamp > approvalDateRange.Item1)
                .And(resource => resource.ApprovalTimestamp < approvalDateRange.Item2);
        }
    }
}