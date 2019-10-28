using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public class ResourceRepository : Repository<Resource>
    {
        public ResourceRepository(SecurityDbContext context) : base(context)
        {
        }

        public List<Resource> FindAllResourcesForRootCatalog()
        {
            return Context.Resources
                .Where(resource => resource.Catalog == null)
                .ToList();
        }

        public List<Resource> FindApprovedResourcesForRootCatalog()
        {
            return Context.Resources
                .Where(resource => resource.Catalog == null)
                .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
                .ToList();
        }

        public List<Resource> GetAllResourcesForCatalog(long catalogId)
        {
            return Context.Resources
                .Where(resource => resource.CatalogId == catalogId)
                .ToList();
        }

        public List<Resource> GetChildCatalogsByUserGroup(long catalogId)
        {
            return Context.Resources
                .Where(resource => resource.CatalogId == catalogId)
                .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
                .ToList();
        }

        public IList<Resource> GetAllResourcesByTagsOrderedByPriority(IList<string> tags)
        {
//            return Context.Resources
//                .Where(resource => resource.Tags.Select(res => res.TagValue).Intersect(tags).Any())
//                .OrderByDescending(resource => resource.Priority).ToList();
            return null;
        }

        public IList<Resource> GetApprovedResourcesByTagsAndSecurityLevelOrderedByPriority(IList<string> tags, int securityLevel)
        {
//            return Context.Resources
//                .Where(resource => resource.Tags.Select(res => res.TagValue).Intersect(tags).Any())
//                .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
//                .Where(resource => resource.SecurityLevel <= securityLevel)
//                .OrderByDescending(resource => resource.Priority).ToList();
            return null;
        }
    }
}