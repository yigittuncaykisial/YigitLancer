using Entities.Models;
using Repositories.Contracts;

namespace Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly RepositoryContext _context;

        public UserRepository(RepositoryContext context) : base(context)
        {
            _context = context;
        }

        public User GetUserById(int id, bool trackChanges = false)
        {
            var query = FindByCondition(u => u.UserId == id, trackChanges);
            return query.FirstOrDefault();
        }

        public User GetUserByUsername(string username, bool trackChanges = false)
        {
            var query = FindByCondition(u => u.UserName == username, trackChanges);
            return query.FirstOrDefault();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public bool ExistsByUserName(string userName) => _context.Users.Any(u => u.UserName == userName);
        public bool ExistsByEmail(string email) => _context.Users.Any(u => u.UserEmail == email);
        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
