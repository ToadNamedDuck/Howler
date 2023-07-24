using Howler.Models;

namespace Howler.Repositories
{
    public interface IUserRepository
    {
        public User GetById(int id);
        public User GetByEmail(string email);
    }
}