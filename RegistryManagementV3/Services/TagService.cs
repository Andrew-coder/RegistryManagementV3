using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _uow;
        
        public TagService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public List<Tag> GetTagsWithNames(Collection<string> names)
        {
            return names.Distinct().Select(GetSingleTagByValueOrCreateNew).ToList();
        }

        private Tag GetSingleTagByValueOrCreateNew(string tagValue)
        {
            return _uow.TagRepository.CheckIfExists(tagValue) 
                ? _uow.TagRepository.FindTagByText(tagValue) 
                : CreateNewTag(tagValue);
        }

        private Tag CreateNewTag(string tagValue)
        {
            return new Tag {TagValue = tagValue, TagResources = new List<TagResources>()};
        }
    }
}