using Entities.Models;
using System.Collections.Generic;

namespace Services.Contracts
{
    public interface IUserService
    {
        IEnumerable<User> GetAllUsers(bool trackChanges);
        bool ExistsByUserName(string userName);
        bool ExistsByEmail(string email);
        void CreateUser(Entities.Models.User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        User Login(string username, string password);
        User GetUserById(int id, bool trackChanges = false);
        User GetUserByUsername(string username, bool trackChanges = false);
        Entities.Models.User GetUserByEmail(string email);
    }
}
