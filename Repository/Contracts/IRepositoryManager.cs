using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IJobRepository JobRepository { get; }
        IReviewRepository ReviewRepository { get; }

        void Save();  // Commit işlemi
    }
}
