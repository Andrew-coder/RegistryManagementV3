using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface IUserService
    {
        ApplicationUser GetById(string id);
    }
}
