using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;

namespace RegistryManagementV3.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public ApplicationUser GetById(string id)
        {
            return _uow.UserRepository.GetById(id);
        }
    }
}