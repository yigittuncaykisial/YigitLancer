using Entities.Models;
using Repositories.Contracts;

namespace Repositories.Contracts
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        User GetUserById(int id, bool trackChanges = false);
        User GetUserByUsername(string username, bool trackChanges = false);
        bool ExistsByUserName(string userName);
        bool ExistsByEmail(string email);
        void CreateUser(Entities.Models.User user);


        void Save();
    }
}
