using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;

namespace RegistryManagementV3.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITagService _tagService;

        public ResourceService(IUnitOfWork uow, ITagService tagService)
        {
            _uow = uow;
            _tagService = tagService;
        }

        public List<Resource> GetAllResources(long? catalogId)
        {
            var resources = new List<Resource>();
            if (catalogId.HasValue)
            {
                resources = _uow.ResourceRepository
                    .AllEntities
                    .Where(resource => resource.CatalogId == catalogId)
                    .ToList();
            }
            else
            {
                resources = _uow.ResourceRepository
                    .AllEntities
                    .Where(resource => resource.Catalog == null)
                    .ToList();
            }
            return resources;
        }

        public List<Resource> GetRootResourcesForUserGroup()
        {
            return _uow.ResourceRepository.FindApprovedResourcesForRootCatalog();
        }

        public IList<Resource> GetAllResourcesForCatalogAndUser(long? catalogId, ApplicationUser user, bool isAdmin)
        {
            IList<Resource> resources = new List<Resource>();
            if (catalogId.HasValue)
            {
                resources = _uow.ResourceRepository
                    .GetAllResourcesForCatalog(catalogId.Value);
            }
            else
            {
                resources = _uow.ResourceRepository.FindAllResourcesForRootCatalog();
            }

            if (!isAdmin)
            {
                var securityLevel = user.UserGroup.SecurityLevel;
                resources = resources
                    .Where(resource => resource.SecurityLevel <= securityLevel)
                    .Where(resource => resource.ResourceStatus == ResourceStatus.Approved)
                    .ToList();
            }
            return resources;
        }

        public List<Resource> GetChildResourcesByUserGroup(long? catalogId, string groupName)
        {
            if (catalogId.HasValue)
            {
                return _uow.ResourceRepository.GetChildCatalogsByUserGroup(catalogId.Value);
            }

            return GetRootResourcesForUserGroup();
        }

        public Resource GetById(long id)
        {
            return _uow.ResourceRepository.GetById(id);
        }

        public void CreateResource(ResourceViewModel resourceViewModel, long catalogId)
        {
            var file = resourceViewModel.ResourceFile;
            var path = Path.Combine(resourceViewModel.ResourceLocation,  
                Path.GetFileName(file.FileName));  
            //TO-DO implement saving
            //            file.SaveAs(path);
            var catalog = _uow.CatalogRepository.GetById(catalogId);
            var priority = resourceViewModel.Priority ?? 0;
            var tagNames = new Collection<string>(resourceViewModel.Tags.Split(','));
            var tags = _tagService.GetTagsWithNames(tagNames);
            var resource = new Resource
            {
                Title = resourceViewModel.Title,
                Description = resourceViewModel.Description,
                Language = resourceViewModel.Language,
                Format = resourceViewModel.Format,
                SecurityLevel = resourceViewModel.SecurityLevel,
                Location = path,
                Priority = priority,
                //Tags = tags,
                ResourceStatus = ResourceStatus.PendingForCreationApprove,
                Catalog = catalog
            };
            _uow.ResourceRepository.Add(resource);
            _uow.Save();
        }

        public void ApproveResource(long resourceId)
        {
            var resource =_uow.ResourceRepository.GetById(resourceId);
            resource.ResourceStatus = ResourceStatus.Approved;
            _uow.Save();
        }

        public void UpdateResource(UpdateResourceViewModel resourceViewModel, Resource resource)
        {
            resource.Title = resourceViewModel.Title;
            resource.Description = resourceViewModel.Description;
            resource.Format = resourceViewModel.Format;
            resource.SecurityLevel = resourceViewModel.SecurityLevel;
            resource.Language = resourceViewModel.Language;
            resource.Priority = resourceViewModel.Priority;
            var tagNames = new Collection<string>(resourceViewModel.Tags.Split(','));
            var tags = _tagService.GetTagsWithNames(tagNames);
            //tags.Except(resource.Tags).ToList().ForEach(tag => resource.Tags.Add(tag));
            _uow.Save();
        }
    }
}