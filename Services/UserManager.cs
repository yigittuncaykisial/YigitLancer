using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Repositories;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class UserManager : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly RepositoryContext _ctx;

        public UserManager(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, RepositoryContext ctx)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _ctx = ctx;
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

        public User? GetUserById(int id, bool trackChanges = false)
        {
            var q = _ctx.Users.AsQueryable();
            if (!trackChanges) q = q.AsNoTracking();
            return q.FirstOrDefault(u => u.UserId == id);
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
        public void DeleteUserAndRelated(int userId)
        {
            // İstersen süper admin kilidi koy:
            if (userId == 90) return;

            using var tx = _ctx.Database.BeginTransaction();

            // 1) Bu kullanıcının taraf olduğu TÜM sohbetler (job'lu/job'suz)
            var convIds = _ctx.Conversations
                .Where(c => c.BuyerUserId == userId || c.FreelancerUserId == userId)
                .Select(c => c.Id)
                .ToList();

            if (convIds.Count > 0)
            {
                // Mesajları sil
                var msgs = _ctx.Messages.Where(m => convIds.Contains(m.ConversationId)).ToList();
                if (msgs.Count > 0) _ctx.Messages.RemoveRange(msgs);

                // Sohbetleri sil
                var convs = _ctx.Conversations.Where(c => convIds.Contains(c.Id)).ToList();
                if (convs.Count > 0) _ctx.Conversations.RemoveRange(convs);

                _ctx.SaveChanges();
            }

            // 2) Bu kullanıcının AÇTIĞI işler -> CASCADE ile review/conv/msg/pr silinir
            var myJobs = _ctx.Jobs.Where(j => j.UserId == userId).ToList();
            if (myJobs.Count > 0)
            {
                _ctx.Jobs.RemoveRange(myJobs);
                _ctx.SaveChanges();
            }

            // 3) Bu kullanıcının ALICI olduğu işler -> PurchasedByUserId = null
            var purchased = _ctx.Jobs.Where(j => j.PurchasedByUserId == userId).ToList();
            if (purchased.Count > 0)
            {
                foreach (var j in purchased)
                    j.PurchasedByUserId = null;

                _ctx.SaveChanges();
            }

            // 4) Bu kullanıcıyla ilişkili review'lar (hem reviewer, hem freelancer tarafı)
            var relReviews = _ctx.Reviews
                .Where(r => r.ReviewerId == userId || r.FreelancerId == userId)
                .ToList();
            if (relReviews.Count > 0)
            {
                _ctx.Reviews.RemoveRange(relReviews);
                _ctx.SaveChanges();
            }

            // 5) Bu kullanıcının buyer olduğu purchase requests
            var prs = _ctx.PurchaseRequests.Where(p => p.BuyerUserId == userId).ToList();
            if (prs.Count > 0)
            {
                _ctx.PurchaseRequests.RemoveRange(prs);
                _ctx.SaveChanges();
            }

            // 6) En son kullanıcı
            var user = _ctx.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                _ctx.Users.Remove(user);
                _ctx.SaveChanges();
            }

            tx.Commit();
        }

    }
}
