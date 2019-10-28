using RegistryManagementV3.Models.Repository;

namespace RegistryManagementV3.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecurityDbContext _context;

        public UnitOfWork(SecurityDbContext context)
        {
            _context = context;
            UserGroupRepository = new UserGroupRepository(_context);
            CatalogRepository = new CatalogRepository(_context);
            ResourceRepository = new ResourceRepository(_context);
            TagRepository = new TagRepository(_context);
        }

        public ResourceRepository ResourceRepository { get; }
        public CatalogRepository CatalogRepository { get; }
        public UserGroupRepository UserGroupRepository{ get; }
        public TagRepository TagRepository { get; }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}