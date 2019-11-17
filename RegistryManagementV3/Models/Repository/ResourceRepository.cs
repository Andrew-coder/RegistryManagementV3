using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Exception;

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
        
        public override Resource GetById(long id)
        {
            var foundResource = Context.Resources
                .Include(resource => resource.Author)
                .Include(resource => resource.Catalog)
                .Include(resource => resource.TagResources)
                .ThenInclude(tr => tr.Tag)
                .FirstOrDefault(resource => resource.Id == id);
            
            if (foundResource == null)
            {
                throw new EntityNotFoundException($"Resource entity with id [{id}] was not found");
            }

            return foundResource;
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