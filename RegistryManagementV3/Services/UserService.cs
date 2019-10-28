using RegistryManagementV3.Models;

namespace RegistryManagementV3.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        //public User LoginUser(string email, string password)
        //{
        //    return _unitOfWork.UserRepository.AllEntities.SingleOrDefault(x => x.Email == email && x.Password == password);
        //}
    }
}