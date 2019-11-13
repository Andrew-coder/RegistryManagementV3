using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Services;
using RegistryManagementV3.Services.Extensions;

namespace RegistryManagementV3.Services
{
    public class ResourceService : IResourceService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITagService _tagService;
        private readonly string _registryPath;

        public ResourceService(IUnitOfWork uow, ITagService tagService, string registryPath)
        {
            _uow = uow;
            _tagService = tagService;
            _registryPath = registryPath;
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

        public Resource GetById(long id)
        {
            return _uow.ResourceRepository.GetById(id);
        }

        public void CreateResource(ResourceViewModel resourceViewModel, long catalogId)
        {
            var file = resourceViewModel.ResourceFile;
            var fileName = file.FileName;
            var path = Path.Combine(_registryPath, Path.GetFileName(fileName.AppendTimeStamp()));
            using (var fileStream = new FileStream(path, FileMode.Create)) {
                file.CopyTo(fileStream);
            }
            
            var catalog = _uow.CatalogRepository.GetById(catalogId);
            var priority = resourceViewModel.Priority ?? 0;
            var tagNames = new Collection<string>(resourceViewModel.Tags.Split(','));
            var fileExtension = Path.GetExtension(fileName);
            var tags = _tagService.GetTagsWithNames(tagNames);
            var resource = new Resource
            {
                Title = resourceViewModel.Title,
                Description = resourceViewModel.Description,
                Language = resourceViewModel.Language,
                Format = fileExtension,
                SecurityLevel = resourceViewModel.SecurityLevel,
                FileName = fileName,
                Location = path,
                Priority = priority,
                IsEditable = false,
                TagResources = new List<TagResources>(),
                ResourceStatus = ResourceStatus.PendingForCreationApprove,
                Catalog = catalog
            };
            var tagResources = tags.Select(tag =>
            {
                var tr = WireTagWithResource(tag, resource);
                tag.TagResources.Add(tr);
                return tr;
            }).ToList();
            
            resource.TagResources.AddRange(tagResources);
            tags.Where(tag => tag.Id == null)
                .ToList()
                .ForEach(tag => _uow.TagRepository.Add(tag));
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

        private static TagResources WireTagWithResource(Tag tag, Resource resource)
        {
            return new TagResources {Tag = tag, Resource = resource};
        }
    }
}