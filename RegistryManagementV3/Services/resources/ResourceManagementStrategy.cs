using System.Collections.Generic;

namespace RegistryManagementV3.Services.resources
{
    public class ResourceManagementStrategy
    {
        private readonly Dictionary<string, ResourceManagementService> _strategies;

        public ResourceManagementStrategy(ResourceManagementService adminResourceManagementService, ResourceManagementService userResourceManagementService)
        {
            _strategies = new Dictionary<string, ResourceManagementService>
            {
                { "admin", adminResourceManagementService },
                { "user", userResourceManagementService }
            };
        }

        public ResourceManagementService FindService(string roleName)
        {
            return _strategies.GetValueOrDefault(roleName.ToLower());
        }
    }
}