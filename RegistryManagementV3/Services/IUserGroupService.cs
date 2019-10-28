using System.Collections.Generic;
using System.Collections.ObjectModel;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface IUserGroupService
    {
        List<UserGroup> GetAllUserGroups();
        List<UserGroup> GetUserGroupsWithNames(ICollection<string> names);
        UserGroup GetUserGroupById(long id);
        void CreateUserGroup(UserGroup userGroup);
        void UpdateUserGroup(UserGroup userGroup);
        void DeleteUserGroup(long id);
    }
}