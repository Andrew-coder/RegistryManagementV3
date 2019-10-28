using System.Linq;

namespace RegistryManagementV3.Models
{
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        void Update(T item);
        T GetById(long id);

        IQueryable<T> AllEntities { get; }
    }
}
