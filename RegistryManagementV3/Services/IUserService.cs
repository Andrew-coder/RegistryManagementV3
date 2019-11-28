using System.Threading.Tasks;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface IUserService
    {
        ApplicationUser GetById(string id);
        Task ApproveUserRegistrationByIdAsync(string id);
        void SetTwoFactorEnabled(ApplicationUser user, bool isTwoFactorEnabled);
    }
}
