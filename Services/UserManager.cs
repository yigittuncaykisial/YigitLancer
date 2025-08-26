using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserManager(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public IEnumerable<User> GetAllUsers(bool trackChanges)
        {
            return _userRepository.FindAll(trackChanges).ToList();
        }

        public void CreateUser(User user)
        {
            // Şifreyi hash'le
            user.UserPassword = _passwordHasher.HashPassword(user, user.UserPassword);
            _userRepository.Create(user);
            _userRepository.Save();
        }

        public bool ExistsByUserName(string userName) => _userRepository.ExistsByUserName(userName);
        public bool ExistsByEmail(string email) => _userRepository.ExistsByEmail(email);



        public void UpdateUser(User user)
        {
            _userRepository.Update(user);
            _userRepository.Save();
        }

        public void DeleteUser(User user)
        {
            _userRepository.Delete(user);
            _userRepository.Save();
        }

        public User GetUserById(int id, bool trackChanges = false)
        {
            return _userRepository.FindByCondition(u => u.UserId == id, trackChanges).FirstOrDefault();
        }

        public User GetUserByUsername(string username, bool trackChanges = false)
        {
            return _userRepository.FindByCondition(u => u.UserName == username, trackChanges).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _userRepository.FindByCondition(u => u.UserEmail == email, false).FirstOrDefault();
        }

        public User Login(string username, string password)
        {
            // Null/empty guard — null password ile hasher çağırmayalım
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _userRepository.FindByCondition(u => u.UserName == username, false).FirstOrDefault();
            if (user == null)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.UserPassword, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

    }
}
