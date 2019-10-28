
using System.Linq;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public class UserGroupRepository : Repository<UserGroup>
    {
        public UserGroupRepository(SecurityDbContext context) : base(context)
        {
        }

        public UserGroup FindUserGroupByName(string name)
        {
            return Context.UserGroups.FirstOrDefault(userGroup => userGroup.Name.Equals(name));
        }
    }
}