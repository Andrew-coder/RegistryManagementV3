using System.Collections.Generic;

namespace RegistryManagementV3.Services.resources
{
    public class ResourceManagementStrategy
    {
        private readonly Dictionary<string, IResourceManagementService> _strategies;

        public ResourceManagementStrategy(IResourceManagementService adminResourceManagementService, IResourceManagementService userResourceManagementService)
        {
            _strategies = new Dictionary<string, IResourceManagementService>
            {
                { "admin", adminResourceManagementService },
                { "user", userResourceManagementService }
            };
        }

        public IResourceManagementService FindService(string roleName)
        {
            return _strategies.GetValueOrDefault(roleName.ToLower());
        }
    }
}