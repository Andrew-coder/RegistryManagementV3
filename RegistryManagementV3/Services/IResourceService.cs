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
        Resource GetById(long id);
        void CreateResourceOnBehalfOfUser(ResourceViewModel resourceViewModel, ApplicationUser user);
        void ApproveResource(long resourceId);
        void UpdateResource(UpdateResourceViewModel resourceViewModel, Resource resource);

        Resource MakeEditable(long id);
        void MarkResourceAsDeleted(long id);
    }
}