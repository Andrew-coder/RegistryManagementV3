using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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

        public override IQueryable<Resource> FindByPredicate(Expression<Func<Resource, bool>> predicate)
        {
            return Context.Set<Resource>()
                .AsExpandable()
                .Include(s => s.Catalog)
                .Include(s => s.TagResources)
                .ThenInclude(tr => tr.Tag)
                .Where(predicate);
        }

        public List<Resource> GetAllResourcesForCatalog(long catalogId)
        {
            return Context.Resources
                .Where(resource => resource.CatalogId == catalogId)
                .ToList();
        }

        public IEnumerable<Resource> FindAllResources()
        {

            return Context.Resources
                .Include(s => s.Catalog)
                .Include(s => s.TagResources)
                .ThenInclude(tr => tr.Tag)
                .ToList();
        }
    }
}