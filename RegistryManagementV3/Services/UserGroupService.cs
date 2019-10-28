using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IUnitOfWork _uow;

        public UserGroupService(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public List<UserGroup> GetAllUserGroups()
        {
            return _uow.UserGroupRepository.AllEntities.ToList();
        }

        public List<UserGroup> GetUserGroupsWithNames(ICollection<string> names)
        {
            return _uow.UserGroupRepository.AllEntities.Where(group => names.Contains(group.Name)).ToList();
        }

        public UserGroup GetUserGroupById(long id)
        {
            return _uow.UserGroupRepository.GetById(id);
        }

        public void CreateUserGroup(UserGroup userGroup)
        {
            var existinUserGroup = _uow.UserGroupRepository.FindUserGroupByName(userGroup.Name);
            if (existinUserGroup != null)
            {
                throw new ArgumentException("Can not create user group with name: " + userGroup.Name);
            }
            _uow.UserGroupRepository.Add(userGroup);
            _uow.Save();
        }

        public void UpdateUserGroup(UserGroup userGroup)
        {
            var updatedUserGroup = _uow.UserGroupRepository.GetById(userGroup.Id);
            if (userGroup == null)
            {
                throw new ArgumentException("User group with such id doesn't exist: " + userGroup.Id);
            }

            updatedUserGroup.Name = userGroup.Name;
            updatedUserGroup.SecurityLevel = userGroup.SecurityLevel;
            _uow.Save();
        }

        public void DeleteUserGroup(long id)
        {
            var userGroup = _uow.UserGroupRepository.GetById(id);
            if (userGroup != null)
            {
                _uow.UserGroupRepository.Remove(userGroup);
            }
            _uow.Save();
        }
    }
}