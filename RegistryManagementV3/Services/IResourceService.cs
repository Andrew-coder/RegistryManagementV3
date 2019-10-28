using System.Collections.Generic;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface IResourceService
    {
        List<Resource> GetAllResources(long? catalogId);
        List<Resource> GetRootResourcesForUserGroup();
        IList<Resource> GetAllResourcesForCatalogAndUser(long? catalogId, ApplicationUser user, bool isAdmin);
        List<Resource> GetChildResourcesByUserGroup(long? catalogId, string userGroup);
        Resource GetById(long id);
        void CreateResource(ResourceViewModel resourceViewModel, long catalogId);
        void ApproveResource(long resourceId);
        void UpdateResource(UpdateResourceViewModel resourceViewModel, Resource resource);
    }
}