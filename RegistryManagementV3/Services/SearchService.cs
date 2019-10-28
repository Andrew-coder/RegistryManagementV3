using System.Collections.Generic;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _uow;

        public SearchService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IList<Resource> SearchResourcesByTags(IList<string> tags, ApplicationUser user, bool isAdmin)
        {
            if (isAdmin)
            {
                return _uow.ResourceRepository.GetAllResourcesByTagsOrderedByPriority(tags);
            }

            return _uow
                .ResourceRepository
                .GetApprovedResourcesByTagsAndSecurityLevelOrderedByPriority(tags, user.UserGroup.SecurityLevel);
        }
    }
}