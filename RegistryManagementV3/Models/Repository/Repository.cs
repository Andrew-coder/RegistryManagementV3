using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace RegistryManagementV3.Models.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected SecurityDbContext Context { get; }

        protected Repository(SecurityDbContext context)
        {
            Context = context;
        }
        public IQueryable<T> AllEntities => Context.Set<T>();

        public T GetById(long id)
        {
            return Context.Set<T>().Find(id);
        }

        public virtual IQueryable<T> FindByPredicate(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().AsExpandable().Where(predicate);
        }

        public void Add(T item)
        {
            Context.Set<T>().Add(item);
        }

        public void Remove(T item)
        {
            Context.Set<T>().Remove(item);
        }

        public void Update(T item)
        {
            Context.Entry(item).State = EntityState.Modified;
        }
    }
}