using System.Linq;
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