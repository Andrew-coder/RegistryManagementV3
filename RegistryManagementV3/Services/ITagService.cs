using System.Collections.Generic;
using System.Collections.ObjectModel;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface ITagService
    {
        List<Tag> GetTagsWithNames(Collection<string> names);
    }
}
