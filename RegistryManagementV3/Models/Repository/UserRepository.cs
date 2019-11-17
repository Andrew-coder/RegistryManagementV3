using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public class UserRepository : Repository<ApplicationUser>
    {
        public UserRepository(SecurityDbContext context) : base(context)
        {
        }

        public ApplicationUser GetById(string id)
        {
            return Context.Set<ApplicationUser>().Find(id);
        }
    }
}